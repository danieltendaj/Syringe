using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Syringe.Core.Http.Logging;
using Syringe.Core.Tests;

namespace Syringe.Core.Http
{
	public interface IHttpClientAdapter
	{
		Task<HttpResponse> SendAsync(HttpLogWriter httpLogWriter, string httpMethod, string url, string postBody, IEnumerable<HeaderItem> headers);
	}
}