using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Web.Http.Dependencies;
using System.Web.Mvc;
using Microsoft.Owin.Hosting;
using Syringe.Core.Configuration;
using Syringe.Web;
using Syringe.Web.DependencyResolution;

namespace Syringe.Tests.Acceptance
{
	public class WebConfig
	{
		private static string _baseUrl;
		public static int Port { get; set; }
		public static IDisposable OwinServer;

		public static string BaseUrl
		{
			get
			{
				if (string.IsNullOrEmpty(_baseUrl))
				{
					// Find a free port. Using port 0 gives you the next free port.
					if (Port == 0)
					{
						var listener = new TcpListener(IPAddress.Loopback, 0);
						listener.Start();
						Port = ((IPEndPoint) listener.LocalEndpoint).Port;
						listener.Stop();
					}

					_baseUrl = $"http://localhost:{Port}";
				}

				return _baseUrl;
			}
		}

		public static void StartSelfHostedOwin(string serviceUrl)
		{
			MvcConfiguration.Configuration = new MvcConfiguration() { ServiceUrl = serviceUrl };
			IoC.Initialize();

			var startup = new Startup();

			// Start it up
			OwinServer = WebApp.Start(BaseUrl, startup.Configuration);
		}
	}
}