using System.Collections.Generic;
using NUnit.Framework;
using Syringe.Core.Runner;
using Syringe.Core.Runner.Assertions;
using Syringe.Core.Tests;

namespace Syringe.Tests.Unit.Runner
{
	public class VerificationsMatcherTests
	{
		[SetUp]
		public void Setup()
		{
			TestHelpers.EnableLogging();
		}

		[Test]
		public void MatchVerifications_invalid_regex_should_set_success_to_false()
		{
			// Arrange
			var sessionVariables = new CapturedVariableProvider();
			var matcher = new AssertionsMatcher(sessionVariables);

			var verifications = new List<Assertion>();
			verifications.Add(new Assertion("dodgy regex", "((*)", AssertionType.Positive, AssertionMethod.Regex));

			string content = "<p>Some content here</p>";

			// Act
			List<Assertion> results = matcher.MatchVerifications(verifications, content);

			// Assert
			Assert.That(results.Count, Is.EqualTo(1));
			Assert.That(results[0].Success, Is.False);
		}

		[Test]
		public void MatchVerifications_should_return_veriftype_positives_in_list()
		{
			// Arrange
			var sessionVariables = new CapturedVariableProvider();
			var matcher = new AssertionsMatcher(sessionVariables);

			var verifications = new List<Assertion>();
			verifications.Add(new Assertion("p1", "a regex", AssertionType.Positive, AssertionMethod.Regex));
			verifications.Add(new Assertion("p2", "another regex", AssertionType.Positive, AssertionMethod.Regex));
			verifications.Add(new Assertion("n1", "one more regex", AssertionType.Negative, AssertionMethod.Regex));


			string content = "<p>whatever</p>";

			// Act
			List<Assertion> results = matcher.MatchVerifications(verifications, content);

			// Assert
			Assert.That(results.Count, Is.EqualTo(3));
		}

		[Test]
		public void MatchVerifications_should_match_text_in_content()
		{
			// Arrange
			var sessionVariables = new CapturedVariableProvider();
			var matcher = new AssertionsMatcher(sessionVariables);

			var verifications = new List<Assertion>();
			verifications.Add(new Assertion("desc1","content here", AssertionType.Positive, AssertionMethod.Regex));
			verifications.Add(new Assertion("desc2", "bad regex", AssertionType.Positive, AssertionMethod.Regex));

			string content = "<p>Some content here</p>";

			// Act
			List<Assertion> results = matcher.MatchVerifications(verifications, content);

			// Assert
			Assert.That(results.Count, Is.EqualTo(2));
			Assert.That(results[0].Success, Is.True);
			Assert.That(results[0].Description, Is.EqualTo("desc1"));
			Assert.That(results[0].Value, Is.EqualTo("content here"));

			Assert.That(results[1].Success, Is.False);
			Assert.That(results[1].Description, Is.EqualTo("desc2"));
			Assert.That(results[1].Value, Is.EqualTo("bad regex"));
		}

		[Test]
		public void MatchVerifications_should_not_match_text_that_is_not_in_content()
		{
			// Arrange
			var sessionVariables = new CapturedVariableProvider();
			var matcher = new AssertionsMatcher(sessionVariables);

			var verifications = new List<Assertion>();
			verifications.Add(new Assertion("desc1", "this isnt in the text", AssertionType.Negative, AssertionMethod.Regex));
			verifications.Add(new Assertion("desc2", "content here", AssertionType.Negative, AssertionMethod.Regex));

			string content = "<p>Some content here</p>";

			// Act
			List<Assertion> results = matcher.MatchVerifications(verifications, content);

			// Assert
			Assert.That(results.Count, Is.EqualTo(2));
			Assert.That(results[0].Success, Is.True);
			Assert.That(results[0].Description, Is.EqualTo("desc1"));
			Assert.That(results[0].Value, Is.EqualTo("this isnt in the text"));

			Assert.That(results[1].Success, Is.False);
			Assert.That(results[1].Description, Is.EqualTo("desc2"));
			Assert.That(results[1].Value, Is.EqualTo("content here"));
		}

		[Test]
		public void MatchVerifications_should_replace_variables_in_regex()
		{
			// Arrange
			var sessionVariables = new CapturedVariableProvider();
			sessionVariables.AddOrUpdateVariable("password", "tedx123");

			var matcher = new AssertionsMatcher(sessionVariables);

			var verifications = new List<Assertion>();
			verifications.Add(new Assertion("desc1", "({password})", AssertionType.Positive, AssertionMethod.Regex));

			string content = "<p>The password is tedx123</p>";

			// Act
			List<Assertion> results = matcher.MatchVerifications(verifications, content);

			// Assert
			Assert.That(results.Count, Is.EqualTo(1));
			Assert.That(results[0].Success, Is.True);
			Assert.That(results[0].Description, Is.EqualTo("desc1"));
			Assert.That(results[0].Value, Is.EqualTo("({password})"));
			Assert.That(results[0].TransformedRegex, Is.EqualTo("(tedx123)"));
		}
	}
}