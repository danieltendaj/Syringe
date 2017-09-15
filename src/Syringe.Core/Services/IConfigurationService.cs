using System.Collections.Generic;
using Syringe.Core.Configuration;
using Syringe.Core.Tests.Scripting;
using Syringe.Core.Tests.Variables;

namespace Syringe.Core.Services
{
	public interface IConfigurationService
	{
		Settings GetSettings();

		IEnumerable<IVariable> GetSystemVariables();

		IEnumerable<string> GetScriptSnippetFilenames(ScriptSnippetType snippetType);
	}
}