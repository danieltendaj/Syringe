using System;
using System.Collections.Generic;
using System.Linq;
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
            const int batchId = 8;
            var testFileQueue = new Mock<ITestFileQueue>();
            var memoryCache = new MemoryCache("test");
            var resultFactory = new Mock<ITestFileResultFactory>();

            memoryCache.Set($"{BatchManager.KeyPrefix}{batchId}", new List<int> { 5 }, DateTimeOffset.MaxValue);
            var testFileInfo = new TestFileRunnerTaskInfo(5);
            testFileQueue
                .Setup(x => x.GetTestFileTaskInfo(5))
                .Returns(testFileInfo);

            TestFileRunResult runResult = GenerateStubTestFileResult();
            resultFactory
                .Setup(x => x.Create(testFileInfo, false, TimeSpan.Zero))
                .Returns(runResult);

            // when
            var batchManager = new BatchManager(testFileQueue.Object, memoryCache, resultFactory.Object);
            BatchStatus batchStatus = batchManager.GetBatchStatus(batchId);

            // then
            Assert.That(batchStatus, Is.Not.Null);
            Assert.That(batchStatus.BatchId, Is.EqualTo(batchId));
            Assert.That(batchStatus.TestFilesResultIds.First(), Is.EqualTo(runResult.ResultId));
            Assert.That(batchStatus.BatchFinished, Is.True);
            Assert.That(batchStatus.AllTestsPassed, Is.True);
            Assert.That(batchStatus.TestFilesRunning, Is.EqualTo(0));
            Assert.That(batchStatus.TestFilesFinished, Is.EqualTo(1));
            Assert.That(batchStatus.TestFilesWithFailedTests, Is.Empty);
            Assert.That(batchStatus.TestFilesFailed, Is.EqualTo(0));
            Assert.That(batchStatus.FailedTasks, Is.Empty);
        }

        [Test]
        public void should_return_expected_batch_status_when_some_tests_fail()
        {
            // given
            const int batchId = 4;
            var testFileQueue = new Mock<ITestFileQueue>();
            var memoryCache = new MemoryCache("test");
            var resultFactory = new Mock<ITestFileResultFactory>();

            memoryCache.Set($"{BatchManager.KeyPrefix}{batchId}", new List<int> { 5 }, DateTimeOffset.MaxValue);
            var testFileInfo = new TestFileRunnerTaskInfo(5);
            testFileQueue
                .Setup(x => x.GetTestFileTaskInfo(5))
                .Returns(testFileInfo);

            TestFileRunResult runResult = GenerateStubTestFileResult(failedTests: true);
            resultFactory
                .Setup(x => x.Create(testFileInfo, false, TimeSpan.Zero))
                .Returns(runResult);

            // when
            var batchManager = new BatchManager(testFileQueue.Object, memoryCache, resultFactory.Object);
            BatchStatus batchStatus = batchManager.GetBatchStatus(batchId);

            // then
            Assert.That(batchStatus, Is.Not.Null);
            Assert.That(batchStatus.BatchFinished, Is.True);
            Assert.That(batchStatus.AllTestsPassed, Is.False);
            Assert.That(batchStatus.TestFilesFinished, Is.EqualTo(1));
            Assert.That(batchStatus.TestFilesWithFailedTests.First(), Is.EqualTo(runResult.ResultId));
            Assert.That(batchStatus.TestFilesFailed, Is.EqualTo(0));
            Assert.That(batchStatus.FailedTasks, Is.Empty);
        }

        private static TestFileRunResult GenerateStubTestFileResult(bool failedTests = false)
        {
            var runResult = new TestFileRunResult
            {
                ResultId = Guid.NewGuid(),
                Finished = true,
                HasFailedTests = failedTests,
                ErrorMessage = string.Empty,
                TestRunFailed = false,
                TimeTaken = TimeSpan.FromDays(1),
                TestResults = new[]
                {
                    new LightweightResult
                    {
                        Success = !failedTests,
                        ActualUrl = "some-url",
                        AssertionsSuccess = true,
                        ExceptionMessage = "ExceptionMessage",
                        Message = "some=message",
                        ResponseCodeSuccess = true,
                        ResponseTime = TimeSpan.FromMinutes(4),
                        ScriptCompilationSuccess = true,
                        TestDescription = "supa-init-test-blah-blink-bloom",
                        TestUrl = "♪ ♫ ♬ 'I can't dream anymore, since you leeeeeft' ♪ ♫ ♬"
                    }
                }
            };
            return runResult;
        }
    }
}