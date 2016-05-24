using System.Collections.Generic;
using System.Net;
using NUnit.Framework;
using Syringe.Core.Environment;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Repositories.Json.Writer;
using Syringe.Core.Tests.Variables;

namespace Syringe.Tests.Integration.Core.Tests.Repositories.Json
{
    [TestFixture]
    public class TestFileWriterTests
    {
        private readonly string JsonExamplesFolder = typeof(TestFileWriterTests).Namespace + ".JsonExamples.";
        private readonly string BlackListText = "I SHOULD NOT EXIST";

        [Test]
        public void should_output_expected_json_when_writing_test_file()
        {
            // given
            var contract = new SerializationContract();
            var testFile = new TestFile
            {
                Environment = BlackListText,
                Filename = "I SHOULD ALSO NOT EXIST",
                Variables = new List<Variable>
                {
                    new Variable
                    {
                        Environment = new Environment
                        {
                            Name = "Env1",
                            Order = 1234,
                            Roles = new []{ new EnvironmentRole {Name = "HUH?", HostName = "Something host", Role = "King"} }
                        },
                        Name = "Variable 1",
                        Value = "Value 1"
                    },
                    new Variable
                    {
                        Environment = new Environment
                        {
                            Name = "Env2",
                            Order = 4321,
                            Roles = new [] { new EnvironmentRole { Name = "BURP?", HostName = "BRAP", Role = "Hemang & Dicks" }}
                        },
                        Name = "Variable 2",
                        Value = "Value 2"
                    }
                },
                Tests = new List<Test>
                {
                    new Test
                    {
                        Method = "POST",
                        AvailableVariables = new List<Variable> {new Variable { Name = BlackListText} },
                        Assertions = new List<Assertion>
                        {
                            new Assertion
                            {
                                Value = "Awesome Value",
                                AssertionMethod = AssertionMethod.CSQuery,
                                AssertionType = AssertionType.Negative,
                                Description = "I SHOULD DO A THING",
                                Log = BlackListText,
                                Success = true,
                                TransformedValue = BlackListText
                            }
                        },
                        Description = "Some Test",
                        BeforeExecuteScript = "Some script thing that Chris did",
                        CapturedVariables = new List<CapturedVariable>
                        {
                            new CapturedVariable { Name = "Captured Var 1", Regex = "/w/t/SOMETHING", PostProcessorType = VariablePostProcessorType.HtmlDecode}
                        },
                        ExpectedHttpStatusCode = HttpStatusCode.BadRequest,
                        Headers = new List<HeaderItem>
                        {
                            new HeaderItem { Key = "Some Key", Value = "Some Value" }
                        },
                        PostBody = "SOOOO MANY PROPERTIES, I am getting bored",
                        Url = "FML"
                    }
                }
            };

            // when
            var writer = new TestFileWriter(contract);
            string result = writer.Write(testFile);

            // then
            string expectedJson = TestHelpers.ReadEmbeddedFile("full-test-file.json", JsonExamplesFolder);
            Assert.That(result.Replace("\r\n", "\n"), Is.EqualTo(expectedJson.Replace("\r\n", "\n")));
            Assert.That(result, Is.Not.StringContaining(BlackListText));
        }

        [Test]
        public void write_should_add_variables()
        {
            // Arrange
            string expectedJson = TestHelpers.ReadEmbeddedFile("variables.xml", JsonExamplesFolder);

            TestFile testFile = CreateTestFile();
            testFile.Variables.Add(new Variable("name1", "value1", "env1"));
            testFile.Variables.Add(new Variable("name2", "value2", "env2"));
            TestFileWriter xmlWriter = CreateTestFileWriter();

            // Act
            string result = xmlWriter.Write(testFile);

            // Assert
            Assert.That(result.Replace("\r\n", "\n"), Is.EqualTo(expectedJson.Replace("\r\n", "\n")));
        }

        [Test]
        public void write_should_add_all_test_attributes()
        {
            // Arrange
            string expectedJson = TestHelpers.ReadEmbeddedFile("all-attributes.xml", JsonExamplesFolder);

            var test = new Test()
            {
                Description = "description",
                Url = "http://myserver",
                Method = "post",
                ExpectedHttpStatusCode = HttpStatusCode.Accepted,
            };
            TestFile testFile = CreateTestFile(test);
            TestFileWriter xmlWriter = CreateTestFileWriter();

            // Act
            string result = xmlWriter.Write(testFile);

            // Assert
            Assert.That(result.Replace("\r\n", "\n"), Is.EqualTo(expectedJson.Replace("\r\n", "\n")));
        }

        [Test]
        public void write_should_add_header_key_and_values_with_cdata_encoding_when_value_has_html_entities()
        {
            // Arrange
            string expectedJson = TestHelpers.ReadEmbeddedFile("headers.xml", JsonExamplesFolder);

            var test = new Test();
            test.AddHeader("key1", "value1");
            test.AddHeader("key2", "some <marvellous> HTML &&&&.");

            TestFile testFile = CreateTestFile(test);
            TestFileWriter xmlWriter = CreateTestFileWriter();

            // Act
            string result = xmlWriter.Write(testFile);

            // Assert
            Assert.That(result.Replace("\r\n", "\n"), Is.EqualTo(expectedJson.Replace("\r\n", "\n")));

        }

        [Test]
        public void write_should_add_postbody_with_cdata()
        {
            // Arrange
            string expectedJson = TestHelpers.ReadEmbeddedFile("postbody.xml", JsonExamplesFolder);

            var test = new Test();
            test.PostBody = "username=corey&password=welcome&myhtml=<body></body>";
            TestFile testFile = CreateTestFile(test);
            TestFileWriter xmlWriter = CreateTestFileWriter();

            // Act
            string result = xmlWriter.Write(testFile);

            // Assert
            Assert.That(result.Replace("\r\n", "\n"), Is.EqualTo(expectedJson.Replace("\r\n", "\n")));
        }

        [Test]
        public void write_should_add_capturedvariables()
        {
            // Arrange
            string expectedJson = TestHelpers.ReadEmbeddedFile("capturedvariables.xml", JsonExamplesFolder);

            var test = new Test();
            test.CapturedVariables.Add(new CapturedVariable("1", "here is (.*?) regex"));
            test.CapturedVariables.Add(new CapturedVariable("2", "plain text"));
            test.CapturedVariables.Add(new CapturedVariable("3", "This is encoded <test> &."));

            TestFile testFile = CreateTestFile(test);
            TestFileWriter xmlWriter = CreateTestFileWriter();

            // Act
            string result = xmlWriter.Write(testFile);

            // Assert
            Assert.That(result.Replace("\r\n", "\n"), Is.EqualTo(expectedJson.Replace("\r\n", "\n")));
        }

        [Test]
        public void write_should_add_verifications()
        {
            // Arrange
            string expectedJson = TestHelpers.ReadEmbeddedFile("assertions.xml", JsonExamplesFolder);

            var test = new Test();
            test.Assertions.Add(new Assertion("p90-1", "regex1", AssertionType.Positive, AssertionMethod.Regex));
            test.Assertions.Add(new Assertion("p90-2", "regex2", AssertionType.Positive, AssertionMethod.Regex));
            test.Assertions.Add(new Assertion("p90-3", "this 3rd positive needs CDATA as it has <html> & stuff in it (.*)", AssertionType.Positive, AssertionMethod.Regex));

            test.Assertions.Add(new Assertion("negev1", "csquery1", AssertionType.Negative, AssertionMethod.CSQuery));
            test.Assertions.Add(new Assertion("negev2", "csquery2", AssertionType.Negative, AssertionMethod.CSQuery));
            test.Assertions.Add(new Assertion("negev3", "this 3rd negative needs CDATA as it has <html> & stuff in it (.*)", AssertionType.Negative, AssertionMethod.CSQuery));

            TestFile testFile = CreateTestFile(test);
            TestFileWriter xmlWriter = CreateTestFileWriter();

            // Act
            string result = xmlWriter.Write(testFile);

            // Assert
            Assert.That(result.Replace("\r\n", "\n"), Is.EqualTo(expectedJson.Replace("\r\n", "\n")));
        }

        [Test]
        public void write_should_write_large_files()
        {
            // Arrange
            string expectedJson = TestHelpers.ReadEmbeddedFile("large-file.xml", JsonExamplesFolder);

            var testFile = new TestFile();
            var list = new List<Test>();

            for (int i = 0; i < 100; i++)
            {
                var test = new Test()
                {
                    Description = "description" + i,
                    Url = "http://myserver",
                    Method = "post",
                    ExpectedHttpStatusCode = HttpStatusCode.Accepted,
                };

                list.Add(test);
            }

            testFile.Tests = list;
            TestFileWriter xmlWriter = CreateTestFileWriter();

            // Act
            string result = xmlWriter.Write(testFile);

            // Assert
            Assert.That(result.Replace("\r\n", "\n"), Is.EqualTo(expectedJson.Replace("\r\n", "\n")));
        }

        [Test]
        public void write_should_add_beforeExecuteScript_with_cdata()
        {
            // Arrange
            string expectedJson = TestHelpers.ReadEmbeddedFile("beforeExecuteScript.xml", JsonExamplesFolder);

            var test = new Test();
            test.BeforeExecuteScript = "using Amazinglib;string name = \"<singletest>\";";
            TestFile testFile = CreateTestFile(test);
            TestFileWriter xmlWriter = CreateTestFileWriter();

            // Act
            string result = xmlWriter.Write(testFile);

            // Assert
            Assert.That(result.Replace("\r\n", "\n"), Is.EqualTo(expectedJson.Replace("\r\n", "\n")));
        }

        private TestFileWriter CreateTestFileWriter()
        {
            return new TestFileWriter(new SerializationContract());
        }

        private TestFile CreateTestFile()
        {
            return new TestFile();
        }

        private TestFile CreateTestFile(Test test)
        {
            var testFile = new TestFile();
            var list = new List<Test>();
            list.Add(test);
            testFile.Tests = list;

            return testFile;
        }
    }
}