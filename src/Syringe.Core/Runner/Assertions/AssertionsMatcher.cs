using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Syringe.Core.Logging;
using Syringe.Core.Tests;

namespace Syringe.Core.Runner.Assertions
{
    internal class AssertionsMatcher
    {
        private readonly CapturedVariableProvider _variableProvider;

	    public AssertionsMatcher(CapturedVariableProvider variableProvider)
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
		            case AssertionMethod.CSQuery:
			            break;

					case AssertionMethod.Regex:
					default:
						var regexMatcher = new RegexMatcher(_variableProvider, assertionLogger);
						regexMatcher.DoIt(item, httpContent);
			            break;
	            }

	            item.Log = assertionLogger.GetLog();
	            matchedItems.Add(item);
            }

		    return matchedItems;
        }
    }
}