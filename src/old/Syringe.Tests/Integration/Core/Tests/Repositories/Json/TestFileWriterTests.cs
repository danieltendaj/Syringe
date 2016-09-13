using System.Collections.Generic;
using System.Net;
using NUnit.Framework;
using Syringe.Core.Environment;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Repositories.Json.Writer;
using Syringe.Core.Tests.Scripting;
using Syringe.Core.Tests.Variables;

namespace Syringe.Tests.Integration.Core.Tests.Repositories.Json
{
    [TestFixture]
    public class TestFileWriterTests
    {
        private readonly string _jsonExamplesFolder = typeof(TestFileWriterTests).Namespace + ".JsonExamples.";
        private readonly string _blackListText = "I SHOULD NOT EXIST";

        [Test]
        public void should_output_expected_json_when_writing_test_file()
        {
            // given
            var contract = new SerializationContract();
            var testFile = new TestFile
            {
                Filename = "I SHOULD ALSO NOT EXIST",
                Variables = new List<Variable>
                {
                    new Variable
                    {
                        Environment = new Environment
                        {
                            Name = "Env1",
                            Order = 1234
                        },
                        Name = "Variable 1",
                        Value = "Value 1"
                    },
                    new Variable
                    {
                        Environment = new Environment
                        {
                            Name = "Env2",
                            Order = 4321
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
                        AvailableVariables = new List<Variable> {new Variable { Name = _blackListText} },
                        Assertions = new List<Assertion>
                        {
                            new Assertion
                            {
                                Value = "Awesome Value",
                                AssertionMethod = AssertionMethod.CssSelector,
                                AssertionType = AssertionType.Negative,
                                Description = "I SHOULD DO A THING",
                                Log = _blackListText,
                                Success = true,
                                TransformedValue = _blackListText
                            }
                        },
                        Description = "Some Test",
                        ScriptSnippets = new ScriptSnippets()
						{
							BeforeExecuteFilename = "uploadfile.snippet"
						},
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
            string expectedJson = TestHelpers.ReadEmbeddedFile("full-test-file.json", _jsonExamplesFolder);
            Assert.That(result.Replace("\r\n", "\n"), Is.EqualTo(expectedJson.Replace("\r\n", "\n")));
            Assert.That(result, Is.Not.StringContaining(_blackListText));
        }
    }
}