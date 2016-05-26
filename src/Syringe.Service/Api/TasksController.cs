using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using RestSharp;
using Syringe.Core.Http;
using Syringe.Core.Runner;
using Syringe.Core.Services;
using Syringe.Core.Tasks;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Results;
using Syringe.Service.Parallel;

namespace Syringe.Service.Api
{
	public class TasksController : ApiController, ITasksService
	{
		private readonly ITestFileQueue _fileQueue;

		public TasksController(ITestFileQueue fileQueue)
		{
			_fileQueue = fileQueue;
		}

		/// <summary>
		/// Run a test file synchronously - waiting for the tests to finish.
		/// </summary>
		[Route("api/tasks/RunTestFile")]
		[HttpGet]
		public TestFileRunResult RunTestFile(string filename, string environment, string username)
		{
			DateTime startTime = DateTime.UtcNow;

			var taskRequest = new TaskRequest()
			{
				Environment = environment,
				Filename = filename,
				Username = username
			};

			try
			{
				// Wait 2 minutes for the tests to run, this can be made configurable later
				TimeSpan timeout = TimeSpan.FromMinutes(2);
				Task<TestFileRunnerTaskInfo> task = _fileQueue.RunAsync(taskRequest);
				bool completed = task.Wait(timeout);
				TimeSpan timeTaken = DateTime.UtcNow - startTime;

				if (completed)
				{
					if (!string.IsNullOrEmpty(task.Result.Errors))
					{
						return new TestFileRunResult()
						{
							Completed = false,
							TimeTaken = timeTaken,
							ErrorMessage = task.Result.Errors
						};
					}

					int failCount = task.Result.Runner.CurrentResults.Count(x => !x.Success);

					return new TestFileRunResult()
					{
						Completed = true,
						TimeTaken = timeTaken,
						HasFailedTests = (failCount > 0),
						ErrorMessage = "",
						TestResults = task.Result.Runner.CurrentResults.Select(result => new LightweightResult()
						{
							Success = result.Success,
							Message = result.Message,
							ExceptionMessage = result.ExceptionMessage,
							AssertionsSuccess = result.AssertionsSuccess,
							ScriptCompilationSuccess = result.ScriptCompilationSuccess,
							ResponseTime = result.ResponseTime,
							ResponseCodeSuccess = result.ResponseCodeSuccess,
							ActualUrl = result.ActualUrl,
							TestUrl = result.Test.Url,
							TestDescription = result.Test.Description
						})
					};
				}
				else
				{
					// Error
					return new TestFileRunResult()
					{
						Completed = false,
						TimeTaken = timeTaken,
						ErrorMessage = "The runner timed out."
					};
				}
			}
			catch (Exception ex)
			{
				TimeSpan timeTaken = DateTime.UtcNow - startTime;

				// Error
				return new TestFileRunResult()
				{
					Completed = false,
					TimeTaken = timeTaken,
					ErrorMessage = ex.ToString()
				};
			}
		}

		[Route("api/tasks/Start")]
		[HttpPost]
		public int Start(TaskRequest item)
		{
			return _fileQueue.Add(item);
		}

		[Route("api/tasks/Stop")]
		[HttpGet]
		public string Stop(int id)
		{
			return _fileQueue.Stop(id);
		}

		[Route("api/tasks/StopAll")]
		[HttpGet]
		public List<string> StopAll()
		{
			return _fileQueue.StopAll();
		}

		[Route("api/tasks/GetRunningTasks")]
		[HttpGet]
		public IEnumerable<TaskDetails> GetRunningTasks()
		{
			return _fileQueue.GetRunningTasks();
		}

		[Route("api/tasks/GetRunningTaskDetails")]
		[HttpGet]
		public TaskDetails GetRunningTaskDetails(int taskId)
		{
			return _fileQueue.GetRunningTaskDetails(taskId);
		}
	}
}