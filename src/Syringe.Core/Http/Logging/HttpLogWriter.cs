using System;
using System.IO;
using System.Linq;
using System.Text;
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
			_writer.WriteLine(RESPONSE_LINE_FORMAT, statusCode, GetStatusDescription(statusCode));

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

        public static string GetStatusDescription(int code)
        {
            switch (code)
            {
                case 100: return "Continue";
                case 101: return "Switching Protocols";
                case 102: return "Processing";
                case 200: return "OK";
                case 201: return "Created";
                case 202: return "Accepted";
                case 203: return "Non-Authoritative Information";
                case 204: return "No Content";
                case 205: return "Reset Content";
                case 206: return "Partial Content";
                case 207: return "Multi-Status";
                case 300: return "Multiple Choices";
                case 301: return "Moved Permanently";
                case 302: return "Found";
                case 303: return "See Other";
                case 304: return "Not Modified";
                case 305: return "Use Proxy";
                case 307: return "Temporary Redirect";
                case 400: return "Bad Request";
                case 401: return "Unauthorized";
                case 402: return "Payment Required";
                case 403: return "Forbidden";
                case 404: return "Not Found";
                case 405: return "Method Not Allowed";
                case 406: return "Not Acceptable";
                case 407: return "Proxy Authentication Required";
                case 408: return "Request Timeout";
                case 409: return "Conflict";
                case 410: return "Gone";
                case 411: return "Length Required";
                case 412: return "Precondition Failed";
                case 413: return "Request Entity Too Large";
                case 414: return "Request-Uri Too Long";
                case 415: return "Unsupported Media Type";
                case 416: return "Requested Range Not Satisfiable";
                case 417: return "Expectation Failed";
                case 422: return "Unprocessable Entity";
                case 423: return "Locked";
                case 424: return "Failed Dependency";
                case 500: return "Internal Server Error";
                case 501: return "Not Implemented";
                case 502: return "Bad Gateway";
                case 503: return "Service Unavailable";
                case 504: return "Gateway Timeout";
                case 505: return "Http Version Not Supported";
                case 507: return "Insufficient Storage";
            }
            return "";
        }
    }
}
