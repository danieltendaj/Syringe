using System;
using System.Collections.Generic;
using System.Net;

namespace Syringe.Core.Tests
{
	public class Test
	{
		public int Position { get; set; }
		public string Description { get; set; }
		public string Method { get; set; }
		public string Url { get; set; }
		public string PostBody { get; set; }
		public string ErrorMessage { get; set; }
		public HttpStatusCode VerifyResponseCode { get; set; }
		public List<HeaderItem> Headers { get; set; }

		public List<CapturedVariable> CapturedVariables { get; set; }
		public List<Assertion> Assertions { get; set; }

		public string Filename { get; set; }
	    public List<Variable> AvailableVariables { get; set; }

	    public Test()
		{
			Headers = new List<HeaderItem>();
			CapturedVariables = new List<CapturedVariable>();
			Assertions = new List<Assertion>();
            AvailableVariables = new List<Variable>();
		}

		public void AddHeader(string key, string value)
		{
			Headers.Add(new HeaderItem(key, value));
		}
	}
}