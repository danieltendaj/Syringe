using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Syringe.Core.Logging;
using Syringe.Core.Tests;

namespace Syringe.Core.Runner
{
    internal class AssertionsMatcher
    {
        private readonly CapturedVariableProvider _variables;

        public AssertionsMatcher(CapturedVariableProvider variables)
        {
            _variables = variables;
        }

        public List<Assertion> MatchVerifications(List<Assertion> verifications, string content)
        {
            var matchedItems = new List<Assertion>();

            foreach (Assertion item in verifications)
            {
                var simpleLogger = new SimpleLogger();

                LogItem(simpleLogger, item.AssertionType, item);
                string regex = item.Value;

                if (!string.IsNullOrEmpty(regex))
                {
                    regex = _variables.ReplaceVariablesIn(regex);
                    item.TransformedRegex = regex;

                    LogRegex(simpleLogger, item.Value, regex);

                    try
                    {
                        bool isMatch = Regex.IsMatch(content, regex, RegexOptions.IgnoreCase);
                        item.Success = true;

                        if (item.AssertionType == AssertionType.Positive && isMatch == false)
                        {
                            item.Success = false;
                            LogFail(simpleLogger, item.AssertionType, regex);
                        }
                        else if (item.AssertionType == AssertionType.Negative && isMatch == true)
                        {
                            item.Success = false;
                            LogFail(simpleLogger, item.AssertionType, regex);
                        }
                        else
                        {
                            LogSuccess(simpleLogger, item.AssertionType, regex);
                        }
                    }
                    catch (ArgumentException e)
                    {
                        // Invalid regex - ignore.
                        item.Success = false;
                        LogException(simpleLogger, e);
                    }
                }
                else
                {
                    LogEmpty(simpleLogger);
                }

                item.Log = simpleLogger.GetLog();
                matchedItems.Add(item);
            }

            return matchedItems;
        }

        private void LogItem(SimpleLogger logger, AssertionType assertionType, Assertion item)
        {
            logger.WriteLine("");
            logger.WriteLine("Verifying {0} item \"{1}\"", assertionType, item.Description);
        }

        private void LogRegex(SimpleLogger logger, string originalRegex, string transformedRegex)
        {
            logger.WriteLine("  - Original regex: {0}", originalRegex);
            logger.WriteLine("  - Regex with variables transformed: {0}", transformedRegex);
        }

        private void LogSuccess(SimpleLogger logger, AssertionType assertionType, string verifyRegex)
        {
            logger.WriteLine("  - {0} verification successful: the regex \"{1}\" matched.", assertionType, verifyRegex);
        }

        private void LogFail(SimpleLogger logger, AssertionType assertionType, string verifyRegex)
        {
            logger.WriteLine("  - {0} verification failed: the regex \"{1}\" did not match.", assertionType, verifyRegex);
        }

        private void LogException(SimpleLogger logger, Exception e)
        {
            logger.WriteLine(" - Invalid regex: {0}", e.Message);
        }

        private void LogEmpty(SimpleLogger logger)
        {
            logger.WriteLine("  - Skipping as the regex was empty.");
        }
    }
}