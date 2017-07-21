using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Syringe.Core.Http.Logging;
using Syringe.Core.Tests;

namespace Syringe.Core.Http
{
	public class HttpClientAdapter : IHttpClientAdapter
	{
		private readonly HttpClient _httpClient;
		private readonly CookieContainer _cookieContainer;

		public HttpClientAdapter(HttpClient httpClient)
		{
			_httpClient = httpClient;
			_cookieContainer = new CookieContainer();
		}

		public async Task<HttpResponse> SendAsync(HttpLogWriter httpLogWriter, string httpMethod, string url, string postBody, IEnumerable<HeaderItem> headers)
		{
			// Construct the request
			var method = new HttpMethod(httpMethod.ToUpper());
			var uri = new Uri(url);
			var httpRequestMessage = new HttpRequestMessage(method, uri);

			if (method == HttpMethod.Post)
			{
				httpRequestMessage.Content = new StringContent(postBody, Encoding.UTF8, "application/x-www-form-urlencoded");
			}

			if (headers != null)
			{
				headers = headers.ToList();
				foreach (var keyValuePair in headers)
				{
					httpRequestMessage.Headers.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}

			// Send the request
			DateTime startTime = DateTime.UtcNow;
			HttpResponseMessage httpResponse = await _httpClient.SendAsync(httpRequestMessage);
			TimeSpan responseTime = DateTime.UtcNow - startTime;

			// Craft a lighter weight response
			var syringeHttpHeaders = new List<HttpHeader>();
			if (httpResponse.Headers != null)
			{
				syringeHttpHeaders = httpResponse.Headers.Select(x => new HttpHeader(x.Key, string.Join(",", x.Value))).ToList();
			}

			var syringeResponse = new HttpResponse()
			{
				Content = await httpResponse.Content.ReadAsStringAsync(),
				Headers = syringeHttpHeaders,
				ResponseTime = responseTime,
				StatusCode = httpResponse.StatusCode
			};

			// Logging
			httpLogWriter.AppendRequest(_httpClient.BaseAddress, httpRequestMessage);
			httpLogWriter.AppendResponse(syringeResponse);

			return syringeResponse;
		}
	}
}