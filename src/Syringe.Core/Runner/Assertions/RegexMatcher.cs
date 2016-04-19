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

		public void Match(Assertion item, string httpContent)
		{
			string regex = item.Value;

			if (!string.IsNullOrEmpty(regex))
			{
				regex = _variableProvider.ReplaceVariablesIn(regex);
				item.TransformedValue = regex;

				_assertionLogger.LogValue(item.Value, regex);

				try
				{
					bool isMatch = Regex.IsMatch(httpContent, regex, RegexOptions.IgnoreCase);
					item.Success = true;

					if (item.AssertionType == AssertionType.Positive && isMatch == false)
					{
						item.Success = false;
						_assertionLogger.LogFail(item.AssertionType, regex, AssertionMethod.Regex);
					}
					else if (item.AssertionType == AssertionType.Negative && isMatch == true)
					{
						item.Success = false;
						_assertionLogger.LogFail(item.AssertionType, regex, AssertionMethod.Regex);
					}
					else
					{
						_assertionLogger.LogSuccess(item.AssertionType, regex, AssertionMethod.Regex);
					}
				}
				catch (ArgumentException e)
				{
					// Invalid regex - ignore.
					item.Success = false;
					_assertionLogger.LogException(AssertionMethod.Regex, e);
				}
			}
			else
			{
				_assertionLogger.LogEmpty();
			}
		}
	}
}