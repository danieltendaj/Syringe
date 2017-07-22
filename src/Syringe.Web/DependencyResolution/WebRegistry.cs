using StructureMap;

namespace Syringe.Web.DependencyResolution
{
	public class WebRegistry : Registry
	{
		public WebRegistry()
		{
			Scan(
				scan =>
				{
					scan.TheCallingAssembly();
					scan.Assembly("Syringe.Core");
					scan.WithDefaultConventions();
				});

			For<Startup>().Use<Startup>().Singleton();
		}
	}
}