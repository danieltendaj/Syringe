using System;
using System.Collections.Generic;
using System.Net;
using NUnit.Framework;
using Syringe.Core.Http;

namespace Syringe.Tests.Unit.Core.Http
{
	public class HttpResponseTests
	{
		[Test]
		public void should_create_headers_in_ctor()
		{
			// Arrange
			var response = new HttpResponse();

			// Act + Assert
			Assert.NotNull(response.Headers);
			Assert.NotNull(response.ResponseTime);
		}

		[Test]
		public void ToString_should_append_headers_and_response_body_and_empty_line()
		{
			// Arrange
			var response = new HttpResponse();
			response.Headers = new List<KeyValuePair<string, string>>()
			{
				new KeyValuePair<string, string>("Server", "Apache"),
				new KeyValuePair<string, string>("Cache-Control", "private, s-maxage=0, max-age=0, must-revalidate"),
				new KeyValuePair<string, string>("Date", "Sun, 12 Apr 2015 19:18:21 GMT"),
				new KeyValuePair<string, string>("Content-Type", "text/html; charset=UTF-8")
			};

			response.Content = "<html><body></body></html>";
			response.StatusCode = HttpStatusCode.OK;

			// Act	
			string content = response.ToString();

			// Assert
			string[] lines = content.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

			Assert.AreEqual("HTTP/1.1 200 OK", lines[0]);
			Assert.AreEqual("Server: Apache", lines[1]);
			Assert.AreEqual("Cache-Control: private, s-maxage=0, max-age=0, must-revalidate", lines[2]);
			Assert.AreEqual("Date: Sun, 12 Apr 2015 19:18:21 GMT", lines[3]);
			Assert.AreEqual("Content-Type: text/html; charset=UTF-8", lines[4]);
			Assert.AreEqual("", lines[5]);
			Assert.AreEqual("<html><body></body></html>", lines[6]);
		}
	}
}