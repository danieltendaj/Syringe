using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Syringe.Core.Configuration;
using Syringe.Core.Services;
using Syringe.Core.Tests.Scripting;
using Syringe.Core.Tests.Variables;
using Syringe.Core.Tests.Variables.ReservedVariables;
using Syringe.Core.Tests.Variables.SharedVariables;

namespace Syringe.Service.Controllers
{
	[Route("api/[controller]")]
	public class ConfigurationController : Controller, IConfigurationService
	{
		private readonly Settings _settings;
		private readonly ISharedVariablesProvider _sharedVariablesProvider;
		private readonly IReservedVariableProvider _reservedVariableProvider;
		private readonly ISnippetFileReader _snippetFileReader;

		public ConfigurationController(IOptions<Settings> settings, ISharedVariablesProvider sharedVariablesProvider, IReservedVariableProvider reservedVariableProvider, ISnippetFileReader snippetFileReader)
		{
			_settings = settings.Value;
			_sharedVariablesProvider = sharedVariablesProvider;
			_reservedVariableProvider = reservedVariableProvider;
			_snippetFileReader = snippetFileReader;
		}

		[Route("api/settings/")]
		[HttpGet]
		public Settings GetSettings()
		{
			return _settings;
		}

		[Route("api/configuration/systemvariables")]
		[HttpGet]
		public IEnumerable<IVariable> GetSystemVariables()
		{
			return _sharedVariablesProvider
						.ListSharedVariables()
						.Concat(_reservedVariableProvider.ListAvailableVariables()
						.Select(x => x.CreateVariable()));
		}

		[Route("api/configuration/scriptsnippetfilenames")]
		[HttpGet]
		public IEnumerable<string> GetScriptSnippetFilenames(ScriptSnippetType snippetType)
		{
			return _snippetFileReader.GetSnippetFilenames(snippetType);
		}
	}
}