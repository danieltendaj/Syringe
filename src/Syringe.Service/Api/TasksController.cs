using System.Collections.Generic;
using System.Web.Http;
using Syringe.Core.Services;
using Syringe.Core.Tasks;
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

		[Route("api/tasks/RunTestFileAndWait")]
		[HttpGet]
		public string RunTestFileAndWait(string filename, string environment)
		{
			string status = ((ParallelTestFileQueue) _fileQueue).RunTestFile(filename, environment);
			return status;
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