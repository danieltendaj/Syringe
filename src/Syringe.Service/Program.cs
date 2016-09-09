using Syringe.Service.DependencyResolution;
using Topshelf;

namespace Syringe.Service
{
	/// <summary>
	/// Influence from https://msdn.microsoft.com/en-us/magazine/dn745865.aspx
	/// </summary>
	internal class Program
	{
		public static int Main(string[] args)
		{
			// .\Syringe.Service.exe [verb] [-option:value] [-switch]
			// run Runs the service from the command line (default)
			// help or –help Displays help
			// install Installs the service

		    var container = IoC.Initialize();

			TopshelfExitCode exitCode = HostFactory.Run(host =>
			{
				host.ApplyCommandLine();

				host.Service<Startup>(service =>
				{
					service.ConstructUsing(() => container.GetInstance<Startup>());
					service.WhenStarted(x => x.Start());
					service.WhenStopped(x => x.Stop());
				});

				host.SetServiceName("Syringe");
				host.SetDisplayName("Syringe");
				host.SetDescription("Syringe RESTful API service");
			});
			return (int) exitCode;
		}
	}
}
