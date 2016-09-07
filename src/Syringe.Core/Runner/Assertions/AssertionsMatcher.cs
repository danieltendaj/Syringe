using System.Collections.Generic;
using Syringe.Core.Tests;

namespace Syringe.Core.Runner.Assertions
{
    internal class AssertionsMatcher
    {
        private readonly ICapturedVariableProvider _variableProvider;

	    public AssertionsMatcher(ICapturedVariableProvider variableProvider)
	    {
		    _variableProvider = variableProvider;
	    }

	    public List<Assertion> MatchVerifications(List<Assertion> verifications, string httpContent)
        {
            var matchedItems = new List<Assertion>();

            foreach (Assertion item in verifications)
            {
                var assertionLogger = new AssertionLogger();
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