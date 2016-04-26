using System;
using System.Collections.Generic;
using System.Net;
using NUnit.Framework;
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
		public void WriteRequest_should_ignore_null_method_parameter()
		{
			// Arrange
			var logWriter = GetHttpLogWriter();
			var stringBuilder = logWriter.StringBuilder;
			var requestDetails = new RequestDetails()
			{
				Method = null,
				Url = "url",
				Headers = new List<HeaderItem>()
			};

			// Act
			logWriter.AppendRequest(requestDetails);

			// Assert
			Assert.AreEqual("", stringBuilder.ToString());
		}

		[Test]
		public void WriteRequest_should_ignore_null_url_parameter()
		{
			// Arrange
			var logWriter = GetHttpLogWriter();
			var stringBuilder = logWriter.StringBuilder;
			var requestDetails = new RequestDetails()
			{
				Method = "get",
				Url = null,
				Headers = new List<HeaderItem>()
			};

			// Act
			logWriter.AppendRequest(requestDetails);

			// Assert
			Assert.AreEqual("", stringBuilder.ToString());
		}

		[Test]
		public void WriteRequest_should_ignore_invalid_url_parameter()
		{
			// Arrange
			var logWriter = GetHttpLogWriter();
			var stringBuilder = logWriter.StringBuilder;
			var requestDetails = new RequestDetails()
			{
				Method = null,
				Url = "not a valid url",
				Headers = new List<HeaderItem>()
			};

			// Act
			logWriter.AppendRequest(requestDetails);

			// Assert
			Assert.AreEqual("", stringBuilder.ToString());
		}

		[Test]
		public void WriteRequest_should_allow_null_for_headers()
		{
			// Arrange
			var logWriter = GetHttpLogWriter();
			var stringBuilder = logWriter.StringBuilder;
			var requestDetails = new RequestDetails()
			{
				Method = null,
				Url = "http://www.uri",
				Headers = null
			};

			// Act
			logWriter.AppendRequest(requestDetails);

			// Assert
			Assert.AreEqual("", stringBuilder.ToString());
		}

		[Test]
		public void WriteRequest_should_write_request_line_and_host_and_extra_newline_at_end()
		{
			// Arrange
			var logWriter = GetHttpLogWriter();
			var stringBuilder = logWriter.StringBuilder;
			var requestDetails = new RequestDetails()
			{
				Method = "post",
				Url = "http://en.wikipedia.org/wiki/Microsoft?a=b",
				Headers = new List<HeaderItem>()
			};

			// Act
			logWriter.AppendRequest(requestDetails);

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
			var headers = new List<HeaderItem>()
			{
				new HeaderItem("Cookie", "aaa=bbb;loggedin=true"),
				new HeaderItem("Accept-Language", "en-US"),
				new HeaderItem("Accept", "text/html")
			};

			var requestDetails = new RequestDetails()
			{
				Method = "post",
				Url = "http://en.wikipedia.org/wiki/Microsoft?a=b",
				Headers = headers
			};

			// Act	
			logWriter.AppendRequest(requestDetails);

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
			var responseDetails = new ResponseDetails()
			{
				Status = HttpStatusCode.NotFound,
				BodyResponse = "",
				Headers = new List<KeyValuePair<string, string>>()
			};

			// Act	
			logWriter.AppendResponse(responseDetails);

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
			var headers = new List<KeyValuePair<string, string>>()
			{
				new KeyValuePair<string, string>("Server", "Apache"),
				new KeyValuePair<string, string>("Cache-Control", "private, s-maxage=0, max-age=0, must-revalidate"),
				new KeyValuePair<string, string>("Date", "Sun, 12 Apr 2015 19:18:21 GMT"),
				new KeyValuePair<string, string>("Content-Type", "text/html; charset=UTF-8")
			};

			var responseDetails = new ResponseDetails()
			{
				Status = HttpStatusCode.OK,
				BodyResponse = "<html><body></body></html>",
				Headers = headers
			};

			// Act	
			logWriter.AppendResponse(responseDetails);

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
		public void WriteResponse_should_ignore_null_headers_and_empty_body()
		{
			// Arrange
			var logWriter = GetHttpLogWriter();
			var stringBuilder = logWriter.StringBuilder;
			var responseDetails = new ResponseDetails()
			{
				Status = HttpStatusCode.OK,
				BodyResponse = "",
				Headers = null
			};

			// Act	
			logWriter.AppendResponse(responseDetails);

			// Assert
			string[] lines = stringBuilder.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

			Assert.AreEqual("HTTP/1.1 200 OK", lines[0]);
			Assert.AreEqual("", lines[1]);
		}
	}
}
