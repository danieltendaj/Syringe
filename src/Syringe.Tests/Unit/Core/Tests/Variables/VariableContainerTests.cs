using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Syringe.Core.Tests.Variables;
using Syringe.Core.Tests.Variables.ReservedVariables;
using Syringe.Tests.StubsMocks;

namespace Syringe.Tests.Unit.Core.Tests.Variables
{
    [TestFixture]
    public class VariableContainerTests
    {
        private ReservedVariableProviderStub _reservedVariableProvider;
        private SharedVariablesProviderStub _sharedVariablesProvider;

        [SetUp]
        public void Setup()
        {
            _reservedVariableProvider = new ReservedVariableProviderStub();
            _sharedVariablesProvider = new SharedVariablesProviderStub();
        }

        [Test]
        public void should_add_and_return_variable()
        {
            // given
            const string environment = "YAY";
            var variableContainer = new VariableContainer(environment, _reservedVariableProvider, _sharedVariablesProvider);
            var variable = new Variable("my variable", "some value", environment);

            // when
            variableContainer.Add(variable);

            // then
            var result = variableContainer.First(x => x == variable);
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void should_not_add_or_return_variable_assigned_against_a_different_environment()
        {
            // given
            const string environment = "YAY";
            var variableContainer = new VariableContainer(environment, _reservedVariableProvider, _sharedVariablesProvider);
            var variable = new Variable("my variable", "some value", "NOT ME");

            // when
            variableContainer.Add(variable);

            // then
            var result = variableContainer.FirstOrDefault(x => x == variable);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void should_return_reserved_variable()
        {
            // given
            const string environment = "another-environment";
            var expectedVariable = new Variable("namey", "valuey", environment);
            var reservedVariable = new Mock<IReservedVariable>();
            reservedVariable
                .Setup(x => x.CreateVariable())
                .Returns(expectedVariable);

            _reservedVariableProvider.ListAvailableVariables_Value = new[] { reservedVariable.Object };

            // when
            var variableContainer = new VariableContainer(environment, _reservedVariableProvider, _sharedVariablesProvider);
            var result = variableContainer.FirstOrDefault(x => x == expectedVariable);

            // then
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void should_return_system_variable()
        {
            // given
            const string environment = "another-environment";
            var expectedVariable = new Variable("some-other-name", "jingle-jangle", environment);
            _sharedVariablesProvider.ListSharedVariables_Value = new IVariable[] { expectedVariable };

            // when
            var variableContainer = new VariableContainer(environment, _reservedVariableProvider, _sharedVariablesProvider);
            var result = variableContainer.FirstOrDefault(x => x == expectedVariable);

            // then
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void should_prioritise_custom_variable_over_other_variables()
        {
            // given
            const string name = "variables-are-super-awesome";
            const string environment = "my-env";
            _sharedVariablesProvider.ListSharedVariables_Value = new IVariable[]
            {
                new Variable("another-thing", "NOT ME", "something else"),
                new Variable(name, "NOT ME", "something else"),
                new Variable(name, "NOT ME", "another one"),
                new Variable(name, "NOT ME", environment),
                new Variable(name, string.Empty, environment),
                new Variable("HELLO", "NOT ME", "CARS"),
            };

            var variableContainer = new VariableContainer(environment, _reservedVariableProvider, _sharedVariablesProvider);
            var expectedVariable = new Variable(name, "expected value", environment);
            variableContainer.Add(expectedVariable);

            // when
            List<IVariable> variables = variableContainer.ToList();
            IVariable result = variables.First(x => x.Name == name && x.Environment.Name == environment);

            // then
            Assert.That(result, Is.EqualTo(expectedVariable));
        }
    }
}