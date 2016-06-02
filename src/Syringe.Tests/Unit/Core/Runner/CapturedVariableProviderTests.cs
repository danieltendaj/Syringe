using System.Collections.Generic;
using NUnit.Framework;
using Syringe.Core.Logging;
using Syringe.Core.Runner;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Variables;
using Syringe.Tests.Extensions;

namespace Syringe.Tests.Unit.Core.Runner
{
	public class CapturedVariableProviderTests
	{
		[SetUp]
		public void Setup()
		{
			TestHelpers.EnableLogging();
        }

		[Test]
		public void should_match_regex_groups_and_set_variable_names_and_values_to_matched_items()
		{
			// Arrange
			var parseResponses = new List<CapturedVariable>()
			{
				new CapturedVariable("var1", @"(\d+)"),
				new CapturedVariable("varFoo", "(<html.+?>)")
			};
			string content = "<html class='bootstrap'><p>Tap tap tap 123</p></html>";

			// Act
			List<Variable> variables = CapturedVariableProvider.MatchVariables(parseResponses, content, new SimpleLogger());

			// Assert
			Assert.That(variables.Count, Is.EqualTo(2));
			Assert.That(variables.ValueByName("var1"), Is.EqualTo("123"));
			Assert.That(variables.ValueByName("varFoo"), Is.EqualTo("<html class='bootstrap'>"));
		}

		[Test]
		public void should_set_value_to_empty_string_when_regex_is_not_matched()
		{
			// Arrange
			var parseResponses = new List<CapturedVariable>()
			{
				new CapturedVariable("var1", @"foo"),
				new CapturedVariable("var2", @"bar"),

			};
			string content = "<html>123 abc</html>";

			// Act
			List<Variable> variables = CapturedVariableProvider.MatchVariables(parseResponses, content, new SimpleLogger());

			// Assert
			Assert.That(variables.Count, Is.EqualTo(2));
			Assert.That(variables.ValueByName("var1"), Is.EqualTo(""));
			Assert.That(variables.ValueByName("var2"), Is.EqualTo(""));
		}

		[Test]
		public void should_set_value_to_empty_string_when_regex_is_invalid()
		{
			// Arrange
			var parseResponses = new List<CapturedVariable>()
			{
				new CapturedVariable("var1", @"(\d+)"),
				new CapturedVariable("var2", @"(() this is a bad regex?("),

			};
			string content = "<html>123 abc</html>";

			// Act
			List<Variable> variables = CapturedVariableProvider.MatchVariables(parseResponses, content, new SimpleLogger());

			// Assert
			Assert.That(variables.Count, Is.EqualTo(2));
			Assert.That(variables.ValueByName("var1"), Is.EqualTo("123"));
			Assert.That(variables.ValueByName("var2"), Is.EqualTo(""));
		}

		[Test]
		public void should_not_concatenate_multiple_matches_into_variable_value()
		{
			// Arrange
			var parseResponses = new List<CapturedVariable>()
			{
				new CapturedVariable("var1", @"(\d+)"),
			};
			string content = "<html>The number 3 and the number 4 combined make 7</html>";

			// Act
			List<Variable> variables = CapturedVariableProvider.MatchVariables(parseResponses, content, new SimpleLogger());

			// Assert
			Assert.That(variables.ValueByName("var1"), Is.EqualTo("3"));
		}
	}
}