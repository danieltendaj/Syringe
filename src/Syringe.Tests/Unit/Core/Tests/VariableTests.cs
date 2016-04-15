using NUnit.Framework;
using Syringe.Core.Tests;

namespace Syringe.Tests.Unit.Core.Tests
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
    }
}