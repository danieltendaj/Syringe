using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Syringe.Service.Models;
using Syringe.Service.Parallel;
using Syringe.Tests.NUnitToXUnit;
using Syringe.Tests.StubsMocks;
using Xunit;

namespace Syringe.Tests.Unit.Service.Parallel
{
	public class BatchManagerTests
	{
		[Fact]
		public void should_start_new_test()
		{
			// given
			var testFileQueue = new TestFileQueueStub();
			var memoryCache = new MemoryCache(new MemoryCacheOptions());
			const string environment = "env-boot-ay";
			const string username = "hemang-and-di**s";
			string[] filenames = { "file1.sjon", "file2.json" };

			// when
			var batchManager = new BatchManager(testFileQueue, memoryCache, null);
			int batchId = batchManager.StartBatch(filenames, environment, username);

			// then
			Assert.Equal(batchId, 1);
			Assert.Equal(testFileQueue.Add_Tasks.Count, 2);
			Assert.Equal(testFileQueue.Add_Tasks[0].Environment, environment);
			Assert.Equal(testFileQueue.Add_Tasks[0].Username, username);
			Assert.Equal(testFileQueue.Add_Tasks[0].Filename, filenames[0]);

			Assert.Equal(testFileQueue.Add_Tasks[1].Environment, environment);
			Assert.Equal(testFileQueue.Add_Tasks[1].Username, username);
			Assert.Equal(testFileQueue.Add_Tasks[1].Filename, filenames[1]);

			var taskIds = memoryCache.Get(BatchManager.KeyPrefix + batchId) as List<int>;
			Assert.NotNull(taskIds);
			Assert.Equal(taskIds[0], 1);
			Assert.Equal(taskIds[1], 2);
		}

		[Fact]
		public void should_throw_exception_when_batch_isnt_found()
		{
			// given
			var testFileQueue = new TestFileQueueStub();
			var memoryCache = new MemoryCache(new MemoryCacheOptions());
			var resultFactory = new Mock<ITestFileResultFactory>();

			// when
			var batchManager = new BatchManager(testFileQueue, memoryCache, resultFactory.Object);

			// then
			Assert.Throws<KeyNotFoundException>(() => batchManager.GetBatchStatus(1));
		}

		[Fact]
		public void should_return_expected_batch_status_when_tests_pass()
		{
			// given
			const int batchId = 8;
			var testFileQueue = new Mock<ITestFileQueue>();
			var memoryCache = new MemoryCache(new MemoryCacheOptions());
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
			Assert.NotNull(batchStatus);
			Assert.Equal(batchStatus.BatchId, batchId);
			Assert.Equal(batchStatus.TestFilesResultIds.First(), runResult.ResultId);
			Assert.Equal(batchStatus.BatchFinished, Is.True);
			Assert.Equal(batchStatus.AllTestsPassed, Is.True);
			Assert.Equal(batchStatus.TestFilesRunning, 0);
			Assert.Equal(batchStatus.TestFilesFinished, 1);
			Assert.Empty(batchStatus.TestFilesWithFailedTests);
			Assert.Equal(batchStatus.TestFilesFailed, 0);
			Assert.Empty(batchStatus.FailedTasks);
		}

		[Fact]
		public void should_return_expected_batch_status_when_test_case_fails()
		{
			// given
			const int batchId = 4;
			var testFileQueue = new Mock<ITestFileQueue>();
			var memoryCache = new MemoryCache(new MemoryCacheOptions());
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
			Assert.NotNull(batchStatus);
			Assert.True(batchStatus.BatchFinished);
			Assert.False(batchStatus.AllTestsPassed);
			Assert.Equal(batchStatus.TestFilesFinished, 1);
			Assert.Equal(batchStatus.TestFilesWithFailedTests.First(), runResult.ResultId);
			Assert.Equal(batchStatus.TestFilesResultIds.First(), runResult.ResultId);
			Assert.Equal(batchStatus.TestFilesFailed, 0);
			Assert.Empty(batchStatus.FailedTasks);
		}

		[Fact]
		public void should_return_expected_batch_status_when_test_task_fails()
		{
			// given
			const int batchId = 4;
			var testFileQueue = new Mock<ITestFileQueue>();
			var memoryCache = new MemoryCache(new MemoryCacheOptions());
			var resultFactory = new Mock<ITestFileResultFactory>();

			memoryCache.Set($"{BatchManager.KeyPrefix}{batchId}", new List<int> { 5 }, DateTimeOffset.MaxValue);
			var testFileInfo = new TestFileRunnerTaskInfo(5);
			testFileQueue
				.Setup(x => x.GetTestFileTaskInfo(5))
				.Returns(testFileInfo);

			TestFileRunResult runResult = GenerateStubTestFileResult(taskFails: true);
			resultFactory
				.Setup(x => x.Create(testFileInfo, false, TimeSpan.Zero))
				.Returns(runResult);

			// when
			var batchManager = new BatchManager(testFileQueue.Object, memoryCache, resultFactory.Object);
			BatchStatus batchStatus = batchManager.GetBatchStatus(batchId);

			// then
			Assert.NotNull(batchStatus);
			Assert.False(batchStatus.BatchFinished);
			Assert.False(batchStatus.AllTestsPassed);
			Assert.Equal(batchStatus.TestFilesFinished, 0);
			Assert.Empty(batchStatus.TestFilesWithFailedTests);
			Assert.Empty(batchStatus.TestFilesResultIds);
			Assert.Equal(batchStatus.TestFilesFailed, 1);
			Assert.Equal(batchStatus.FailedTasks.First(), 5);
		}

		private static TestFileRunResult GenerateStubTestFileResult(bool failedTests = false, bool taskFails = false)
		{
			var runResult = new TestFileRunResult
			{
				ResultId = taskFails ? (Guid?)null : Guid.NewGuid(),
				Finished = !taskFails,
				HasFailedTests = failedTests,
				ErrorMessage = string.Empty,
				TestRunFailed = taskFails,
				TimeTaken = TimeSpan.FromDays(1),
				TestResults = taskFails ? new LightweightResult[0] : new[]
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