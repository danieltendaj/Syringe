using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;

namespace Syringe.Core.Tests.Results
{
    [BsonIgnoreExtraElements]
    public class TestFileResult
    {
	    public Guid Id { get; set; }
	    public string Filename { get; set; }
        public string Environment { get; set; }
	    public string Username { get; set; }

        public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
        public TimeSpan TotalRunTime { get; set; }
        public int TotalTestsRun { get; set; }
		public TimeSpan MaxResponseTime { get; set; }
		public TimeSpan MinResponseTime { get; set; }
		public List<TestResult> TestResults { get; set; }

		public int TotalTestsPassed => TestResults.Count(x => x.Success == true);
	    public int TotalTestsFailed => TestResults.Count(x => x.Success == false);

	    public int TotalAssertionsPassed
		{
			get
			{
				return TestResults.Sum(x => x.AssertionResults.Count(v => v.Success == true));
			}
		}

		public int TotalAssertionsFailed
		{
			get 
			{ 
				return TestResults.Sum(x => x.AssertionResults.Count(v => v.Success == false)); 
			}
		}

        public TestFileResult()
        {
            TestResults = new List<TestResult>();
        }
    }
}