using System;
using System.Threading;
using Moq;
using NUnit.Framework;
using Syringe.Core.Configuration;
using Syringe.Core.Tasks;
using Syringe.Service.Jobs;
using Syringe.Service.Parallel;

namespace Syringe.Tests.Unit.Service.Jobs
{
    [TestFixture]
    public class TasksCleanupJobTests
    {
        private Mock<IConfiguration> _configurationMock;
        private Mock<ITestFileQueue> _testFileQueueMock;
        private int _callbackCount;
        private TasksCleanupJob _job;

        [SetUp]
        public void Setup()
        {
            _callbackCount = 0;
            _configurationMock = new Mock<IConfiguration>();
            _testFileQueueMock = new Mock<ITestFileQueue>();
            _job = new TasksCleanupJob(_configurationMock.Object, _testFileQueueMock.Object);
        }

        [Test]
        public void should_clear_results_before_older_than_specified()
        {
            // given
            TaskDetails[] tasks =
            {
                new TaskDetails { TaskId = 1, IsComplete = false, StartTime = DateTime.Now },
                new TaskDetails { TaskId = 6, IsComplete = true, StartTime = DateTime.Now.Subtract(TimeSpan.FromHours(5)) },
                new TaskDetails { TaskId = 55, IsComplete = true, StartTime = DateTime.Now.Subtract(_job._retention).AddMinutes(1) },
            };
            _testFileQueueMock
                .Setup(x => x.GetRunningTasks())
                .Returns(tasks);

            // when
            _job.Cleanup(null);

            // then
            _testFileQueueMock.Verify(x => x.Remove(6));

            _testFileQueueMock.Verify(x => x.Remove(1), Times.Never);
            _testFileQueueMock.Verify(x => x.Remove(55), Times.Never);
        }

        [Test]
        public void should_execute_given_callback_via_timer_and_then_stop()
        {
            // given
            _configurationMock
                .Setup(x => x.CleanupSchedule)
                .Returns(TimeSpan.FromMilliseconds(5));

            // when
            _job.Start(DummyCallback);

            // then
            Thread.Sleep(TimeSpan.FromMilliseconds(100));
            Assert.That(_callbackCount, Is.GreaterThanOrEqualTo(3));

            _job.Stop();
            int localCallbackStore = _callbackCount;
            Thread.Sleep(TimeSpan.FromMilliseconds(30));
            Assert.That(_callbackCount, Is.EqualTo(localCallbackStore));
        }
        
        private void DummyCallback(object guff)
        {
            _callbackCount++;
        }
    }
}