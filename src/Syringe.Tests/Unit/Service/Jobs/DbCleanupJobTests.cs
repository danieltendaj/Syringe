using System;
using System.Threading;
using Moq;
using NUnit.Framework;
using Syringe.Core.Configuration;
using Syringe.Core.Tests.Results.Repositories;
using Syringe.Service.Jobs;

namespace Syringe.Tests.Unit.Service.Jobs
{
    [TestFixture]
    public class DbCleanupJobTests
    {
        private Mock<IConfiguration> _configurationMock;
        private Mock<ITestFileResultRepository> _repositoryMock;
        private int _callbackCount;
        private DbCleanupJob _job;

        [SetUp]
        public void Setup()
        {
            _callbackCount = 0;
            _configurationMock = new Mock<IConfiguration>();
            _repositoryMock = new Mock<ITestFileResultRepository>();
            _job = new DbCleanupJob(_configurationMock.Object, _repositoryMock.Object);
        }

        [Test]
        public void should_clear_results_before_now()
        {
            // given
            const int expectedDaysOfRetention = 66;
            _configurationMock
                .Setup(x => x.DaysOfDataRetention)
                .Returns(expectedDaysOfRetention);

            // when
            _job.Cleanup();

            // then
            _repositoryMock
                .Verify(x => x.DeleteBeforeDate(DateTime.Today.AddDays(-expectedDaysOfRetention)), Times.Once);
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

        [Test]
        public void start_should_clear_data()
        {
            // given

            // when
            _job.Start();
            Thread.Sleep(TimeSpan.FromMilliseconds(50));

            // then
            _repositoryMock
                .Verify(x => x.DeleteBeforeDate(It.IsAny<DateTime>()), Times.AtLeastOnce);

            _job.Stop();
        }

        private void DummyCallback(object guff)
        {
            _callbackCount++;
        }
    }
}