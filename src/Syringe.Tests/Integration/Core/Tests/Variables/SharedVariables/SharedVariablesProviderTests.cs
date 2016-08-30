using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Syringe.Core.Tests.Variables;
using Syringe.Core.Tests.Variables.SharedVariables;

namespace Syringe.Tests.Integration.Core.Tests.Variables.SharedVariables
{
    public class SharedVariablesProviderTests
    {
        private readonly string _sharedVariablesOutput = Path.Combine(Environment.CurrentDirectory, "shared-variable-example.json");
        
        [SetUp]
        public void Setup()
        {
            string jsonExamplesFolder = typeof (SharedVariablesProviderTests).Namespace + ".JsonExamples.";
            string jsonContents = TestHelpers.ReadEmbeddedFile("shared-variables-example.json", jsonExamplesFolder);
            File.WriteAllText(_sharedVariablesOutput, jsonContents);
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
            var provider = new SharedVariablesProvider(_sharedVariablesOutput);

            // when
            var variables = provider.ListSharedVariables();

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
    }
}