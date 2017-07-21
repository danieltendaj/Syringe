using System;
using System.Net;
using System.Net.Http;
using Syringe.Core.Exceptions;

namespace Syringe.Core.Configuration
{
	public class HealthCheck : IHealthCheck
	{
		internal readonly string ServiceUrl;
		private HttpClient _client;

		public HealthCheck(string serviceUrl)
		{
			ServiceUrl = serviceUrl;
			_client = new HttpClient();
		}

		public async void CheckServiceConfiguration()
		{
			_client.BaseAddress = new Uri(ServiceUrl);

			var request = new HttpRequestMessage(HttpMethod.Get, "/api/healthcheck/CheckConfiguration");
			var response = await _client.SendAsync(request);

			if (response.StatusCode != HttpStatusCode.OK)
				throw new HealthCheckException("The REST service at {0} did not return a 200 OK. Is the service running?", request.RequestUri);

			string content = await response.Content.ReadAsStringAsync();
			if (!content.Contains("Everything is OK"))
				throw new HealthCheckException("The REST service at {0} configuration check failed: \n{1}", request.RequestUri, response.Content);
		}

		public async void CheckServiceSwaggerIsRunning()
		{
			_client.BaseAddress = new Uri(ServiceUrl);

			var request = new HttpRequestMessage(HttpMethod.Get, "/swagger/ui/index");
			var response = await _client.SendAsync(request);

			if (response.StatusCode != HttpStatusCode.OK)
				throw new HealthCheckException("The REST service at {0} did not return a 200 OK. Is the service running?", request.RequestUri);

			string content = await response.Content.ReadAsStringAsync();
			if (!content.Contains("Syringe REST API"))
				throw new HealthCheckException("The REST service at {0} did not return content with 'Syringe REST API' in the body.", request.RequestUri);
		}
	}
}