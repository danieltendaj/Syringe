using System;
using System.Collections.Generic;

namespace Syringe.Service.Models
{
	public class TestFileRunResult
	{
	    public Guid? ResultId { get; set; }
		public bool Finished { get; set; }
        public bool TestRunFailed { get; set; }
        public bool HasFailedTests { get; set; }
		public TimeSpan TimeTaken { get; set; }
		public string ErrorMessage { get; set; }
		public IEnumerable<LightweightResult> TestResults { get; set; }

	    public TestFileRunResult()
		{
			TestResults = new List<LightweightResult>();
		}
	}
}