using System.Collections.Generic;
using Syringe.Core.Runner.Logging;
using Syringe.Core.Tests;

namespace Syringe.Core.Runner.Assertions
{
    internal class AssertionsMatcher
    {
        private readonly ICapturedVariableProvider _variableProvider;
        private readonly ITestFileRunnerLogger _logger;

        public AssertionsMatcher(ICapturedVariableProvider variableProvider, ITestFileRunnerLogger logger)
        {
            _variableProvider = variableProvider;
            _logger = logger;
        }

        public List<Assertion> MatchVerifications(List<Assertion> verifications, string httpContent)
        {
            var matchedItems = new List<Assertion>();

            foreach (Assertion item in verifications)
            {
                var assertionLogger = new AssertionLogger(_logger);
				assertionLogger.LogItem(item.AssertionType, item);

	            switch (item.AssertionMethod)
	            {
		            case AssertionMethod.CssSelector:
						var cqMatcher = new AngleSharpMatcher(_variableProvider, assertionLogger);
						cqMatcher.Match(item, httpContent);
			            break;

					case AssertionMethod.Regex:
					default:
						var regexMatcher = new RegexMatcher(_variableProvider, assertionLogger);
						regexMatcher.Match(item, httpContent);
			            break;
	            }

	            item.Log = assertionLogger.GetLog();
	            matchedItems.Add(item);
            }

		    return matchedItems;
        }
    }
}