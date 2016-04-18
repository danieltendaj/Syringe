using System;
using Syringe.Core.Logging;
using Syringe.Core.Tests;

namespace Syringe.Core.Runner.Assertions
{
	internal class AssertionLogger
	{
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
			_logger.WriteLine("");
			_logger.WriteLine("Verifying {0} item \"{1}\"", assertionType, item.Description);
		}

		public void LogRegex(string originalRegex, string transformedRegex)
		{
			_logger.WriteLine("  - Original regex: {0}", originalRegex);
			_logger.WriteLine("  - Regex with variables transformed: {0}", transformedRegex);
		}

		public void LogSuccess(AssertionType assertionType, string verifyRegex)
		{
			_logger.WriteLine("  - {0} verification successful: the regex \"{1}\" matched.", assertionType, verifyRegex);
		}

		public void LogFail(AssertionType assertionType, string verifyRegex)
		{
			_logger.WriteLine("  - {0} verification failed: the regex \"{1}\" did not match.", assertionType, verifyRegex);
		}

		public void LogException(Exception e)
		{
			_logger.WriteLine(" - Invalid regex: {0}", e.Message);
		}

		public void LogEmpty()
		{
			_logger.WriteLine("  - Skipping as the regex was empty.");
		}
	}
}