using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using NUnit.Framework;
using RestSharp;
using Syringe.Core.Tests;
using Syringe.Tests.Integration.ClientAndService;
using Syringe.Web;

namespace Syringe.Tests.Acceptance
{
	public class AcceptanceTests
	{
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			ServiceConfig.Port = 6768;
			ServiceConfig.StartSelfHostedOwin();

			WebConfig.Port = 6800;
			WebConfig.StartSelfHostedOwin(serviceUrl: "http://localhost:6768");
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			ServiceConfig.OwinServer.Dispose();
			WebConfig.OwinServer.Dispose();
		}

		[Test]
		public void quick_test()
		{
			// given
			var restClient = new RestClient("http://localhost:6800");
			var restRequest = new RestRequest("/");
			IRestResponse response = restClient.ExecuteAsGet(restRequest, "GET");

			// when
			string html = response.Content;

			// then
			Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
			Assert.That(html, Is.StringContaining("Test Files"));
		}
	}
}
