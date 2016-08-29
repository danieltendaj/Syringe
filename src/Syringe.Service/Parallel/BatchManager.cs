using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
using Syringe.Core.Tasks;
using Syringe.Service.Models;

namespace Syringe.Service.Parallel
{
    public class BatchManager : IBatchManager
    {
        internal const string KeyPrefix = "batch_";
        private int _lastBatchId;
        private readonly ITestFileQueue _testFileQueue;
        private readonly ObjectCache _objectCache;
        private readonly ITestFileResultFactory _testFileResultFactory;

        public BatchManager(ITestFileQueue testFileQueue, ObjectCache objectCache, ITestFileResultFactory testFileResultFactory)
        {
            _testFileQueue = testFileQueue;
            _objectCache = objectCache;
            _testFileResultFactory = testFileResultFactory;
        }

        public int StartBatch(string[] filenames, string environment, string username)
        {
            int batchId = Interlocked.Increment(ref _lastBatchId);
            List<int> taskIds = new List<int>(filenames.Length);
            
            foreach (string filename in filenames)
            {
                var taskRequest = new TaskRequest
                {
                    Environment = environment,
                    Filename = filename,
                    Username = username
                };

                int taskId = _testFileQueue.Add(taskRequest);
                taskIds.Add(taskId);
            }

            // cache batch info only for a limited time...
            string key = $"{KeyPrefix}{batchId}";
            _objectCache.Set(key, taskIds, DateTimeOffset.Now.AddHours(1));

            return batchId;
        }

        public BatchStatus GetBatchStatus(int batchId)
        {
            string key = $"{KeyPrefix}{batchId}";

            if (!_objectCache.Contains(key))
            {
                throw new KeyNotFoundException($"Unknown batch id: {batchId}");
            }

            var batchInfo = (List<int>)_objectCache[key];
            var testFilesState = new List<TestFileRunResult>(batchInfo.Count);
            var failedTests = new List<int>();

            foreach (int taskId in batchInfo)
            {
                TestFileRunnerTaskInfo taskInfo = _testFileQueue.GetTestFileTaskInfo(taskId);
                TestFileRunResult testFileRunResult = _testFileResultFactory.Create(taskInfo, false, TimeSpan.Zero);
                testFilesState.Add(testFileRunResult);

                if (testFileRunResult.TestRunFailed)
                {
                    failedTests.Add(taskId);
                }
            }

            bool completed = testFilesState.TrueForAll(x => x.Completed);
            IEnumerable<Guid> testFilesResultIds = testFilesState
                                            .Where(x => x.ResultId.HasValue)
                                            .Select(x => x.ResultId.Value);

            return new BatchStatus
            {
                BatchId = batchId,
                TestFilesResultIds = testFilesResultIds,
                Completed = completed,
                AllTestsPassed = completed && testFilesState.TrueForAll(x => !x.HasFailedTests),
                TestFilesRunning = testFilesState.Count(x => !x.Completed),
                TestFilesCompleted = testFilesState.Count(x => x.Completed),
                TestFilesWithFailedTests = testFilesState.Where(x => x.HasFailedTests && x.ResultId.HasValue).Select(x => x.ResultId.Value),
                TestFilesFailed = failedTests.Count,
                FailedTasks = failedTests,
            };
        }
    }
}