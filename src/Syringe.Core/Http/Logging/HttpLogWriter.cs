using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using RestSharp;

namespace Syringe.Core.Http.Logging
{
	public class HttpLogWriter
	{
		private static readonly string REQUEST_LINE_FORMAT = "{0} {1} HTTP/1.1";
		private static readonly string HEADER_FORMAT = "{0}: {1}";
		private static readonly string RESPONSE_LINE_FORMAT = "HTTP/1.1 {0} {1}";
		private readonly StringWriter _writer;

		public StringBuilder StringBuilder { get; set; }

		public HttpLogWriter()
		{
			StringBuilder = new StringBuilder();
			_writer = new StringWriter(StringBuilder);
		}

		public virtual void AppendRequest(Uri uri, IRestRequest request)
		{
			_writer.WriteLine(REQUEST_LINE_FORMAT, request.Method.ToString().ToUpper(), uri.ToString());
			_writer.WriteLine(HEADER_FORMAT, "Host", uri.Host);

			if (request.Parameters != null)
			{
				foreach (Parameter parameter in request.Parameters.Where(x => x.Type == ParameterType.HttpHeader))
				{
					_writer.WriteLine(HEADER_FORMAT, parameter.Name, parameter.Value);
				}

				Parameter postBody = request.Parameters.FirstOrDefault(x => x.Type == ParameterType.RequestBody);
				if (postBody != null)
					_writer.WriteLine(postBody.Value);
			}

			_writer.WriteLine();
		}

		public virtual void AppendResponse(IRestResponse response)
		{
			int statusCode = (int)response.StatusCode;
			_writer.WriteLine(RESPONSE_LINE_FORMAT, statusCode, HttpWorkerRequest.GetStatusDescription(statusCode));

			if (response.Headers != null)
			{
				foreach (Parameter parameter in response.Headers)
				{
					_writer.WriteLine(HEADER_FORMAT, parameter.Name, parameter.Value);
				}
			}

			_writer.WriteLine();

			if (!string.IsNullOrEmpty(response.Content))
				_writer.WriteLine(response.Content);
		}
	}
}
