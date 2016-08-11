using Syringe.Core.Configuration;

namespace Syringe.Tests.StubsMocks
{
	public class ConfigurationStoreMock : IConfigurationStore
	{
		public IConfiguration Configuration { get; set; }

		public ConfigurationStoreMock()
		{
			Configuration = new JsonConfiguration();
		}

		public IConfiguration Load()
		{
			return Configuration;
		}
	}
}