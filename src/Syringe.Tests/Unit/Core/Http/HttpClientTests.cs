using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using RestSharp;
using Syringe.Core.Http;
using Syringe.Core.Http.Logging;
using Syringe.Core.Tests;
using Syringe.Tests.StubsMocks;
using HttpResponse = Syringe.Core.Http.HttpResponse;

namespace Syringe.Tests.Unit.Core.Http
{
	public class HttpClientTests
	{
		private RestClientMock _restClientMock;

		private HttpLogWriter GetHttpLogWriter()
		{
			return new HttpLogWriter();
		}

		[Test]
		public async Task should_return_expected_html_content()
		{
			// given
			var httpLogWriter = GetHttpLogWriter();
			var restResponse = new RestResponseStub()
			{
				Content = "<html>some text </html>"
			};
			HttpClient httpClient = CreateClient(restResponse);

			string method = "get";
			string url = "http://www.example.com";
			string postBody = "";
			var headers = new List<HeaderItem>();
			var restRequest = httpClient.CreateRestRequest(method, url, postBody, headers);

			// when
			HttpResponse response = await httpClient.ExecuteRequestAsync(restRequest, httpLogWriter);

			// then
			Assert.NotNull(response);
			Assert.AreEqual(restResponse.Content, response.Content);
		}

		[Test]
		public async Task should_ignore_null_headers()
		{
			// given
			var httpLogWriter = GetHttpLogWriter();
			HttpClient httpClient = CreateClient(new RestResponse());

			string method = "get";
			string url = "http://www.example.com";
			string postBody = "";
			var headers = new List<HeaderItem>();
			var restRequest = httpClient.CreateRestRequest(method, url, postBody, headers);

			// when
			HttpResponse response = await httpClient.ExecuteRequestAsync(restRequest, httpLogWriter);

			// then
			Assert.IsNotNull(response);
		}

		[Test]
		public async Task should_add_postbody_and_x_form_to_request_when_method_is_post()
		{
			// given
			var httpLogWriter = GetHttpLogWriter();
			HttpClient httpClient = CreateClient(new RestResponse());

			string method = "post";
			string url = "http://www.example.com";
			string postBody = "keywords=foo&location=london";
			var headers = new List<HeaderItem>();
			var restRequest = httpClient.CreateRestRequest(method, url, postBody, headers);

			// when
			await httpClient.ExecuteRequestAsync(restRequest, httpLogWriter);
			
			// then
			Parameter parameter = _restClientMock.RestRequest.Parameters.First();
			Assert.AreEqual("application/x-www-form-urlencoded", parameter.Name);
			Assert.AreEqual(postBody, parameter.Value);
			Assert.AreEqual(ParameterType.RequestBody, parameter.Type);
		}

		[Test]
		public async Task should_use_httpmethod_get_when_method_is_invalid()
		{
			// given
			var httpLogWriter = GetHttpLogWriter();
			HttpClient httpClient = CreateClient(new RestResponse());

			string method = "snort";
			string url = "http://www.example.com";
			string postBody = "";
			var headers = new List<HeaderItem>();
			var restRequest = httpClient.CreateRestRequest(method, url, postBody, headers);

			// when
			await httpClient.ExecuteRequestAsync(restRequest, httpLogWriter);

			// then
			Assert.AreEqual(Method.GET, _restClientMock.RestRequest.Method);
		}

		[Test]
		public async Task should_add_headers()
		{
			// given
			var httpLogWriter = GetHttpLogWriter();
			HttpClient httpClient = CreateClient(new RestResponse());

			string method = "get";
			string url = "http://www.example.com";
			string postBody = "";
			var headers = new List<HeaderItem>()
			{
				new HeaderItem("user-agent", "Netscape Navigator 1"),
				new HeaderItem("cookies", "mmm cookies"),
			};
			var restRequest = httpClient.CreateRestRequest(method, url, postBody, headers);

			// when
			await httpClient.ExecuteRequestAsync(restRequest, httpLogWriter);

			// then
			var parameters = _restClientMock.RestRequest.Parameters;
			var userAgent = parameters.First(x => x.Name == "user-agent");
			var cookies = parameters.First(x => x.Name == "cookies");

			Assert.AreEqual(2, parameters.Count);
			Assert.AreEqual("Netscape Navigator 1", userAgent.Value);
			Assert.AreEqual("mmm cookies", cookies.Value);
		}

		[Test]
		public async Task should_fill_response_properties()
		{
			// given
			var httpLogWriter = GetHttpLogWriter();
			var restResponseStub = new RestResponseStub();
			restResponseStub.Content = "HTTP/1.1 200 OK\nServer: Apache\n\n<html>some text </html>";
			restResponseStub.StatusCode = HttpStatusCode.Accepted;
			restResponseStub.Headers = new Parameter[] { new Parameter(), new Parameter() };

			HttpClient httpClient = CreateClient(restResponseStub);

			string method = "get";
			string url = "http://www.example.com";
			string postBody = "";
			var headers = new List<HeaderItem>();
			var restRequest = httpClient.CreateRestRequest(method, url, postBody, headers);

			// when
			HttpResponse response = await httpClient.ExecuteRequestAsync(restRequest, httpLogWriter);

			// then
			Assert.AreEqual(restResponseStub.StatusCode, response.StatusCode);
			Assert.AreEqual(restResponseStub.Content, response.Content);
			Assert.AreEqual(restResponseStub.Headers.Count, response.Headers.Count);
		}

		[Test]
		public async Task should_record_response_times()
		{
			// given
			var httpLogWriter = GetHttpLogWriter();
			HttpClient httpClient = CreateClient(new RestResponse());
			_restClientMock.ResponseTime = TimeSpan.FromSeconds(1);

			string method = "get";
			string url = "http://www.example.com";
			string postBody = "";
			var headers = new List<HeaderItem>();
			var restRequest = httpClient.CreateRestRequest(method, url, postBody, headers);

			// when
			HttpResponse response = await httpClient.ExecuteRequestAsync(restRequest, httpLogWriter);

			// then
			Assert.That(response.ResponseTime, Is.GreaterThanOrEqualTo(TimeSpan.FromSeconds(1)));
		}

		private HttpClient CreateClient(IRestResponse restResponse)
		{
			_restClientMock = new RestClientMock();
			_restClientMock.RestResponse = restResponse;
			return new HttpClient(_restClientMock);
		}
	}
}