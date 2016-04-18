using System;
using System.Collections.Generic;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Results;

namespace Syringe.Tests.Unit.Runner
{
	internal class TestFileResultsBuilder
	{
		private readonly List<TestResult> _testResults;
		private TestResult _currentTestResult;

		public TestFileResultsBuilder()
		{
			_testResults = new List<TestResult>();
		}

		public TestFileResultsBuilder New()
		{
			_currentTestResult = new TestResult();
			return this;
		}

		public TestFileResultsBuilder WithSuccess()
		{
			_currentTestResult.ResponseCodeSuccess = true;
			return this;
		}

		public TestFileResultsBuilder WithFail()
		{
			_currentTestResult.ResponseCodeSuccess = false;
			return this;
		}

		public TestFileResultsBuilder AddPositiveVerify(bool success = true)
		{
			_currentTestResult.AssertionResults.Add(new Assertion("item " + DateTime.Now, "regex", AssertionType.Positive, AssertionMethod.Regex) {Success = success});
			return this;
		}

		public TestFileResultsBuilder AddNegativeVerify(bool success = true)
		{
			_currentTestResult.AssertionResults.Add(new Assertion("item " + DateTime.Now, "regex", AssertionType.Negative, AssertionMethod.Regex) {Success = success});
			return this;
		}

		public TestFileResultsBuilder Add()
		{
			_testResults.Add(_currentTestResult);
			return this;
		}

		public List<TestResult> GetCollection()
		{
			return _testResults;
		}

	}
}