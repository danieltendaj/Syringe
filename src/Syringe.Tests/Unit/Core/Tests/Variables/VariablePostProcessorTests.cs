using NUnit.Framework;
using Syringe.Core.Tests.Variables;

namespace Syringe.Tests.Unit.Core.Tests.Variables
{
    [TestFixture]
    public class VariablePostProcessorTests
    {
        [Test]
        public void should_not_change_text_when_type_is_none()
        {
            // given
            var processorType = VariablePostProcessorType.None;
            const string inputText = "this&is&amp;really_";

            // when
            var processor = new VariablePostProcessor();
            string outputText = processor.Process(inputText, processorType);

            // then
            Assert.That(outputText, Is.EqualTo(inputText));
        }

        [TestCase("filename=test.json&amp;pageNumber=1&amp;", "filename=test.json&pageNumber=1&")]
        [TestCase("filename=test.json<MARK<is>my<lover>in_RL;", "filename=test.json<MARK<is>my<lover>in_RL;")]
        public void should_html_decode(string inputText, string expectedOutput)
        {
            // given
            var processorType = VariablePostProcessorType.HtmlDecode;

            // when
            var processor = new VariablePostProcessor();
            string outputText = processor.Process(inputText, processorType);

            // then
            Assert.That(outputText, Is.EqualTo(expectedOutput));
        }
    }
}