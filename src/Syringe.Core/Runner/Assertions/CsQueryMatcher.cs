using System;
using CsQuery;
using Syringe.Core.Tests;

namespace Syringe.Core.Runner.Assertions
{
	internal class CsQueryMatcher
	{
		private readonly CapturedVariableProvider _variableProvider;
		private readonly AssertionLogger _assertionLogger;

		public CsQueryMatcher(CapturedVariableProvider variableProvider, AssertionLogger assertionLogger)
		{
			_variableProvider = variableProvider;
			_assertionLogger = assertionLogger;
		}

		public void Match(Assertion item, string httpContent)
		{
			string selector = item.Value;

			if (!string.IsNullOrEmpty(selector) && !string.IsNullOrEmpty(httpContent))
			{
				selector = _variableProvider.ReplaceVariablesIn(selector);
				item.TransformedValue = selector;

				_assertionLogger.LogValue(item.Value, selector);

				// Grab everything from the start of the first html tag.
				string html = "";
				int htmlTagStart = httpContent.IndexOf("<html", StringComparison.InvariantCultureIgnoreCase);
				if (htmlTagStart > -1)
				{
					html = httpContent.Substring(htmlTagStart);
				}

				try
				{
					CQ cq = CQ.Create(html);
					CQ result = cq.Find(selector);
					bool isMatch = (result != null && result.Length > 0);
					item.Success = isMatch;

					if (item.AssertionType == AssertionType.Positive && isMatch == false)
					{
						item.Success = false;
						_assertionLogger.LogFail(item.AssertionType, selector, AssertionMethod.CSQuery);
					}
					else if (item.AssertionType == AssertionType.Negative && isMatch == true)
					{
						item.Success = false;
						_assertionLogger.LogFail(item.AssertionType, selector, AssertionMethod.CSQuery);
					}
					else
					{
						_assertionLogger.LogSuccess(item.AssertionType, selector, AssertionMethod.CSQuery);
					}
				}
				catch (Exception e)
				{
					// Something went wrong, CQ doesn't tell use what exceptions it throws
					item.Success = false;
					_assertionLogger.LogException(AssertionMethod.CSQuery, e);
				}
			}
			else
			{
				_assertionLogger.LogEmpty();
			}
		}
	}
}