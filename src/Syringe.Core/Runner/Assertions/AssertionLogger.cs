using System;
using Syringe.Core.Runner.Logging;
using Syringe.Core.Tests;

namespace Syringe.Core.Runner.Assertions
{
	internal class AssertionLogger
	{
		public static readonly string EMPTY_ASSERTION_TEXT = "Skipping as the assertion value/http content was empty.";
		private readonly ITestFileRunnerLogger _logger;

		public AssertionLogger(ITestFileRunnerLogger logger)
		{
		    _logger = logger;
		}

	    public string GetLog()
		{
			return _logger.GetLog();
		}

		public void LogItem(AssertionType assertionType, Assertion item)
		{
			_logger.WriteLine("Verifying {0} item \"{1}\"", assertionType, item.Description ?? "(no description)");
		}

		public void LogValue(string originalValue, string transformedValue)
		{
			_logger.WriteLine("- Original assertion value: {0}", originalValue);
			_logger.WriteLine("- Assertion value with variables transformed: {0}", transformedValue);
		}

		public void LogSuccess(AssertionType assertionType, string value, AssertionMethod method)
		{
			_logger.WriteLine("- {0} verification successful: the {1} \"{2}\" matched.", assertionType, method, value);
		}

		public void LogFail(AssertionType assertionType, string value, AssertionMethod method)
		{
			_logger.WriteLine("- {0} verification failed: the {1} \"{2}\" did not match.", assertionType, method, value);
		}

		public void LogException(AssertionMethod method, Exception e)
		{
			_logger.WriteLine("- Invalid {0}: {1}", method, e.Message);
		}

		public void LogEmpty()
		{
			_logger.WriteLine(EMPTY_ASSERTION_TEXT);
		}
	}
}