using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Web.Http.Dependencies;
using Microsoft.Owin.Hosting;
using StructureMap;
using Syringe.Core.Configuration;
using Syringe.Core.Tests.Scripting;
using Syringe.Service;
using Syringe.Service.DependencyResolution;
using Syringe.Service.Parallel;

namespace Syringe.Tests.Integration.ClientAndService
{
	public class ServiceStarter
	{
		private static string _baseUrl;
		private static string _testFilesDirectoryPath;

		public static IDisposable OwinServer;
        public static IContainer Container;
		private static string _scriptSnippetDirectoryPath;

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
					string baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Integration", "ClientAndService");
					_testFilesDirectoryPath = Path.Combine(baseDirectory, "TestFiles");
				}

				return _testFilesDirectoryPath;
			}
		}

		public static string ScriptSnippetDirectoryPath
		{
			get
			{
				if (string.IsNullOrEmpty(_scriptSnippetDirectoryPath))
				{
					_scriptSnippetDirectoryPath = Path.Combine(TestFilesDirectoryPath, "ScriptSnippets");
				}

				return _scriptSnippetDirectoryPath;
			}
		}

		public static void StartSelfHostedOwin()
		{
            var jsonConfiguration = new JsonConfiguration()
			{
				TestFilesBaseDirectory = TestFilesDirectoryPath,
				ScriptSnippetDirectory = ScriptSnippetDirectoryPath,
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

		public static void RecreateScriptSnippetDirectories()
		{
			// If there are more enum values in the future, this can iterate them.
			string snippetDirectory = Path.Combine(ScriptSnippetDirectoryPath, ScriptSnippetType.BeforeExecute.ToString());

			Console.WriteLine("Deleting and creating {0}", snippetDirectory);

			if (Directory.Exists(snippetDirectory))
			{
				Directory.Delete(snippetDirectory, true);
			}

			Directory.CreateDirectory(snippetDirectory);
			File.WriteAllText(Path.Combine(snippetDirectory, "snippet1.snippet"), "snippet1");
			File.WriteAllText(Path.Combine(snippetDirectory, "snippet2.snippet"), "snippet2");
		}
	}
}