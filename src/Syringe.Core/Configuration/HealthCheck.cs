using System.Net;
using RestSharp;
using Syringe.Core.Exceptions;

namespace Syringe.Core.Configuration
{
	public class HealthCheck : IHealthCheck
	{
		internal readonly string ServiceUrl;

		public HealthCheck(string serviceUrl)
		{
			ServiceUrl = serviceUrl;
		}

		public void CheckServiceConfiguration()
		{
			var client = new RestClient(ServiceUrl);
			var request = new RestRequest("/api/healthcheck/CheckConfiguration");

			IRestResponse response = client.Execute(request);

			if (response.StatusCode != HttpStatusCode.OK)
				throw new HealthCheckException("The REST service at {0} did not return a 200 OK. Is the service running?", response.ResponseUri);

			if (!response.Content.Contains("Everything is OK"))
				throw new HealthCheckException("The REST service at {0} configuration check failed: \n{1}", response.ResponseUri, response.Content);
		}

		public void CheckServiceSwaggerIsRunning()
		{
			var client = new RestClient(ServiceUrl);
			var request = new RestRequest("/swagger/ui/index");

			IRestResponse response = client.Execute(request);

			if (response.Content.Contains("Syringe REST API"))
				throw new HealthCheckException("The REST service at {0} did not return content with 'Syringe REST API' in the body.", response.ResponseUri);
		}
	}
}
