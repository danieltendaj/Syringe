using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;
using Syringe.Core.Configuration;
using Syringe.Core.Http;
using Syringe.Core.Repositories;
using Syringe.Core.Runner;
using Syringe.Core.Tasks;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Results;
using Syringe.Core.Xml.Reader;

namespace Syringe.Service.Parallel
{
	/// <summary>
	/// A TPL based queue for running test files using the default <see cref="TestFileRunner"/>
	/// </summary>
	internal class ParallelTestFileQueue : ITestFileQueue, ITaskObserver
	{
		private int _lastTaskId;
		private readonly ConcurrentDictionary<int, TestFileRunnerTaskInfo> _currentTasks;
		private readonly IConfiguration _configuration;
		private readonly ITestFileResultRepository _repository;
	    private readonly ITaskPublisher _taskPublisher;

	    public ParallelTestFileQueue(ITestFileResultRepository repository, ITaskPublisher taskPublisher, IConfiguration configuration)
		{
			_currentTasks = new ConcurrentDictionary<int, TestFileRunnerTaskInfo>();
			_configuration = configuration;

			_repository = repository;
	        _taskPublisher = taskPublisher;
		}

		/// <summary>
		/// Adds a request to run a test file to the queue of tasks to run.
		/// </summary>
		public int Add(TaskRequest item)
		{
			int taskId = Interlocked.Increment(ref _lastTaskId);

			var cancelTokenSource = new CancellationTokenSource();

			var taskInfo = new TestFileRunnerTaskInfo(taskId);
			taskInfo.Request = item;
			taskInfo.StartTime = DateTime.UtcNow;
			taskInfo.Username = item.Username;
			taskInfo.BranchName = item.BranchName;
			taskInfo.Position = item.Position;

		    Task childTask = StartSessionAsync(taskInfo);

			taskInfo.CancelTokenSource = cancelTokenSource;
			taskInfo.CurrentTask = childTask;

			_currentTasks.TryAdd(taskId, taskInfo);
			return taskId;
		}

		/// <summary>
		/// Hacky, will be fixed later.
		/// </summary>
		internal string RunTestFile(string filename)
		{
			try
			{
				string branchName = "";
				string xmlFilename = filename;
				string fullPath = Path.Combine(_configuration.TestFilesBaseDirectory, branchName, xmlFilename);
				string xml = File.ReadAllText(fullPath);

				using (var stringReader = new StringReader(xml))
				{
					var testCaseReader = new TestFileReader();
					TestFile testFile = testCaseReader.Read(stringReader);
					testFile.Filename = xmlFilename;

					var httpClient = new HttpClient(new RestClient());

					var runner = new TestFileRunner(httpClient, _repository);
					Task<TestFileResult> task = runner.RunAsync(testFile);
					bool success = task.Wait(TimeSpan.FromMinutes(3));

					if (success)
					{
						int failCount = runner.CurrentResults.Count(x => x.Success);
						if (failCount == 0)
						{
							return "success";
						}
						else
						{
							IEnumerable<TestResult> failedCases = runner.CurrentResults.Where(x => x.Success == false);
							string names = string.Join(",", failedCases.Select(x => "'" +x.Test.Description+ "'"));
							return $"fail - tests that failed: {names}";
						}
					}
					else
					{
						return "fail - the runner timed out or crashed.";
					}
				}
			}
			catch (Exception e)
			{
				return "fail - " + e.ToString();
			}
		}

		/// <summary>
		/// Starts the test file run.
		/// </summary>
		internal async Task StartSessionAsync(TestFileRunnerTaskInfo item)
		{
			try
			{
				string branchName = item.BranchName;

				// Read in the XML file from the branch folder, e.g. "c:\syringe\master\test1.xml"
				string xmlFilename = item.Request.Filename;
				string fullPath = Path.Combine(_configuration.TestFilesBaseDirectory, branchName, xmlFilename);
				string xml = File.ReadAllText(fullPath);

				using (var stringReader = new StringReader(xml))
				{
					var testCaseReader = new TestFileReader();
					TestFile testFile = testCaseReader.Read(stringReader);
                    if (item.Position.HasValue)
                    {
                        testFile.Tests = testFile.Tests.Where(x => x.Position == item.Position);
                    }
					testFile.Filename = xmlFilename;

					var httpClient = new HttpClient(new RestClient());

					var runner = new TestFileRunner(httpClient, _repository);
					item.Runner = runner;
					await runner.RunAsync(testFile);
				}
			}
			catch (Exception e)
			{
				item.Errors = e.ToString();
			}
		}

		/// <summary>
		/// Shows minimal information about all test XML file requests in the queue, and their status,
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
					BranchName = task.BranchName,
					Status = task.CurrentTask.Status.ToString(),
					IsComplete = task.CurrentTask.IsCompleted,
					CurrentIndex = (runner != null) ? task.Runner.TestsRun : 0,
					TotalTests = (runner != null) ? task.Runner.TotalTests : 0,
				};
			});
		}

		/// <summary>
		/// Shows the full information about a *single* test run - it doesn't have to be running, it could be complete.
		/// This includes the results of every test in the test file.
		/// </summary>
		public TaskDetails GetRunningTaskDetails(int taskId)
		{
			TestFileRunnerTaskInfo task;
			_currentTasks.TryGetValue(taskId, out task);
			if (task == null)
			{
				return null;
			}

			TestFileRunner runner = task.Runner;
			return new TaskDetails()
			{
				TaskId = task.Id,
				Username = task.Username,
				BranchName = task.BranchName,
				Status = task.CurrentTask.Status.ToString(),
				Results = (runner != null) ? runner.CurrentResults.ToList() : new List<TestResult>(),
				CurrentIndex = (runner != null) ? runner.TestsRun : 0,
				TotalTests = (runner != null) ? runner.TotalTests : 0,
				Errors = task.Errors
			};
		}

		/// <summary>
		/// Stops a test XML request task in the queue, returning a message of whether the stop succeeded or not.
		/// </summary>
		public string Stop(int taskId)
		{
			TestFileRunnerTaskInfo task;
			_currentTasks.TryRemove(taskId, out task);
			if (task == null)
			{
				return "FAILED - Cancel request made, but removing from the list of tasks failed";
			}


			task.Runner.Stop();
			task.CancelTokenSource.Cancel(false);

			return string.Format("OK - Task {0} stopped and removed", task.Id);
		}

		/// <summary>
		/// Attempts to shut down all running tasks.
		/// </summary>
		public List<string> StopAll()
		{
			List<string> results = new List<string>();
			foreach (TestFileRunnerTaskInfo task in _currentTasks.Values)
			{
				results.Add(Stop(task.Id));
			}

			return results;
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