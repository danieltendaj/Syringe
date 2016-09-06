using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Moq;
using NUnit.Framework;
using Syringe.Core.Configuration;
using Syringe.Core.Tests.Variables;
using Syringe.Core.Tests.Variables.SharedVariables;

namespace Syringe.Tests.Integration.Core.Tests.Variables.SharedVariables
{
    public class SharedVariablesProviderTests
    {
        private readonly string _sharedVariablesOutput = Path.Combine(Environment.CurrentDirectory, "shared-variable-example.json");
        private Mock<IConfigLocator> _configLocatorMock;

        [SetUp]
        public void Setup()
        {
            string jsonExamplesFolder = typeof(SharedVariablesProviderTests).Namespace + ".JsonExamples.";
            string jsonContents = TestHelpers.ReadEmbeddedFile("shared-variables-example.json", jsonExamplesFolder);
            File.WriteAllText(_sharedVariablesOutput, jsonContents);

            _configLocatorMock = new Mock<IConfigLocator>();
            _configLocatorMock
                .Setup(x => x.ResolveConfigFile("shared-variables.json"))
                .Returns(_sharedVariablesOutput);
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(_sharedVariablesOutput))
            {
                File.Delete(_sharedVariablesOutput);
            }
        }

        [Test]
        public void should_read_shared_variables_as_expected()
        {
            // given
            var provider = new SharedVariablesProvider(_configLocatorMock.Object);

            // when
            IEnumerable<IVariable> variables = provider.ListSharedVariables();

            // then
            IVariable variable1 = variables.FirstOrDefault(x => x.Name == "test-name");
            Assert.That(variable1, Is.Not.Null);
            Assert.That(variable1.Value, Is.EqualTo("test-value"));
            Assert.That(variable1.Environment.Name, Is.EqualTo("Development"));

            IVariable variable2 = variables.FirstOrDefault(x => x.Name == "some-ther-name");
            Assert.That(variable2, Is.Not.Null);
            Assert.That(variable2.Value, Is.EqualTo("something else"));
            Assert.That(variable2.Environment.Name, Is.EqualTo("UAT"));
        }

        [Test]
        public void should_return_empty_variables_when_config_is_missing()
        {
            // given
            var provider = new SharedVariablesProvider(_configLocatorMock.Object);
            _configLocatorMock
                .Setup(x => x.ResolveConfigFile("shared-variables.json"))
                .Throws<FileNotFoundException>();

            // when
            IEnumerable<IVariable> variables = provider.ListSharedVariables();

            // then
            Assert.That(variables, Is.Empty);
        }
    }
}