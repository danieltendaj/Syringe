using System;
using System.Collections.Generic;

namespace Syringe.Service.Api
{
	public class TestFileRunResult
	{
	    public Guid? ResultId { get; set; }
		public bool Completed { get; set; }
		public bool HasFailedTests { get; set; }
		public TimeSpan TimeTaken { get; set; }
		public string ErrorMessage { get; set; }
		public IEnumerable<LightweightResult> TestResults { get; set; }

		public TestFileRunResult()
		{
			TestResults = new List<LightweightResult>();
		}
	}

	public class LightweightResult
	{
		public string TestDescription { get; set; }
		public string TestUrl { get; set; }
		public string ActualUrl { get; set; }
		public string Message { get; set; }
		public TimeSpan ResponseTime { get; set; }
		public bool ResponseCodeSuccess { get; set; }
		public string ExceptionMessage { get; set; }

		public bool Success { get; set; }
		public bool AssertionsSuccess { get; set; }
		public bool ScriptCompilationSuccess { get; set; }
	}
}