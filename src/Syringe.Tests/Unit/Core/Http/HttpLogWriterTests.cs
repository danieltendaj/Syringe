using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Policy;
using NUnit.Framework;
using RestSharp;
using Syringe.Core.Http.Logging;
using Syringe.Core.Tests;

namespace Syringe.Tests.Unit.Core.Http
{
	public class HttpLogWriterTests
	{
		private HttpLogWriter GetHttpLogWriter()
		{
			var logWriter = new HttpLogWriter();
			return logWriter;
		}

		[Test]
		public void WriteRequest_should_write_request_line_and_host_and_extra_newline_at_end()
		{
			// Arrange
			var logWriter = GetHttpLogWriter();
			var stringBuilder = logWriter.StringBuilder;
			var request = new RestRequest();
			request.Method = Method.POST;

			var uri = new Uri("http://en.wikipedia.org/wiki/Microsoft?a=b");

			// Act
			logWriter.AppendRequest(uri, request);

			// Assert
			string[] lines = stringBuilder.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

			Assert.AreEqual("POST http://en.wikipedia.org/wiki/Microsoft?a=b HTTP/1.1", lines[0]);
			Assert.AreEqual("Host: en.wikipedia.org", lines[1]);
			Assert.AreEqual("", lines[2]);
		}

		[Test]
		public void WriteRequest_should_append_headers_after_host()
		{
			// Arrange
			var logWriter = GetHttpLogWriter();
			var stringBuilder = logWriter.StringBuilder;
			var request = new RestRequest();
			request.Method = Method.POST;
			request.AddHeader("Cookie", "aaa=bbb;loggedin=true");
			request.AddHeader("Accept-Language", "en-US");
			request.AddHeader("Accept", "text/html");

			var uri = new Uri("http://en.wikipedia.org/wiki/Microsoft?a=b");

			// Act	
			logWriter.AppendRequest(uri, request);

			// Assert
			string[] lines = stringBuilder.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

			Assert.AreEqual("POST http://en.wikipedia.org/wiki/Microsoft?a=b HTTP/1.1", lines[0]);
			Assert.AreEqual("Host: en.wikipedia.org", lines[1]);
			Assert.AreEqual("Cookie: aaa=bbb;loggedin=true", lines[2]);
			Assert.AreEqual("Accept-Language: en-US", lines[3]);
			Assert.AreEqual("Accept: text/html", lines[4]);
			Assert.AreEqual("", lines[5]);
		}

		[Test]
		public void WriteResponse_should_write_status_code_and_status_description_and_empty_line()
		{
			// Arrange
			var logWriter = GetHttpLogWriter();
			var stringBuilder = logWriter.StringBuilder;
			var response = new RestResponse()
			{
				StatusCode = HttpStatusCode.NotFound
			};

			// Act	
			logWriter.AppendResponse(response);

			// Assert
			string[] lines = stringBuilder.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

			Assert.AreEqual("HTTP/1.1 404 Not Found", lines[0]);
			Assert.AreEqual("", lines[1]);
		}

		[Test]
		public void WriteResponse_should_append_headers_and_response_body_and_empty_line()
		{
			// Arrange
			var logWriter = GetHttpLogWriter();
			var stringBuilder = logWriter.StringBuilder;
			var response = new RestResponse()
			{
				StatusCode = HttpStatusCode.OK,
				Content = "<html><body></body></html>"
			};

			response.Headers.Add(new Parameter() { Name = "Server", Value = "Apache" });
			response.Headers.Add(new Parameter() { Name = "Cache-Control", Value = "private, s-maxage=0, max-age=0, must-revalidate" });
			response.Headers.Add(new Parameter() { Name = "Date", Value = "Sun, 12 Apr 2015 19:18:21 GMT" });
			response.Headers.Add(new Parameter() { Name = "Content-Type", Value = "text/html; charset=UTF-8" });

			// Act	
			logWriter.AppendResponse(response);

			// Assert
			string[] lines = stringBuilder.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

			Assert.AreEqual("HTTP/1.1 200 OK", lines[0]);
			Assert.AreEqual("Server: Apache", lines[1]);
			Assert.AreEqual("Cache-Control: private, s-maxage=0, max-age=0, must-revalidate", lines[2]);
			Assert.AreEqual("Date: Sun, 12 Apr 2015 19:18:21 GMT", lines[3]);
			Assert.AreEqual("Content-Type: text/html; charset=UTF-8", lines[4]);
			Assert.AreEqual("", lines[5]);
			Assert.AreEqual("<html><body></body></html>", lines[6]);
		}

		[Test]
		public void WriteResponse_should_ignore_empty_body()
		{
			// Arrange
			var logWriter = GetHttpLogWriter();
			var stringBuilder = logWriter.StringBuilder;
			var response = new RestResponse()
			{
				StatusCode = HttpStatusCode.OK,
				Content = ""
			};

			// Act	
			logWriter.AppendResponse(response);

			// Assert
			string[] lines = stringBuilder.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

			Assert.AreEqual("HTTP/1.1 200 OK", lines[0]);
			Assert.AreEqual("", lines[1]);
		}
	}
}
