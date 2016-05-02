using System;
using Syringe.Core.Logging;
using Syringe.Core.Tests;

namespace Syringe.Core.Runner.Assertions
{
	internal class AssertionLogger
	{
		public static readonly string EMPTY_ASSERTION_TEXT = "Skipping as the assertion value/http content was empty.";
		private readonly SimpleLogger _logger;

		public AssertionLogger()
		{
			_logger = new SimpleLogger();
		}

		public string GetLog()
		{
			return _logger.GetLog();
		}

		public void LogItem(AssertionType assertionType, Assertion item)
		{
			_logger.WriteIndentedLine("Verifying {0} item \"{1}\"", assertionType, item.Description);
		}

		public void LogValue(string originalValue, string transformedValue)
		{
			_logger.WriteDoubleIndentedLine("Original assertion value: {0}", originalValue);
			_logger.WriteDoubleIndentedLine("Assertion value with variables transformed: {0}", transformedValue);
		}

		public void LogSuccess(AssertionType assertionType, string value, AssertionMethod method)
		{
			_logger.WriteDoubleIndentedLine("{0} verification successful: the {1} \"{2}\" matched.", assertionType, method, value);
		}

		public void LogFail(AssertionType assertionType, string value, AssertionMethod method)
		{
			_logger.WriteDoubleIndentedLine("{0} verification failed: the {1} \"{2}\" did not match.", assertionType, method, value);
		}

		public void LogException(AssertionMethod method, Exception e)
		{
			_logger.WriteDoubleIndentedLine("Invalid {0}: {1}", method, e.Message);
		}

		public void LogEmpty()
		{
			_logger.WriteIndentedLine(EMPTY_ASSERTION_TEXT);
		}
	}
}