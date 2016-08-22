using System.Collections.Generic;
using RestSharp;
using Syringe.Core.Services;
using Environment = Syringe.Core.Environment.Environment;

namespace Syringe.Client
{
	public class EnvironmentsClient : IEnvironmentsService
	{
		internal readonly string ServiceUrl;
		private readonly RestSharpHelper _restSharpHelper;
		
		public EnvironmentsClient(string serviceUrl)
		{
			ServiceUrl = serviceUrl;
			_restSharpHelper = new RestSharpHelper("/api/environments");
		}

		public IEnumerable<Environment> Get()
		{
			var client = new RestClient(ServiceUrl);
			IRestRequest request = _restSharpHelper.CreateRequest();

			IRestResponse response = client.Execute(request);
			return _restSharpHelper.DeserializeOrThrow<IEnumerable<Environment>>(response);
		}
	}
}