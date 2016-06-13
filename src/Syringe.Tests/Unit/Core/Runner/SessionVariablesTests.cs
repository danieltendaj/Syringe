using System.Collections.Generic;
using NUnit.Framework;
using Syringe.Core.Runner;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Variables;

namespace Syringe.Tests.Unit.Core.Runner
{
	public class SessionVariablesTests
    {
	    private const string _devEnvironment = "DEV";
	    private const string _prodEnvironment = "PROD";

        [Test]
        public void AddOrUpdateVariable_should_set_variable()
        {
            // Arrange
            var sessionVariables = new CapturedVariableProvider(_devEnvironment);
            var variable = new Variable("nano", "leaf", _devEnvironment);

            // Act
            sessionVariables.AddOrUpdateVariable(variable);

            // Assert
            Assert.That(sessionVariables.GetVariableValue("nano"), Is.EqualTo("leaf"));
        }

        [Test]
        public void AddOrUpdateVariable_should_not_set_variable_when_in_different_environments()
        {
            // Arrange
            var sessionVariables = new CapturedVariableProvider(_devEnvironment);
            var variable = new Variable("nano", "leaf", _prodEnvironment);

            // Act
            sessionVariables.AddOrUpdateVariable(variable);

            // Assert
            Assert.That(sessionVariables.GetVariableValue("nano"), Is.EqualTo(string.Empty));
        }

        [Test]
        public void AddOrUpdateVariable_should_update_variable_when_already_set()
        {
            // Arrange
            var sessionVariables = new CapturedVariableProvider(_devEnvironment);
            var variable1 = new Variable("nano", "leaf", _devEnvironment);
            var variable2 = new Variable("nano", "leaf2", _devEnvironment);

            // Act
            sessionVariables.AddOrUpdateVariable(variable1);
            sessionVariables.AddOrUpdateVariable(variable2);

            // Assert
            Assert.That(sessionVariables.GetVariableValue("nano"), Is.EqualTo("leaf2"));
        }

        [Test]
        public void AddOrUpdateVariable_should_not_update_variable_when_already_set_and_in_different_environments()
        {
            // Arrange
            var sessionVariables = new CapturedVariableProvider(_prodEnvironment);
            var variable1 = new Variable("nano", "leaf", _prodEnvironment);
            var variable2 = new Variable("nano", "leaf2", _devEnvironment);

            // Act
            sessionVariables.AddOrUpdateVariable(variable1);
            sessionVariables.AddOrUpdateVariable(variable2);

            // Assert
            Assert.That(sessionVariables.GetVariableValue("nano"), Is.EqualTo("leaf"));
        }

        [Test]
		public void AddOrUpdateVariables_should_set_variable()
		{
			// Arrange
			var sessionVariables = new CapturedVariableProvider(_devEnvironment);

			// Act
			sessionVariables.AddOrUpdateVariables(new List<Variable>()
			{
				new Variable("nano", "leaf", _devEnvironment),
				new Variable("light", "bulb", _devEnvironment)
			});


			// Assert
			Assert.That(sessionVariables.GetVariableValue("nano"), Is.EqualTo("leaf"));
			Assert.That(sessionVariables.GetVariableValue("light"), Is.EqualTo("bulb"));
        }

        [Test]
        public void AddOrUpdateVariables_should_update_variable_when_already_set_and_original_is_set_as_default_variable()
        {
            // Arrange
            var sessionVariables = new CapturedVariableProvider(_devEnvironment);

            // Act
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

            // Assert
            Assert.That(sessionVariables.GetVariableValue("nano"), Is.EqualTo("leaf2"));
            Assert.That(sessionVariables.GetVariableValue("light"), Is.EqualTo("bulb2"));
        }

        [Test]
        public void AddOrUpdateVariables_should_not_update_variable_when_setting_as_default_and_has_existing_variable_set_against_a_specific_environment()
        {
            // Arrange
            var sessionVariables = new CapturedVariableProvider(_devEnvironment);

            // Act
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

            // Assert
            Assert.That(sessionVariables.GetVariableValue("nano"), Is.EqualTo("leaf"));
            Assert.That(sessionVariables.GetVariableValue("light"), Is.EqualTo("bulb"));
        }

        [Test]
        public void AddOrUpdateVariables_should_update_variable_when_both_existing_and_new_variable_have_environment_set()
        {
            // Arrange
            var sessionVariables = new CapturedVariableProvider(_devEnvironment);

            // Act
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

            // Assert
            Assert.That(sessionVariables.GetVariableValue("nano"), Is.EqualTo("leaf2"));
            Assert.That(sessionVariables.GetVariableValue("light"), Is.EqualTo("bulb2"));
        }

        [Test]
		public void ReplacePlainTextVariablesIn_should_replace_all_variables()
		{
			// Arrange
			var sessionVariables = new CapturedVariableProvider(_devEnvironment);
			sessionVariables.AddOrUpdateVariable(new Variable("nano", "leaf", _devEnvironment));
			sessionVariables.AddOrUpdateVariable(new Variable("two", "ten", _devEnvironment));

			string template = "{nano} {dummy} {two}";
			string expectedText = "leaf {dummy} ten";

			// Act
			string actualText = sessionVariables.ReplacePlainTextVariablesIn(template);

			// Assert
			Assert.That(actualText, Is.EqualTo(expectedText));
		}

		[Test]
		public void ReplaceVariablesIn_should_replace_all_variables_and_escape_regex_characters_in_values()
		{
			// Arrange
			var sessionVariables = new CapturedVariableProvider(_devEnvironment);
			sessionVariables.AddOrUpdateVariable(new Variable("nano", "$var leaf", _devEnvironment));
			sessionVariables.AddOrUpdateVariable(new Variable("two", "(.*?) [a-z] ^perlmagic", _devEnvironment));

			string template = "{nano} {dummy} {two}";
			string expectedText = @"\$var\ leaf {dummy} \(\.\*\?\)\ \[a-z]\ \^perlmagic";

			// Act
			string actualText = sessionVariables.ReplaceVariablesIn(template);

			// Assert
			Assert.That(actualText, Is.EqualTo(expectedText));
		}
	}
}