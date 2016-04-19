using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;
using Syringe.Core.Http;

namespace Syringe.Core.Tests.Results
{
    [BsonIgnoreExtraElements]
    public class TestResult
	{
		public int Position { get; set; }
		public Guid SessionId { get; set; }
	    public Test Test { get; set; }
		public string ActualUrl { get; set; }
	    public string Message { get; set; }
	    public TimeSpan ResponseTime { get; set; }
        public List<Assertion> AssertionResults { get; set; }
		public bool ResponseCodeSuccess { get; set; }
		public HttpResponse HttpResponse { get; set; }
		public string ExceptionMessage { get; set; }
		public string HttpLog { get; set; }
		public string HttpContent { get; set; }
		public string Log { get; set; }

		public bool Success
		{
			get { return ResponseCodeSuccess && AssertionsSuccess; }
		}
		
		public bool AssertionsSuccess
		{
			get
			{
				if (AssertionResults == null || AssertionResults.Count == 0)
					return true;

				return AssertionResults.Count(x => x.Success == false) == 0;
			}
		}
		public TestResult()
		{
            AssertionResults = new List<Assertion>();
		}
	}
}
