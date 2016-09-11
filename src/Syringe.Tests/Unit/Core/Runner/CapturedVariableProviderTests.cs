using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Syringe.Core.Runner;
using Syringe.Core.Runner.Logging;
using Syringe.Core.Tests.Variables;
using Syringe.Core.Tests.Variables.Encryption;
using Syringe.Tests.Extensions;
using Syringe.Tests.StubsMocks;

namespace Syringe.Tests.Unit.Core.Runner
{
	public class SessionVariablesTests
	{
		private const string _devEnvironment = "DEV";
		private const string _prodEnvironment = "PROD";
		private VariableContainerStub _variableContainer;

		[SetUp]
		public void Setup()
		{
			_variableContainer = new VariableContainerStub();
		}

		[Test]
		public void matchvariables_should_match_regex_groups_and_set_variable_names_and_values_to_matched_items()
		{
			// given
			var parseResponses = new List<CapturedVariable>()
			{
				new CapturedVariable("var1", @"(\d+)"),
				new CapturedVariable("varFoo", "(<html.+?>)")
			};
			string content = "<html class='bootstrap'><p>Tap tap tap 123</p></html>";

			// when
			List<Variable> variables = CapturedVariableProvider.MatchVariables(parseResponses, content, new TestFileRunnerLoggerMock());

			// then
			Assert.That(variables.Count, Is.EqualTo(2));
			Assert.That(variables.ValueByName("var1"), Is.EqualTo("123"));
			Assert.That(variables.ValueByName("varFoo"), Is.EqualTo("<html class='bootstrap'>"));
		}

		[Test]
		public void matchvariables_should_set_value_to_empty_string_when_regex_is_not_matched()
		{
			// given
			var parseResponses = new List<CapturedVariable>()
			{
				new CapturedVariable("var1", @"foo"),
				new CapturedVariable("var2", @"bar"),

			};
			string content = "<html>123 abc</html>";

			// when
			List<Variable> variables = CapturedVariableProvider.MatchVariables(parseResponses, content, new TestFileRunnerLoggerMock());

			// then
			Assert.That(variables.Count, Is.EqualTo(2));
			Assert.That(variables.ValueByName("var1"), Is.EqualTo(""));
			Assert.That(variables.ValueByName("var2"), Is.EqualTo(""));
		}

		[Test]
		public void matchvariables_should_set_value_to_empty_string_when_regex_is_invalid()
		{
			// given
			var parseResponses = new List<CapturedVariable>()
			{
				new CapturedVariable("var1", @"(\d+)"),
				new CapturedVariable("var2", @"(() this is a bad regex?("),
			};
			string content = "<html>123 abc</html>";

			// when
			List<Variable> variables = CapturedVariableProvider.MatchVariables(parseResponses, content, new TestFileRunnerLoggerMock());

			// then
			Assert.That(variables.Count, Is.EqualTo(2));
			Assert.That(variables.ValueByName("var1"), Is.EqualTo("123"));
			Assert.That(variables.ValueByName("var2"), Is.EqualTo(""));
		}

		[Test]
		public void matchvariables_should_not_concatenate_multiple_matches_into_variable_value()
		{
			// given
			var parseResponses = new List<CapturedVariable>()
			{
				new CapturedVariable("var1", @"(\d+)"),
			};
			string content = "<html>The number 3 and the number 4 combined make 7</html>";

			// when
			List<Variable> variables = CapturedVariableProvider.MatchVariables(parseResponses, content, new TestFileRunnerLogger());

			// then
			Assert.That(variables.ValueByName("var1"), Is.EqualTo("3"));
		}

		[Test]
		public void AddOrUpdateVariable_should_set_variable()
		{
			// given
			var sessionVariables = new CapturedVariableProvider(_variableContainer, _devEnvironment, new VariableEncryptorStub());
			var variable = new Variable("nano", "leaf", _devEnvironment);

			// whenS
			sessionVariables.AddOrUpdateVariable(variable);

			// then
			Assert.That(sessionVariables.GetVariableValue("nano"), Is.EqualTo("leaf"));
		}

		[Test]
		public void AddOrUpdateVariable_should_not_set_variable_when_in_different_environments()
		{
			// given
			var sessionVariables = new CapturedVariableProvider(_variableContainer, _devEnvironment, new VariableEncryptorStub());
			var variable = new Variable("nano", "leaf", _prodEnvironment);

			// when
			sessionVariables.AddOrUpdateVariable(variable);

			// then
			Assert.That(sessionVariables.GetVariableValue("nano"), Is.EqualTo(string.Empty));
		}

		[Test]
		public void AddOrUpdateVariable_should_update_variable_when_already_set()
		{
			// given
			var sessionVariables = new CapturedVariableProvider(_variableContainer, _devEnvironment, new VariableEncryptorStub());
			var variable1 = new Variable("nano", "leaf", _devEnvironment);
			var variable2 = new Variable("nano", "leaf2", _devEnvironment);

			// when
			sessionVariables.AddOrUpdateVariable(variable1);
			sessionVariables.AddOrUpdateVariable(variable2);

			// then
			Assert.That(sessionVariables.GetVariableValue("nano"), Is.EqualTo("leaf2"));
		}

		[Test]
		public void AddOrUpdateVariable_should_not_update_variable_when_already_set_and_in_different_environments()
		{
			// given
			var sessionVariables = new CapturedVariableProvider(_variableContainer, _prodEnvironment, new VariableEncryptorStub());
			var variable1 = new Variable("nano", "leaf", _prodEnvironment);
			var variable2 = new Variable("nano", "leaf2", _devEnvironment);

			// when
			sessionVariables.AddOrUpdateVariable(variable1);
			sessionVariables.AddOrUpdateVariable(variable2);

			// then
			Assert.That(sessionVariables.GetVariableValue("nano"), Is.EqualTo("leaf"));
		}

		[Test]
		public void AddOrUpdateVariables_should_set_variable()
		{
			// given
			var sessionVariables = new CapturedVariableProvider(_variableContainer, _devEnvironment, new VariableEncryptorStub());

			// when
			sessionVariables.AddOrUpdateVariables(new List<Variable>()
			{
				new Variable("nano", "leaf", _devEnvironment),
				new Variable("light", "bulb", _devEnvironment)
			});


			// then
			Assert.That(sessionVariables.GetVariableValue("nano"), Is.EqualTo("leaf"));
			Assert.That(sessionVariables.GetVariableValue("light"), Is.EqualTo("bulb"));
		}

		[Test]
		public void AddOrUpdateVariables_should_not_update_variable_when_already_set_and_original_is_set_as_default_variable()
		{
			// given
			var sessionVariables = new CapturedVariableProvider(_variableContainer, _devEnvironment, new VariableEncryptorStub());

			// when
			sessionVariables.AddOrUpdateVariables(new List<Variable>()
			{
				new Variable("nano", "leaf", string.Empty),
				new Variable("light", "bulb", string.Empty),
			});
			sessionVariables.AddOrUpdateVariables(new List<Variable>()
			{
				new Variable("nano", "leaf2", _devEnvironment),
				new Variable("light", "bulb2", _devEnvironment)
			});

			// then
			Assert.That(sessionVariables.GetVariableValue("nano"), Is.EqualTo("leaf"));
			Assert.That(sessionVariables.GetVariableValue("light"), Is.EqualTo("bulb"));
		}

		[Test]
		public void AddOrUpdateVariables_should_not_update_variable_when_setting_as_default_and_has_existing_variable_set_against_a_specific_environment()
		{
			// given
			var sessionVariables = new CapturedVariableProvider(_variableContainer, _devEnvironment, new VariableEncryptorStub());

			// when
			sessionVariables.AddOrUpdateVariables(new List<Variable>()
			{
				new Variable("nano", "leaf", _devEnvironment),
				new Variable("light", "bulb", _devEnvironment),
			});
			sessionVariables.AddOrUpdateVariables(new List<Variable>()
			{
				new Variable("nano", "leaf2", string.Empty),
				new Variable("light", "bulb2", string.Empty)
			});

			// then
			Assert.That(sessionVariables.GetVariableValue("nano"), Is.EqualTo("leaf"));
			Assert.That(sessionVariables.GetVariableValue("light"), Is.EqualTo("bulb"));
		}

		[Test]
		public void AddOrUpdateVariables_should_update_variable_when_both_existing_and_new_variable_have_environment_set()
		{
			// given
			var sessionVariables = new CapturedVariableProvider(_variableContainer, _devEnvironment, new VariableEncryptorStub());

			// when
			sessionVariables.AddOrUpdateVariables(new List<Variable>()
			{
				new Variable("nano", "leaf", _devEnvironment),
				new Variable("light", "bulb", _devEnvironment),
			});
			sessionVariables.AddOrUpdateVariables(new List<Variable>()
			{
				new Variable("nano", "leaf2", _devEnvironment),
				new Variable("light", "bulb2", _devEnvironment)
			});

			// then
			Assert.That(sessionVariables.GetVariableValue("nano"), Is.EqualTo("leaf2"));
			Assert.That(sessionVariables.GetVariableValue("light"), Is.EqualTo("bulb2"));
		}

		[Test]
		public void ReplacePlainTextVariablesIn_should_replace_all_variables()
		{
			// given
			var sessionVariables = new CapturedVariableProvider(_variableContainer, _devEnvironment, new VariableEncryptorStub());
			sessionVariables.AddOrUpdateVariable(new Variable("nano", "leaf", _devEnvironment));
			sessionVariables.AddOrUpdateVariable(new Variable("two", "ten", _devEnvironment));

			string template = "{nano} {dummy} {two}";
			string expectedText = "leaf {dummy} ten";

			// when
			string actualText = sessionVariables.ReplacePlainTextVariablesIn(template);

			// then
			Assert.That(actualText, Is.EqualTo(expectedText));
		}

		[Test]
		public void ReplacePlainTextVariablesIn_should_call_decrypt()
		{
			// given
			string variableValue = "leaf";

			var mock = new Mock<IVariableEncryptor>();
			mock.Setup(x => x.Decrypt(It.IsAny<string>()))
				.Returns(variableValue)
				.Verifiable("decrypt not called");

			var sessionVariables = new CapturedVariableProvider(_variableContainer, _devEnvironment, mock.Object);
			sessionVariables.AddOrUpdateVariable(new Variable("nano", variableValue, _devEnvironment));

			string template = "{nano}";

			// when
			string actualText = sessionVariables.ReplacePlainTextVariablesIn(template);

			// then
			mock.Verify(x => x.Decrypt(variableValue));
		}

		[Test]
		public void ReplaceVariablesIn_should_replace_all_variables_and_escape_regex_characters_in_values()
		{
			// given
			var sessionVariables = new CapturedVariableProvider(_variableContainer, _devEnvironment, new VariableEncryptorStub());
			sessionVariables.AddOrUpdateVariable(new Variable("nano", "$var leaf", _devEnvironment));
			sessionVariables.AddOrUpdateVariable(new Variable("two", "(.*?) [a-z] ^perlmagic", _devEnvironment));

			string template = "{nano} {dummy} {two}";
			string expectedText = @"\$var\ leaf {dummy} \(\.\*\?\)\ \[a-z]\ \^perlmagic";

			// when
			string actualText = sessionVariables.ReplaceVariablesIn(template);

			// then
			Assert.That(actualText, Is.EqualTo(expectedText));
		}

		[Test]
		public void ReplaceVariablesIn_should_call_decrypt()
		{
			// given
			string variableValue = "leaf";

			var mock = new Mock<IVariableEncryptor>();
			mock.Setup(x => x.Decrypt(It.IsAny<string>()))
				.Returns(variableValue)
				.Verifiable("decrypt not called");

			var sessionVariables = new CapturedVariableProvider(_variableContainer, _devEnvironment, mock.Object);
			sessionVariables.AddOrUpdateVariable(new Variable("nano", variableValue, _devEnvironment));

			string template = "{nano}";

			// when
			sessionVariables.ReplaceVariablesIn(template);

			// then
			mock.Verify(x => x.Decrypt(variableValue));
		}
	}
}