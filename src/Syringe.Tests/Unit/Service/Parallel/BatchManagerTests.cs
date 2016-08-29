using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using Moq;
using NUnit.Framework;
using Syringe.Service.Models;
using Syringe.Service.Parallel;
using Syringe.Tests.StubsMocks;

namespace Syringe.Tests.Unit.Service.Parallel
{
    [TestFixture]
    public class BatchManagerTests
    {
        [Test]
        public void should_start_new_test()
        {
            // given
            var testFileQueue = new TestFileQueueStub();
            var memoryCache = new MemoryCache("test");
            const string environment = "env-boot-ay";
            const string username = "hemang-and-di**s";
            string[] filenames = { "file1.sjon", "file2.json" };

            // when
            var batchManager = new BatchManager(testFileQueue, memoryCache, null);
            int batchId = batchManager.StartBatch(filenames, environment, username);

            // then
            Assert.That(batchId, Is.EqualTo(1));
            Assert.That(testFileQueue.Add_Tasks.Count, Is.EqualTo(2));
            Assert.That(testFileQueue.Add_Tasks[0].Environment, Is.EqualTo(environment));
            Assert.That(testFileQueue.Add_Tasks[0].Username, Is.EqualTo(username));
            Assert.That(testFileQueue.Add_Tasks[0].Filename, Is.EqualTo(filenames[0]));

            Assert.That(testFileQueue.Add_Tasks[1].Environment, Is.EqualTo(environment));
            Assert.That(testFileQueue.Add_Tasks[1].Username, Is.EqualTo(username));
            Assert.That(testFileQueue.Add_Tasks[1].Filename, Is.EqualTo(filenames[1]));

            var taskIds = memoryCache[BatchManager.KeyPrefix + batchId] as List<int>;
            Assert.That(taskIds, Is.Not.Null);
            Assert.That(taskIds[0], Is.EqualTo(1));
            Assert.That(taskIds[1], Is.EqualTo(2));
        }

        [Test]
        public void should_throw_exception_when_batch_isnt_found()
        {
            // given
            var testFileQueue = new TestFileQueueStub();
            var memoryCache = new MemoryCache("test");
            var resultFactory = new Mock<ITestFileResultFactory>();

            // when
            var batchManager = new BatchManager(testFileQueue, memoryCache, resultFactory.Object);

            // then
            Assert.Throws<KeyNotFoundException>(() => batchManager.GetBatchStatus(1));
        }

        [Test]
        public void should_return_expected_batch_status_when_tests_pass()
        {
            // given
            var testFileQueue = new Mock<ITestFileQueue>();
            var memoryCache = new MemoryCache("test");
            var resultFactory = new Mock<ITestFileResultFactory>();

            memoryCache.Set($"{BatchManager.KeyPrefix}8", new List<int> { 5 }, DateTimeOffset.MaxValue);
            var testFileInfo = new TestFileRunnerTaskInfo(5);
            testFileQueue
                .Setup(x => x.GetTestFileTaskInfo(5))
                .Returns(testFileInfo);

            var runResult = new TestFileRunResult
            {
                ResultId = 
            };
            resultFactory
                .Setup(x => x.Create(testFileInfo, false, TimeSpan.Zero))
                .Returns(runResult);                

            // when
            var batchManager = new BatchManager(testFileQueue.Object, memoryCache, resultFactory.Object);
            BatchStatus batchStatus = batchManager.GetBatchStatus(1);

            // then
            Assert.That(batchStatus, Is.Not.Null);
            Assert.That(batchStatus.BatchId, Is.EqualTo(8));
            //Assert.That(batchStatus.BatchId, Is.EqualTo(8));
        }
    }
}