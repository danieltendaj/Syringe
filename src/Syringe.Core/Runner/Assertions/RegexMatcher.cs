using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Syringe.Core.Logging;
using Syringe.Core.Tests;

namespace Syringe.Core.Runner.Assertions
{
	internal class RegexMatcher
	{
		private readonly CapturedVariableProvider _variableProvider;
		private readonly AssertionLogger _assertionLogger;

		public RegexMatcher(CapturedVariableProvider variableProvider, AssertionLogger assertionLogger)
		{
			_variableProvider = variableProvider;
			_assertionLogger = assertionLogger;
		}

		public void DoIt(Assertion item, string httpContent)
		{
			string regex = item.Value;

			if (!string.IsNullOrEmpty(regex))
			{
				regex = _variableProvider.ReplaceVariablesIn(regex);
				item.TransformedRegex = regex;

				_assertionLogger.LogRegex(item.Value, regex);

				try
				{
					bool isMatch = Regex.IsMatch(httpContent, regex, RegexOptions.IgnoreCase);
					item.Success = true;

					if (item.AssertionType == AssertionType.Positive && isMatch == false)
					{
						item.Success = false;
						_assertionLogger.LogFail(item.AssertionType, regex);
					}
					else if (item.AssertionType == AssertionType.Negative && isMatch == true)
					{
						item.Success = false;
						_assertionLogger.LogFail(item.AssertionType, regex);
					}
					else
					{
						_assertionLogger.LogSuccess(item.AssertionType, regex);
					}
				}
				catch (ArgumentException e)
				{
					// Invalid regex - ignore.
					item.Success = false;
					_assertionLogger.LogException(e);
				}
			}
			else
			{
				_assertionLogger.LogEmpty();
			}
		}
	}
}