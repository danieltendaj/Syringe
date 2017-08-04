using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
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
		private readonly IConfiguration _configuration;
		private readonly ISharedVariablesProvider _sharedVariablesProvider;
		private readonly IReservedVariableProvider _reservedVariableProvider;
		private readonly SnippetFileReader _snippetFileReader;

		public ConfigurationController(IConfiguration configuration, ISharedVariablesProvider sharedVariablesProvider, IReservedVariableProvider reservedVariableProvider, SnippetFileReader snippetFileReader)
		{
			_configuration = configuration;
			_sharedVariablesProvider = sharedVariablesProvider;
			_reservedVariableProvider = reservedVariableProvider;
			_snippetFileReader = snippetFileReader;
		}

		[Route("api/configuration/")]
		[HttpGet]
		public IConfiguration GetConfiguration()
		{
			return _configuration;
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