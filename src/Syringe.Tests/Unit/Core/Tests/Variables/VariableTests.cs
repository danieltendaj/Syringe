using NUnit.Framework;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Variables;

namespace Syringe.Tests.Unit.Core.Tests.Variables
{
    [TestFixture]
    public class VariableTests
    {
        [TestCase("dev", "dev")]
        [TestCase("dev", "DEV")]
        [TestCase("Production", "production")]
        [TestCase("", "production")]
        [TestCase(null, "production")]
        [TestCase("", "")] // not sure about this one
        [TestCase(null, null)] // not sure about this one
        public void should_match_environment(string variableEnvironment, string environmentToTest)
        {
            // given
            var variable = new Variable("some name", "some value", variableEnvironment);

            // when
            bool matched = variable.MatchesEnvironment(environmentToTest);

            // then
            Assert.That(matched, Is.True);
        }

        [TestCase("dev", "production")]
        [TestCase("dev", "PROD")]
        [TestCase("Production", "productionS")]
        public void should_not_match_environment(string variableEnvironment, string environmentToTest)
        {
            // given
            var variable = new Variable("some name", "some value", variableEnvironment);

            // when
            bool matched = variable.MatchesEnvironment(environmentToTest);

            // then
            Assert.That(matched, Is.False);
        }

        [TestCase("some name", "dev")]
        [TestCase("some NAME", "dev")]
        [TestCase("some NAME", "DEV")]
        [TestCase("some name", "DEV")]
        public void should_match_name_and_environment(string name, string environment)
        {
            // given
            var baseVariable = new Variable("some name", "some value", "dev");
            var variableToTest = new Variable(name, "another value", environment);

            // when
            bool matched = baseVariable.MatchesNameAndEnvironment(variableToTest);

            // then
            Assert.That(matched, Is.True);
        }

        [TestCase("a-different-name", "dev")]
        [TestCase("some name", "a-different-environment")]
        [TestCase("a-different-name", "a-different-environment")]
        [TestCase(null, null)]
        public void should_not_match_name_and_environment(string name, string environment)
        {
            // given
            var baseVariable = new Variable("some name", "some value", "dev");
            var variableToTest = name == null ? null : new Variable(name, "another value", environment);

            // when
            bool matched = baseVariable.MatchesNameAndEnvironment(variableToTest);

            // then
            Assert.That(matched, Is.False);
        }
    }
}