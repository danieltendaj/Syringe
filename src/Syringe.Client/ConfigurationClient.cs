using RestSharp;
using Syringe.Core.Configuration;
using Syringe.Core.Services;

namespace Syringe.Client
{
	public class ConfigurationClient : IConfigurationService
	{
		private readonly string _serviceUrl;
		private readonly RestSharpHelper _restSharpHelper;
		private static IConfiguration _configuration;

		public ConfigurationClient(string serviceUrl)
		{
			_serviceUrl = serviceUrl;
			_restSharpHelper = new RestSharpHelper("/api/configuration");
		}

		public IConfiguration GetConfiguration()
		{
			if (_configuration == null)
			{
				var client = new RestClient(_serviceUrl);
				IRestRequest request = _restSharpHelper.CreateRequest("");

				IRestResponse response = client.Execute(request);
				_configuration = _restSharpHelper.DeserializeOrThrow<JsonConfiguration>(response);
			}

			return _configuration;
		}
	}
}