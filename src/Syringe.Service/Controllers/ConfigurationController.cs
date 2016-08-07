using System.Collections.Generic;
using System.Web.Http;
using Syringe.Core.Configuration;
using Syringe.Core.Services;
using Syringe.Core.Tests.Scripting;
using Syringe.Core.Tests.Variables;

namespace Syringe.Service.Controllers
{
	public class ConfigurationController : ApiController, IConfigurationService
	{
		private readonly IConfiguration _configuration;
        private readonly IVariableContainer _variableContainer;
		private readonly SnippetFileReader _snippetFileReader;

		public ConfigurationController(IConfiguration configuration, IVariableContainer variableContainer, SnippetFileReader snippetFileReader)
		{
			_configuration = configuration;
            _variableContainer = variableContainer;
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
        public IEnumerable<Variable> GetSystemVariables()
		{
            return _variableContainer;
		}

		[Route("api/configuration/scriptsnippetfilenames")]
		[HttpGet]
		public IEnumerable<string> GetScriptSnippetFilenames(ScriptSnippetType snippetType)
		{
			return _snippetFileReader.GetSnippetFilenames(snippetType);
		}
	}
}
