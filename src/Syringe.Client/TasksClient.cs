using System.Collections.Generic;
using Syringe.Client.Http;
using Syringe.Core.Services;
using Syringe.Core.Tasks;

namespace Syringe.Client
{
	public class TasksClient : ITasksService
	{
		internal readonly string ServiceUrl;
		private readonly FlurlWrapper _wrapper;

		public TasksClient(string serviceUrl)
		{
			ServiceUrl = serviceUrl;
			var factory = new CustomHttpClientFactory(serviceUrl);
			_wrapper = new FlurlWrapper(factory, "/api/tasks");
		}

		public int Start(TaskRequest item)
		{
			return _wrapper.Post<int>("", item).Result;
		}

		public IEnumerable<TaskDetails> GetTasks()
		{
			return _wrapper.Get<List<TaskDetails>>("systemvariables").Result;
		}

		public TaskDetails GetTask(int taskId)
		{
			_wrapper.AddParameter("taskId", taskId.ToString());
			return _wrapper.Get<TaskDetails>("").Result;
		}

		public int StartBatch(string[] fileNames, string environment, string username)
		{
			throw new System.NotImplementedException();
		}
	}
}