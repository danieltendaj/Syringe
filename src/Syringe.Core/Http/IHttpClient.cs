using System.Collections.Generic;
using System.Threading.Tasks;
using Syringe.Core.Http.Logging;
using Syringe.Core.Tests;

namespace Syringe.Core.Http
{
	public interface IHttpClient
	{
		Task<HttpResponse> ExecuteRequestAsync(string httpMethod, string url, string postBody, IEnumerable<HeaderItem> headers, HttpLogWriter httpLogWriter);
	}
}