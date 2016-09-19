using System.Collections.Generic;
using RestSharp;
using Syringe.Client.Http;
using Syringe.Core.Configuration;
using Syringe.Core.Services;
using Syringe.Core.Tests.Scripting;
using Syringe.Core.Tests.Variables;

namespace Syringe.Client
{
	public class ConfigurationClient : IConfigurationService
	{
		internal readonly string ServiceUrl;
		private readonly FlurlWrapper _wrapper;

		public ConfigurationClient(string serviceUrl)
		{
			ServiceUrl = serviceUrl;
            var factory = new CustomHttpClientFactory(serviceUrl);
			_wrapper = new FlurlWrapper(factory, "/api/configuration");
		}

		public IConfiguration GetConfiguration()
		{
		    return _wrapper.Get<IConfiguration>("").Result;
		}

	    public IEnumerable<IVariable> GetSystemVariables()
        {
            return _wrapper.Get<List<Variable>>("systemvariables").Result;
        }

		public IEnumerable<string> GetScriptSnippetFilenames(ScriptSnippetType snippetType)
		{
            _wrapper.AddParameter("snippetType", snippetType.ToString());
            return _wrapper.Get<List<string>>("scriptsnippetfilenames").Result;
        }
    }
}