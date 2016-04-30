using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Syringe.Core.Http.Logging;
using Syringe.Core.Tests;

namespace Syringe.Core.Http
{
	public class HttpClient : IHttpClient
	{
		private readonly IRestClient _restClient;
		private readonly CookieContainer _cookieContainer;

		static HttpClient()
		{
			// Allow invalid SSL certificates
			ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
		}

		public HttpClient(IRestClient restClient)
		{
			_restClient = restClient;
			_cookieContainer = new CookieContainer();
		}

		public async Task<HttpResponse> ExecuteRequestAsync(IRestRequest request, HttpLogWriter httpLogWriter)
		{
			//
			// Get the response back, parsing the headers
			//
            DateTime startTime = DateTime.UtcNow;
			IRestResponse response = await _restClient.ExecuteTaskAsync(request);
		    TimeSpan responseTime = DateTime.UtcNow - startTime;

			List<KeyValuePair<string, string>> keyvaluePairs = new List<KeyValuePair<string, string>>();
			if (response.Headers != null)
			{ 
				keyvaluePairs = response.Headers.Select(x => new KeyValuePair<string, string>(x.Name, Convert.ToString(x.Value)))
												.ToList();
			}

			// Logging
			httpLogWriter.AppendRequest(_restClient.BaseUrl, request);
			httpLogWriter.AppendResponse(response);

			return new HttpResponse()
			{
				StatusCode = response.StatusCode,
				Content = response.Content,
				Headers = keyvaluePairs,
                ResponseTime = responseTime
			};
		}

		public IRestRequest CreateRestRequest(string httpMethod, string url, string postBody, IEnumerable<HeaderItem> headers)
		{
			Uri uri;
			if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
				throw new ArgumentException(url + " is not a valid Uri", nameof(url));

			_restClient.BaseUrl = uri;
			_restClient.CookieContainer = _cookieContainer;

			//
			// Make the request adding the content-type, body and headers
			//
			Method method = GetMethodEnum(httpMethod);
			var request = new RestRequest(method);
			if (method == Method.POST)
			{
				const string contentType = "application/x-www-form-urlencoded";

				// From the RestSharp docs:
				// "The name of the parameter will be used as the Content-Type header for the request."
				request.AddParameter(contentType, postBody, ParameterType.RequestBody);
			}

			if (headers != null)
			{
				headers = headers.ToList();
				foreach (var keyValuePair in headers)
				{
					request.AddHeader(keyValuePair.Key, keyValuePair.Value);
				}
			}

			return request;
		}

		private Method GetMethodEnum(string httpMethod)
		{
			var method = Method.GET;

			if (!Enum.TryParse(httpMethod, true, out method))
			{
				method = Method.GET;
			}

			return method;
		}
	}
}
