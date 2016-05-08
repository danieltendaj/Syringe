using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Web.Http.Dependencies;
using Microsoft.Owin.Hosting;
using Syringe.Core.Configuration;
using Syringe.Service;
using Syringe.Service.DependencyResolution;

namespace Syringe.Tests.Integration.ClientAndService
{
	public class ServiceStarter
	{
		private static string _baseUrl;
		private static string _xmlDirectoryPath;

		public static string MongodbDatabaseName => "Syringe-Tests";
		public static IDisposable OwinServer;

		public static int Port { get; set; }

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

		/// <summary>
		/// The full path, e.g. ...\bin\debug\integration\xml
		/// </summary>
		public static string XmlDirectoryPath
		{
			get
			{
				if (string.IsNullOrEmpty(_xmlDirectoryPath))
				{
					_xmlDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Integration", "Xml");
				}

				return _xmlDirectoryPath;
			}
		}

		public static void StartSelfHostedOwin()
		{
			string xmlFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Integration", "Xml");

			var jsonConfiguration = new JsonConfiguration()
			{
				MongoDbDatabaseName = MongodbDatabaseName,
				TestFilesBaseDirectory = xmlFolder,
				ServiceUrl = BaseUrl
			};

			// Use the service's IoC container
			var container = IoC.Initialize();
			container.Configure(x => x.For<IConfiguration>().Use(jsonConfiguration));

			// Inject instances into it
			var service = new Startup(container.GetInstance<IDependencyResolver>(), jsonConfiguration, container.GetInstance<ITestFileQueue>(), container.GetInstance<Microsoft.AspNet.SignalR.IDependencyResolver>());

			// Start it up
			OwinServer = WebApp.Start(BaseUrl, service.Configuration);
		}

		public static void RecreateXmlDirectory()
		{
			Console.WriteLine("Deleting and creating {0}", ServiceStarter.XmlDirectoryPath);

			if (Directory.Exists(XmlDirectoryPath))
				Directory.Delete(XmlDirectoryPath, true);
			
			Directory.CreateDirectory(XmlDirectoryPath);
		}
	}
}