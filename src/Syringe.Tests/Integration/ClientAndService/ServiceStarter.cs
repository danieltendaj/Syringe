using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Web.Http.Dependencies;
using Microsoft.Owin.Hosting;
using StructureMap;
using Syringe.Core.Configuration;
using Syringe.Service;
using Syringe.Service.DependencyResolution;

namespace Syringe.Tests.Integration.ClientAndService
{
	public class ServiceStarter
	{
		private static string _baseUrl;
		private static string _testFilesDirectoryPath;

		public static string MongodbDatabaseName => "Syringe-Tests";
		public static IDisposable OwinServer;
        public static IContainer Container;

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
		/// The full path, e.g. ...\bin\debug\integration\testfiles
		/// </summary>
		public static string TestFilesDirectoryPath
		{
			get
			{
				if (string.IsNullOrEmpty(_testFilesDirectoryPath))
				{
					_testFilesDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Integration", "TestFiles");
				}

				return _testFilesDirectoryPath;
			}
		}

		public static void StartSelfHostedOwin()
		{
            var jsonConfiguration = new JsonConfiguration()
			{
				MongoDbDatabaseName = MongodbDatabaseName,
				TestFilesBaseDirectory = TestFilesDirectoryPath,
				ServiceUrl = BaseUrl
			};

		    StopSelfHostedOwin();

            // Use the service's IoC container
            Container = IoC.Initialize();
			Container.Configure(x => x.For<IConfiguration>().Use(jsonConfiguration));

			// Inject instances into it
			var service = new Startup(Container.GetInstance<IDependencyResolver>(), jsonConfiguration, Container.GetInstance<ITestFileQueue>(), Container.GetInstance<Microsoft.AspNet.SignalR.IDependencyResolver>());

			// Start it up
			OwinServer = WebApp.Start(BaseUrl, service.Configuration);
		}

	    public static void StopSelfHostedOwin()
	    {
	        if (OwinServer != null)
	        {
	            OwinServer.Dispose();
	            OwinServer = null;
	        }

	        if (Container != null)
	        {
	            Container.Dispose();
	            Container = null;
	        }
	    }

		public static void RecreateTestFileDirectory()
		{
			Console.WriteLine("Deleting and creating {0}", TestFilesDirectoryPath);

		    if (Directory.Exists(TestFilesDirectoryPath))
		    {
		        Directory.Delete(TestFilesDirectoryPath, true);
		    }
			
			Directory.CreateDirectory(TestFilesDirectoryPath);
		}
	}
}