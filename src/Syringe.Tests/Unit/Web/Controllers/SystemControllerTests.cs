using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using Syringe.Core.Configuration;
using Syringe.Core.Services;
using Syringe.Core.Tests.Variables.Encryption;
using Syringe.Web.Controllers;
using Syringe.Web.Models;

namespace Syringe.Tests.Unit.Web.Controllers
{
    [TestFixture]
    public class SystemControllerTests
    {
        private Mock<IVariableEncryptor> _variableEncryptorMock;
        private Mock<IConfigurationService> _configurationServiceMock;
        private SystemController _controller;

        [SetUp]
        public void Setup()
        {
            _variableEncryptorMock = new Mock<IVariableEncryptor>();
            _configurationServiceMock = new Mock<IConfigurationService>();
            _controller = new SystemController(_variableEncryptorMock.Object, _configurationServiceMock.Object);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void EncryptData_should_be_enabled_when_encryption_key_is_given(bool encryptionKeyEntered)
        {
            // given
            var configurationMock = new Mock<IConfiguration>();
            configurationMock
                .Setup(x => x.EncryptionKey)
                .Returns(encryptionKeyEntered ? "key" : null);
            _configurationServiceMock
                .Setup(x => x.GetConfiguration())
                .Returns(configurationMock.Object);

            // when
            var result = _controller.EncryptData() as ViewResult;

            // then
            Assert.That(result.ViewName, Is.EqualTo("EncryptData"));

            var model = result.Model as EncryptedDataViewModel;
            Assert.That(model.IsEnabled, Is.EqualTo(encryptionKeyEntered));
        }

        [TestCase(null)]
        [TestCase("")]
        public void EncryptData_should_not_encrypt_data_when_no_data_is_given(string variableValue)
        {
            // given

            // when
            var result = _controller.EncryptData(variableValue) as ViewResult;

            // then
            Assert.That(result.ViewName, Is.EqualTo("EncryptData"));

            var model = result.Model as EncryptedDataViewModel;
            Assert.That(model.IsEnabled, Is.True);
            Assert.That(model.EncryptedValue, Is.EqualTo(string.Empty));
            Assert.That(model.PlainValue, Is.EqualTo(string.Empty));

            _variableEncryptorMock
                .Verify(x => x.Encrypt(It.IsAny<string>(), true), Times.Never);
        }

        [Test]
        public void EncryptData_should_encrypt_data()
        {
            // given
            const string variableValue = "POLARISE THE HULL PLATING";
            const string expectedEncryptedValue = "£$%^&*(DFGHJK";
            _variableEncryptorMock
                .Setup(x => x.Encrypt(variableValue, true))
                .Returns(expectedEncryptedValue);

            // when
            var result = _controller.EncryptData(variableValue) as ViewResult;

            // then
            Assert.That(result.ViewName, Is.EqualTo("EncryptData"));

            var model = result.Model as EncryptedDataViewModel;
            Assert.That(model.IsEnabled, Is.True);
            Assert.That(model.EncryptedValue, Is.EqualTo(expectedEncryptedValue));
            Assert.That(model.PlainValue, Is.EqualTo(variableValue));
        }
    }
}