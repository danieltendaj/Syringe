using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Syringe.Core.Runner;
using Syringe.Core.Tasks;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Repositories;
using Syringe.Core.Tests.Results;

namespace Syringe.Service.Parallel
{
    /// <summary>
    /// A TPL based queue for running test files using the default <see cref="TestFileRunner"/>
    /// </summary>
    internal class ParallelTestFileQueue : ITestFileQueue, ITaskObserver
    {
        private int _lastTaskId;
        private readonly ConcurrentDictionary<int, TestFileRunnerTaskInfo> _currentTasks;
        private readonly ITestRepository _testRepository;
        private readonly ITestFileRunnerFactory _testFileRunnerFactory;
        private readonly ITaskPublisher _taskPublisher;

        public ParallelTestFileQueue(ITaskPublisher taskPublisher, ITestRepository testRepository, ITestFileRunnerFactory testFileRunnerFactory)
        {
            _currentTasks = new ConcurrentDictionary<int, TestFileRunnerTaskInfo>();
            _testRepository = testRepository;
            _testFileRunnerFactory = testFileRunnerFactory;

            _taskPublisher = taskPublisher;
        }

        /// <summary>
        /// Adds a request to run a test file to the queue of tasks to run.
        /// </summary>
        public int Add(TaskRequest item)
        {
            int taskId = Interlocked.Increment(ref _lastTaskId);

            var cancelTokenSource = new CancellationTokenSource();

            var taskInfo = new TestFileRunnerTaskInfo(taskId)
            {
                Request = item,
                StartTime = DateTime.UtcNow,
                Username = item.Username
            };

            Task childTask = StartSessionAsync(taskInfo);

            taskInfo.CancelTokenSource = cancelTokenSource;
            taskInfo.CurrentTask = childTask;

            _currentTasks.TryAdd(taskId, taskInfo);
            return taskId;
        }

        /// <summary>
        /// Starts the test file run.
        /// </summary>
        public async Task StartSessionAsync(TestFileRunnerTaskInfo item)
        {
            try
            {
                string filename = item.Request.Filename;

                TestFile testFile = _testRepository.GetTestFile(filename);
                testFile.Filename = filename;
                
                TestFileRunner runner = _testFileRunnerFactory.Create();
                item.Runner = runner;

                item.TestFileResults = await runner.RunAsync(testFile, item.Request.Environment, item.Request.Username);
            }
            catch (Exception e)
            {
                item.Errors = e.ToString();
            }
        }

        /// <summary>
        /// Shows minimal information about all test file requests in the queue, and their status,
        /// and who started the run.
        /// </summary>
        public IEnumerable<TaskDetails> GetRunningTasks()
        {
            return _currentTasks.Values.Select(task =>
            {
                TestFileRunner runner = task.Runner;

                return new TaskDetails()
                {
                    TaskId = task.Id,
                    Username = task.Username,
                    Status = task.CurrentTask.Status.ToString(),
                    IsComplete = task.CurrentTask.IsCompleted,
                    CurrentIndex = (runner != null) ? task.Runner.TestsRun : 0,
                    TotalTests = (runner != null) ? task.Runner.TotalTests : 0,
                };
            });
        }

        public TestFileRunnerTaskInfo GetTestFileTaskInfo(int taskId)
        {
            TestFileRunnerTaskInfo task;
            _currentTasks.TryGetValue(taskId, out task);

            return task;
        }

        /// <summary>
        /// Shows the full information about a *single* test run - it doesn't have to be running, it could be complete.
        /// This includes the results of every test in the test file.
        /// </summary>
        public TaskDetails GetRunningTaskDetails(int taskId)
        {
            TestFileRunnerTaskInfo task = GetTestFileTaskInfo(taskId);
            TestFileRunner runner = task.Runner;

            return new TaskDetails
            {
                TaskId = task.Id,
                Username = task.Username,
                Status = task.CurrentTask.Status.ToString(),
                Results = runner?.CurrentResults.ToList() ?? new List<TestResult>(),
                CurrentIndex = runner?.TestsRun ?? 0,
                TotalTests = runner?.TotalTests ?? 0,
                Errors = task.Errors
            };
        }

        public TaskMonitoringInfo StartMonitoringTask(int taskId)
        {
            TestFileRunnerTaskInfo task;
            _currentTasks.TryGetValue(taskId, out task);
            if (task == null)
            {
                return null;
            }

            TestFileRunner runner = task.Runner;

            _taskPublisher.Start(taskId, runner);

            return new TaskMonitoringInfo(runner.TotalTests);
        }
    }
}