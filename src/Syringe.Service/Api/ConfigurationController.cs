using System.Collections.Generic;
using System.Web.Http;
using Syringe.Core.Configuration;
using Syringe.Core.Services;
using Syringe.Core.Tests.Variables;

namespace Syringe.Service.Api
{
	public class ConfigurationController : ApiController, IConfigurationService
	{
		private readonly IConfiguration _configuration;
        private readonly IVariableContainer _variableContainer;

        public ConfigurationController(IConfiguration configuration, IVariableContainer variableContainer)
		{
			_configuration = configuration;
            _variableContainer = variableContainer;
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
	}
}
