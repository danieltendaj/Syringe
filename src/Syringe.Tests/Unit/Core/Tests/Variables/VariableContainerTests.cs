using System.Linq;
using Moq;
using NUnit.Framework;
using Syringe.Core.Tests.Variables;
using Syringe.Core.Tests.Variables.ReservedVariables;

namespace Syringe.Tests.Unit.Core.Tests.Variables
{
    [TestFixture]
    public class VariableContainerTests
    {
        private Mock<IReservedVariableProvider> _reservedVariableProvider;
        private VariableContainer _variableContainer;

        [SetUp]
        public void Setup()
        {
            _reservedVariableProvider = new Mock<IReservedVariableProvider>();
            _variableContainer = new VariableContainer(_reservedVariableProvider.Object);
        }

        [Test]
        public void should_add_and_return_variable()
        {
            // given
            var variable = new Variable();

            // when
            _variableContainer.Add(variable);

            // then
            var result = _variableContainer.First(x => x == variable);
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void should_return_reserved_variable()
        {
            // given
            var expectedVariable = new Variable("namey", "valuey", "environy");
            var reservedVariable = new Mock<IReservedVariable>();
            reservedVariable
                .Setup(x => x.CreateVariable())
                .Returns(expectedVariable);

            _reservedVariableProvider
                .Setup(x => x.ListAvailableVariables())
                .Returns(new[] { reservedVariable.Object });

            // when
            var result = _variableContainer.FirstOrDefault(x => x == expectedVariable);

            // then
            Assert.That(result, Is.Not.Null);
        }
    }
}