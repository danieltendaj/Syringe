using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using LiteDB;

namespace Syringe.Core.Http
{
    public class HttpResponse
	{
		public HttpStatusCode StatusCode { get; set; }
		public string Content { get; set; }
        public TimeSpan ResponseTime { get; set; }
	    public List<HttpHeader> Headers { get; set; }
        // end work arounds

        public HttpResponse()
        {
            Headers = new List<HttpHeader>();
		}

		public override string ToString()
		{
			var stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(string.Format("HTTP/1.1 {0} OK", (int)StatusCode));

			foreach (HttpHeader keyValuePair in Headers)
			{
				stringBuilder.AppendLine(string.Format("{0}: {1}", keyValuePair.Key, keyValuePair.Value));
			}

			stringBuilder.AppendLine();
			stringBuilder.Append(Content);

			return stringBuilder.ToString();
		}
    }
}