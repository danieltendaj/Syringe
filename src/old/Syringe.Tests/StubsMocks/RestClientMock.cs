using System;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;

namespace Syringe.Tests.StubsMocks
{
	public class RestClientMock : RestClient
	{
		public IRestResponse RestResponse { get; set; }

		public IRestRequest RestRequest { get; private set; }
		public TimeSpan ResponseTime { get; set; }

		public override Task<IRestResponse> ExecuteTaskAsync(IRestRequest request)
		{
			if (ResponseTime > TimeSpan.MinValue)
				Thread.Sleep(ResponseTime);

			RestRequest = request;
			return Task.FromResult(RestResponse);
		}

		public override IRestResponse Execute(IRestRequest request)
		{
		    if (ResponseTime > TimeSpan.MinValue)
		    {
		        Thread.Sleep(ResponseTime);
		    }

			RestRequest = request;
			return RestResponse;
		}
	}
}