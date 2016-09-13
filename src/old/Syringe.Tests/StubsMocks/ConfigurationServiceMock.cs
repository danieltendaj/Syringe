using System.Collections.Generic;
using Syringe.Core.Configuration;
using Syringe.Core.Services;
using Syringe.Core.Tests.Scripting;
using Syringe.Core.Tests.Variables;

namespace Syringe.Tests.StubsMocks
{
	public class ConfigurationServiceMock : IConfigurationService
	{
		public IConfiguration Configuration { get; set; }
		public List<Variable> SystemVariables { get; set; }
		public List<string> SnippetFilenames { get; set; }

		public ConfigurationServiceMock()
		{
			Configuration = new JsonConfiguration();
			SystemVariables = new List<Variable>();
			SnippetFilenames = new List<string>();
		}

		public IConfiguration GetConfiguration()
		{
			return Configuration;
		}

		public IEnumerable<IVariable> GetSystemVariables()
		{
			return SystemVariables;
		}

		public IEnumerable<string> GetScriptSnippetFilenames(ScriptSnippetType snippetType)
		{
			return SnippetFilenames;
		}
	}
}