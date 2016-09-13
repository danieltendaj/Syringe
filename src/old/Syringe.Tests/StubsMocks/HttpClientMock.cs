using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestSharp;
using Syringe.Core.Http;
using Syringe.Core.Http.Logging;
using Syringe.Core.Tests;
using HttpResponse = Syringe.Core.Http.HttpResponse;

namespace Syringe.Tests.StubsMocks
{
	public class HttpClientMock : IHttpClient
	{
		private int _responseCounter;
		public bool LogLastRequestCalled { get; set; }
		public bool LogLastResponseCalled { get; set; }
		public HttpResponse Response { get; set; }
		public List<TimeSpan> ResponseTimes { get; set; }
		public List<HttpResponse> Responses { get; set; }

		public IRestRequest RestRequest { get; set; }

		public HttpClientMock(HttpResponse response)
		{
			Response = response;
		}

		public IRestRequest CreateRestRequest(string httpMethod, string url, string postBody, IEnumerable<HeaderItem> headers)
		{
			return RestRequest;
		}

		public Task<HttpResponse> ExecuteRequestAsync(IRestRequest request, HttpLogWriter httpLogWriter)
		{
			if (Responses == null)
			{
				if (ResponseTimes != null)
				{
					Response.ResponseTime = ResponseTimes[_responseCounter++];
				}

				return Task.FromResult(Response);
			}

			return Task.FromResult(Responses[_responseCounter++]);
		}
	}
}