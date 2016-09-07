using System;
using CsQuery;
using Syringe.Core.Tests;

namespace Syringe.Core.Runner.Assertions
{
	internal class AngleSharpMatcher
	{
		private readonly ICapturedVariableProvider _variableProvider;
		private readonly AssertionLogger _assertionLogger;

		public AngleSharpMatcher(ICapturedVariableProvider variableProvider, AssertionLogger assertionLogger)
		{
			_variableProvider = variableProvider;
			_assertionLogger = assertionLogger;
		}

		public void Match(Assertion assertion, string httpContent)
		{
			string selector = assertion.Value;

			if (!string.IsNullOrEmpty(selector) && !string.IsNullOrEmpty(httpContent))
			{
				selector = _variableProvider.ReplaceVariablesIn(selector);
				assertion.TransformedValue = selector;

				_assertionLogger.LogValue(assertion.Value, selector);

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

					switch (assertion.AssertionType)
					{
						case AssertionType.Positive:
							if (isMatch == false)
							{
								assertion.Success = false;
								_assertionLogger.LogFail(assertion.AssertionType, selector, AssertionMethod.CssSelector);
							}
							else
							{
								_assertionLogger.LogSuccess(assertion.AssertionType, selector, AssertionMethod.CssSelector);
								assertion.Success = true;
							}
							break;

						case AssertionType.Negative:
							if (isMatch == true)
							{
								assertion.Success = false;
								_assertionLogger.LogFail(assertion.AssertionType, selector, AssertionMethod.CssSelector);
							}
							else
							{
								_assertionLogger.LogSuccess(assertion.AssertionType, selector, AssertionMethod.CssSelector);
								assertion.Success = true;
							}
							break;

						default:
							throw new ArgumentOutOfRangeException();
					}
				}
				catch (Exception e)
				{
					// Something went wrong, CQ doesn't tell use what exceptions it throws
					assertion.Success = false;
					_assertionLogger.LogException(AssertionMethod.CssSelector, e);
				}
			}
			else
			{
				_assertionLogger.LogEmpty();
			}
		}
	}
}