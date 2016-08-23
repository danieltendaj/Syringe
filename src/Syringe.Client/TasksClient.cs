using System.Collections.Generic;
using Newtonsoft.Json;
using RestSharp;
using Syringe.Core.Services;
using Syringe.Core.Tasks;

namespace Syringe.Client
{
	public class TasksClient : ITasksService
	{
		internal readonly string ServiceUrl;

		public TasksClient(string serviceUrl)
		{
			ServiceUrl = serviceUrl;
		}

		public int Start(TaskRequest item)
		{
			var client = new RestClient(ServiceUrl);
			IRestRequest request = CreateRequest();
			request.AddJsonBody(item);
			request.Method = Method.POST;

			IRestResponse response = client.Execute(request);
			return ParseOrDefault(response.Content, 0);
		}
        
		public IEnumerable<TaskDetails> GetTasks()
        {
            var client = new RestClient(ServiceUrl);
            IRestRequest request = new RestRequest("/api/tasks/");
            
            var response = client.Execute<List<TaskDetails>>(request);
            return response.Data;
        }

		public TaskDetails GetTask(int taskId)
		{
			var client = new RestClient(ServiceUrl);
			IRestRequest request = CreateRequest();
			request.AddParameter("taskId", taskId);

			// Don't use the Restsharp JSON deserializer, it fails
			IRestResponse response = client.Execute(request);
			TaskDetails details = JsonConvert.DeserializeObject<TaskDetails>(response.Content);

			return details;
		}

		private IRestRequest CreateRequest(string action = "")
		{
			return new RestRequest($"/api/task/{action}");
		}

		public static int ParseOrDefault(string value, int defaultValue)
		{
			int result;
			if (int.TryParse(value, out result))
				return result;

			return defaultValue;
		}
	}
}