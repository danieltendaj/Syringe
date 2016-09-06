using System;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
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

                try
                {
                    var parser = new HtmlParser();
                    IHtmlDocument document = parser.Parse(httpContent);

                    IHtmlCollection<IElement> result = document.QuerySelectorAll(selector);
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
                    // Something happened with Anglesharp, it doesn't document its exceptions
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