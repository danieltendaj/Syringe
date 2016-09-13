using System.Collections.Generic;
using RestSharp;
using Syringe.Core.Configuration;
using Syringe.Core.Services;
using Syringe.Core.Tests.Scripting;
using Syringe.Core.Tests.Variables;

namespace Syringe.Client
{
	public class ConfigurationClient : IConfigurationService
	{
		internal readonly string ServiceUrl;
		private readonly RestSharpHelper _restSharpHelper;

		public ConfigurationClient(string serviceUrl)
		{
			ServiceUrl = serviceUrl;
			_restSharpHelper = new RestSharpHelper("/api/configuration");
		}

		public IConfiguration GetConfiguration()
		{
			var client = new RestClient(ServiceUrl);
			IRestRequest request = _restSharpHelper.CreateRequest("");

			IRestResponse response = client.Execute(request);
			return _restSharpHelper.DeserializeOrThrow<JsonConfiguration>(response);
		}

	    public IEnumerable<IVariable> GetSystemVariables()
        {
            var client = new RestClient(ServiceUrl);
            IRestRequest request = _restSharpHelper.CreateRequest("systemvariables");

            IRestResponse response = client.Execute(request);
            return _restSharpHelper.DeserializeOrThrow<List<Variable>>(response);
        }

		public IEnumerable<string> GetScriptSnippetFilenames(ScriptSnippetType snippetType)
		{
			var client = new RestClient(ServiceUrl);
			IRestRequest request = _restSharpHelper.CreateRequest("scriptsnippetfilenames");
			request.AddQueryParameter("snippetType", snippetType.ToString());

			IRestResponse response = client.Execute(request);
			return _restSharpHelper.DeserializeOrThrow<List<string>>(response);
		}
	}
}