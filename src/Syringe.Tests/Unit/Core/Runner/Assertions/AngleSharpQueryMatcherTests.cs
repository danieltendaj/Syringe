using Moq;
using NUnit.Framework;
using Syringe.Core.Runner;
using Syringe.Core.Runner.Assertions;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Variables;
using Syringe.Tests.StubsMocks;

namespace Syringe.Tests.Unit.Core.Runner.Assertions
{
    [TestFixture]
    public class AngleSharpQueryMatcherTests
    {
        private VariableContainerStub _variableContainerStub;
        private CapturedVariableProvider _variableProvider;
        private AssertionLogger _assertionLogger;

        [SetUp]
        public void Setup()
        {
            _variableContainerStub = new VariableContainerStub();
            _variableProvider = new CapturedVariableProvider(_variableContainerStub, "development", new VariableEncryptorStub());
            _assertionLogger = new AssertionLogger();
        }

        private AngleSharpMatcher CreateCsQueryMatcher()
        {
            return new AngleSharpMatcher(_variableProvider, _assertionLogger);
        }

        [Test]
        public void should_log_empty_when_no_selector_or_httpcontent()
        {
            // given
            AngleSharpMatcher matcher = CreateCsQueryMatcher();
            var assertion = new Assertion();

            // when
            matcher.Match(assertion, "");

            // then
            Assert.That(_assertionLogger.GetLog(), Is.StringContaining(AssertionLogger.EMPTY_ASSERTION_TEXT));
        }

        [Test]
        public void should_replace_variables_in_selector()
        {
            // given
            string html = "<html></html>";

            var variable1 = new Variable("variable1", "1-value", "development");
            var variable2 = new Variable("variable2", "2-value", "development");
            _variableProvider.AddOrUpdateVariable(variable1);
            _variableProvider.AddOrUpdateVariable(variable2);

            AngleSharpMatcher matcher = CreateCsQueryMatcher();
            var assertion = new Assertion();
            assertion.Value = "#{variable1} .{variable2}";

            // when
            matcher.Match(assertion, html);

            // then
            Assert.That(assertion.TransformedValue, Is.EqualTo("#1-value .2-value"));
        }

        [Test]
        public void should_ignore_http_headers_and_use_first_html_tag()
        {
            string httpContent = @"HTTP/1.1 200 OK                                                                                                                                      
MyHeader: <body class='foo'></body>
Cache - Control: private, max-age=0                                                                                                                    
Server: gws

<html>
<body class='foo'>some text here</body>
</html>";

            AngleSharpMatcher matcher = CreateCsQueryMatcher();
            var assertion = new Assertion();
            assertion.AssertionType = AssertionType.Positive;
            assertion.Value = "body.foo:contains('some text here')";

            // when
            matcher.Match(assertion, httpContent);

            // then
            Assert.That(assertion.Success, Is.True);
        }

        [Test]
        public void should_set_success_to_false_when_no_match_found_with_positive_assertiontype()
        {
            string httpContent = @"<html><body class='foo'>some text here</body></html>";

            AngleSharpMatcher matcher = CreateCsQueryMatcher();
            var assertion = new Assertion();
            assertion.AssertionType = AssertionType.Positive;
            assertion.Value = "body.someclass";

            // when
            matcher.Match(assertion, httpContent);

            // then
            Assert.That(assertion.Success, Is.False);
        }

        [Test]
        public void should_set_success_to_true_when_match_found_with_negative_assertiontype()
        {
            string httpContent = @"<html><body class='foo'>some text here</body></html>";

            AngleSharpMatcher matcher = CreateCsQueryMatcher();
            var assertion = new Assertion();
            assertion.AssertionType = AssertionType.Negative;
            assertion.Value = "div#blahblah";

            // when
            matcher.Match(assertion, httpContent);

            // then
            Assert.That(assertion.Success, Is.True);
        }

        [Test]
        public void should_log_value_and_selector()
        {
            // given
            string html = "<html></html>";

            AngleSharpMatcher matcher = CreateCsQueryMatcher();
            var assertion = new Assertion();
            assertion.Value = "#id .class body";

            // when
            matcher.Match(assertion, html);

            // then
            string log = _assertionLogger.GetLog();
            Assert.That(log, Is.StringContaining("Original assertion value: #id .class body"));
            Assert.That(log, Is.StringContaining("Assertion value with variables transformed: #id .class body"));
        }

        [Test]
        public void should_log_succesful_assertions()
        {
            // given
            string html = "<html><body id=mybody></body></html>";

            AngleSharpMatcher matcher = CreateCsQueryMatcher();
            var assertion = new Assertion();
            assertion.Value = "body#mybody";

            // when
            matcher.Match(assertion, html);

            // then
            string log = _assertionLogger.GetLog();
            Assert.That(log, Is.StringContaining("Positive verification successful: the CssSelector \"body#mybody\" matched."));
        }

        [Test]
        public void should_log_failed_assertions()
        {
            // given
            string html = "<html><body></body></html>";

            AngleSharpMatcher matcher = CreateCsQueryMatcher();
            var assertion = new Assertion();
            assertion.Value = "pre";

            // when
            matcher.Match(assertion, html);

            // then
            string log = _assertionLogger.GetLog();
            Assert.That(log, Is.StringContaining("Positive verification failed: the CssSelector \"pre\" did not match."));
        }

        [Test]
        public void should_catch_CQ_exceptions()
        {
            // given
            string html = "<html><body></body></html>";

            AngleSharpMatcher matcher = CreateCsQueryMatcher();
            var assertion = new Assertion();
            assertion.Value = "### ...";

            // when
            matcher.Match(assertion, html);

            // then
            Assert.That(assertion.Success, Is.False);
        }
    }
}