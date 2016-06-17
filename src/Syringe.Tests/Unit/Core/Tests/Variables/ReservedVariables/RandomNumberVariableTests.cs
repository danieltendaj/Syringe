using NUnit.Framework;
using Syringe.Core.Tests.Variables.ReservedVariables;

namespace Syringe.Tests.Unit.Core.Tests.Variables.ReservedVariables
{
    [TestFixture]
    public class RandomNumberVariableTests
    {
        [Test]
        public void should_return_expected_variable()
        {
            // given
            var variable = new RandomNumberVariable();

            // when
            var result = variable.CreateVariable();

            // then
            Assert.That(variable.Description, Is.EqualTo("Returns a random number each time it is used."));
            Assert.That(variable.Name, Is.EqualTo("_randomNumber"));
            Assert.That(result.Environment.Name, Is.EqualTo(string.Empty));
            Assert.That(result.Name, Is.EqualTo("_randomNumber"));
        }

        [Test]
        public void should_always_return_a_random_number()
        {
            // given
            var variable = new RandomNumberVariable();

            // when
            var var1 = variable.CreateVariable();
            var var2 = variable.CreateVariable();

            // then
            Assert.That(var1, Is.Not.EqualTo(var2));
            Assert.That(var1.Value, Is.Not.EqualTo(var2.Value));
        }
    }
}