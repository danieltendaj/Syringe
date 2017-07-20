using System.Collections.Generic;
using System.Threading.Tasks;
using Syringe.Core.Http.Logging;
using Syringe.Core.Tests;

namespace Syringe.Core.Http
{
	public interface IHttpClient
	{
		IRestRequest CreateRestRequest(string httpMethod, string url, string postBody, IEnumerable<HeaderItem> headers);

		Task<HttpResponse> ExecuteRequestAsync(IRestRequest request, HttpLogWriter httpLogWriter);
	}
}