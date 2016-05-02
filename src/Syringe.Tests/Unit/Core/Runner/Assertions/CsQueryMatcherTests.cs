using NUnit.Framework;
using Syringe.Core.Runner;
using Syringe.Core.Runner.Assertions;
using Syringe.Core.Tests;

namespace Syringe.Tests.Unit.Core.Runner.Assertions
{
	[TestFixture]
	public class CsQueryMatcherTests
	{
		private CapturedVariableProvider _variableProvider;
		private AssertionLogger _assertionLogger;

		[SetUp]
		public void Setup()
		{
			_variableProvider = new CapturedVariableProvider("development");
			_assertionLogger = new AssertionLogger();
		}

		private CsQueryMatcher CreateCsQueryMatcher()
		{
			return new CsQueryMatcher(_variableProvider, _assertionLogger);
		}

		[Test]
		public void should_log_empty_when_no_selector_or_httpcontent()
		{
			// given
			CsQueryMatcher matcher = CreateCsQueryMatcher();
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

			CsQueryMatcher matcher = CreateCsQueryMatcher();
			var assertion = new Assertion();
			assertion.Value = "#{variable1} .{variable2}";

			// when
			matcher.Match(assertion, html);

			// then
			Assert.That(assertion.TransformedValue, Is.EqualTo("#1-value .2-value"));
		}

		[Test]
		public void should_log_value_and_selector()
		{
			// given
			CsQueryMatcher matcher = CreateCsQueryMatcher();

			// when

			// then
			Assert.That(matcher, Is.Not.Null);
		}

		[Test]
		public void should_ignore_http_headers_and_use_first_html_tag()
		{
			// given
			CsQueryMatcher matcher = CreateCsQueryMatcher();

			// when

			// then
			Assert.That(matcher, Is.Not.Null);
		}

		[Test]
		public void should_set_success_to_false_when_no_match_found_with_positive_assertiontype()
		{
			// given
			CsQueryMatcher matcher = CreateCsQueryMatcher();

			// when

			// then
			Assert.That(matcher, Is.Not.Null);
		}

		[Test]
		public void should_set_success_to_false_when_match_found_with_negative_assertiontype()
		{
			// given
			CsQueryMatcher matcher = CreateCsQueryMatcher();

			// when

			// then
			Assert.That(matcher, Is.Not.Null);
		}

		[Test]
		public void should_log_succesful_assertions()
		{
			// given
			CsQueryMatcher matcher = CreateCsQueryMatcher();

			// when

			// then
			Assert.That(matcher, Is.Not.Null);
		}

		[Test]
		public void should_log_failed_assertions()
		{
			// given
			CsQueryMatcher matcher = CreateCsQueryMatcher();

			// when

			// then
			Assert.That(matcher, Is.Not.Null);
		}

		[Test]
		public void should_catch_CQ_exceptions()
		{
			// given
			CsQueryMatcher matcher = CreateCsQueryMatcher();

			// when

			// then
			Assert.That(matcher, Is.Not.Null);
		}
	}
}