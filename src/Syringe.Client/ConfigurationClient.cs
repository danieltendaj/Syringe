using RestSharp;
using Syringe.Core.Configuration;
using Syringe.Core.Services;

namespace Syringe.Client
{
	public class ConfigurationClient : IConfigurationService
	{
		private readonly string _serviceUrl;
		private readonly RestSharpHelper _restSharpHelper;

		public ConfigurationClient(string serviceUrl)
		{
			_serviceUrl = serviceUrl;
			_restSharpHelper = new RestSharpHelper("/api/configuration");
		}

		public IConfiguration GetConfiguration()
		{
			var client = new RestClient(_serviceUrl);
			IRestRequest request = _restSharpHelper.CreateRequest("");

			IRestResponse response = client.Execute(request);
			return _restSharpHelper.DeserializeOrThrow<JsonConfiguration>(response);
		}
	}
}