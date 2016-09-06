using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Repositories;
using Syringe.Core.Tests.Variables;
using Syringe.Service.Parallel;

namespace Syringe.Tests.Unit.Service.Parallel
{
    [TestFixture]
    public class TestFileAssemblerTests
    {
        [Test]
        public void should_return_expected_test_given_filename()
        {
            // given
            const string filename = "abc123.json";

            var expectedTestFile = new TestFile();
            var testFileRepository = new Mock<ITestRepository>();
            testFileRepository
                .Setup(x => x.GetTestFile(filename))
                .Returns(expectedTestFile);

            // when
            var assembler = new TestFileAssembler(testFileRepository.Object);
            var testFile = assembler.AssembleTestFile(filename, null);

            // then
            Assert.That(testFile, Is.EqualTo(expectedTestFile));
        }

        [TestCase("abc123.json")]
        [TestCase("test/folder/321cba.json")]
        public void should_populate_test_file_with_variable_overrides(string filename)
        {
            // given
            const string environment = "me-iz-a-env-yeah";

            var expectedTestFile = new TestFile();
            var testFileRepository = new Mock<ITestRepository>();
            testFileRepository
                .Setup(x => x.GetTestFile(filename))
                .Returns(expectedTestFile);

            // when
            var assembler = new TestFileAssembler(testFileRepository.Object);
            var testFile = assembler.AssembleTestFile(filename + "?my-var-1=abc&my-var-2=cba", environment);

            // then
            Assert.That(testFile.Variables.Count, Is.EqualTo(2));
            var var1 = testFile.Variables.First(x => x.Name == "my-var-1");
            Assert.That(var1.Value, Is.EqualTo("abc"));
            Assert.That(var1.Environment.Name, Is.EqualTo(environment));

            var var2 = testFile.Variables.First(x => x.Name == "my-var-2");
            Assert.That(var2.Value, Is.EqualTo("cba"));
            Assert.That(var2.Environment.Name, Is.EqualTo(environment));
        }

        [Test]
        public void should_override_existing_variable_overrides_if_they_exist()
        {
            // given
            const string filename = "abc123.json";
            const string environment = "me-iz-another-env-yeah";

            var expectedTestFile = new TestFile
            {
                Variables = new List<Variable>
                {
                    new Variable("my-var-2", "original-value", "some-env"),
                    new Variable("existing-var", "doobeedoo", "hi")
                }
            };
            var testFileRepository = new Mock<ITestRepository>();
            testFileRepository
                .Setup(x => x.GetTestFile(filename))
                .Returns(expectedTestFile);

            // when
            var assembler = new TestFileAssembler(testFileRepository.Object);
            var testFile = assembler.AssembleTestFile(filename + "?my-var-1=abc&my-var-2=cba", environment);

            // then
            Assert.That(testFile.Variables.Count, Is.EqualTo(3));
            var var1 = testFile.Variables.First(x => x.Name == "my-var-1");
            Assert.That(var1.Value, Is.EqualTo("abc"));
            Assert.That(var1.Environment.Name, Is.EqualTo(environment));

            var var2 = testFile.Variables.First(x => x.Name == "my-var-2");
            Assert.That(var2.Value, Is.EqualTo("cba"));
            Assert.That(var2.Environment.Name, Is.EqualTo(environment));

            var existingVar = testFile.Variables.First(x => x.Name == "existing-var");
            Assert.That(existingVar.Value, Is.EqualTo("doobeedoo"));
            Assert.That(existingVar.Environment.Name, Is.EqualTo("hi"));
        }

        [Test]
        public void should_return_null_if_test_file_was_not_found()
        {
            // given
            const string filename = "abc123.json";

            var expectedTestFile = new TestFile();
            var testFileRepository = new Mock<ITestRepository>();

            // when
            var assembler = new TestFileAssembler(testFileRepository.Object);
            var testFile = assembler.AssembleTestFile(filename + "?my-var-1=abc&my-var-2=cba", null);

            // then
            Assert.That(testFile, Is.Null);
        }
    }
}