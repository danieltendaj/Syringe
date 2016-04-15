using System.Net;
using Newtonsoft.Json;
using RestSharp;

namespace Syringe.Client
{
	public class RestSharpHelper
	{
		private readonly string _requestPath;

		public RestSharpHelper(string requestPath)
		{
			_requestPath = requestPath;
		}

		public T DeserializeOrThrow<T>(IRestResponse response)
		{
			if (response.StatusCode == HttpStatusCode.OK)
			{
				return JsonConvert.DeserializeObject<T>(response.Content);
			}

			if (response.StatusCode == 0)
			{
				throw new ClientException("REST Client error, status code 0 - {0}.", response.ErrorMessage);
			}
			else
			{
				throw new ClientException("REST Client error: status code {0} - {1}", response.StatusCode, response.Content);
			}
		}

		public IRestRequest CreateRequest(string action)
		{
			return new RestRequest($"{_requestPath}/{action}");
		}
	}
}