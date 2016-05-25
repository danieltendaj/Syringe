using System.IO;
using System.Linq;
using System.Net;
using NUnit.Framework;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Repositories.Json.Reader;
using Syringe.Core.Tests.Variables;

namespace Syringe.Tests.Integration.Core.Tests.Repositories.Json
{
    public class TestFileReaderTests
    {
        private readonly string JsonExamplesFolder = typeof(TestFileWriterTests).Namespace + ".JsonExamples.";

        [Test]
        public void should_read_json_and_deserialise_to_object()
        {
            // when
            var reader = new TestFileReader();
            var jsonFile = TestHelpers.ReadEmbeddedFile("full-test-file.json", JsonExamplesFolder);
            var stringReader = new StringReader(jsonFile);
            var result = reader.Read(stringReader);

            // then
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Tests.Count());

            var test = result.Tests.First();
            Assert.AreEqual("Some Test", test.Description);
            Assert.AreEqual("POST", test.Method);
            Assert.AreEqual("FML", test.Url);
            Assert.AreEqual("SOOOO MANY PROPERTIES, I am getting bored", test.PostBody);
            Assert.AreEqual(HttpStatusCode.BadRequest, test.ExpectedHttpStatusCode);

            Assert.AreEqual(1, test.Headers.Count);
            Assert.AreEqual("Some Key", test.Headers[0].Key);
            Assert.AreEqual("Some Value", test.Headers[0].Value);

            Assert.AreEqual(1, test.CapturedVariables.Count);
            Assert.AreEqual("Captured Var 1", test.CapturedVariables[0].Name);
            Assert.AreEqual("/w/t/SOMETHING", test.CapturedVariables[0].Regex);
            Assert.AreEqual(VariablePostProcessorType.HtmlDecode, test.CapturedVariables[0].PostProcessorType);

            Assert.AreEqual(1, test.Assertions.Count);
            Assert.AreEqual("I SHOULD DO A THING", test.Assertions[0].Description);
            Assert.AreEqual(AssertionMethod.CSQuery, test.Assertions[0].AssertionMethod);
            Assert.AreEqual("Awesome Value", test.Assertions[0].Value);
            Assert.AreEqual(AssertionType.Negative, test.Assertions[0].AssertionType);
            Assert.AreEqual(VariablePostProcessorType.HtmlDecode, test.CapturedVariables[0].PostProcessorType);

            Assert.AreEqual("Some script thing that Chris did", test.BeforeExecuteScript);

            Assert.AreEqual(2, result.Variables.Count);
            Assert.AreEqual("Variable 1", result.Variables[0].Name);
            Assert.AreEqual("Value 1", result.Variables[0].Value);
            Assert.AreEqual("Env1", result.Variables[0].Environment.Name);

            Assert.AreEqual("Variable 2", result.Variables[1].Name);
            Assert.AreEqual("Value 2", result.Variables[1].Value);
            Assert.AreEqual("Env2", result.Variables[1].Environment.Name);
        }
    }
}
