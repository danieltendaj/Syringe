using System.Collections.Generic;
using Syringe.Core.Configuration;
using Syringe.Core.Environment;

namespace Syringe.Tests.StubsMocks
{
	public class ConfigurationStoreMock : IConfigurationStore
	{
		public IConfiguration Configuration { get; set; }

		public ConfigurationStoreMock()
		{
			Configuration = new JsonConfiguration()
			{
				Environments = new List<Environment>(),
				Settings = new Settings()
			};
		}

		public IConfiguration Load()
		{
			return Configuration;
		}

		public string ResolveConfigFile(string fileName)
		{
			throw new System.NotImplementedException();
		}
	}
}