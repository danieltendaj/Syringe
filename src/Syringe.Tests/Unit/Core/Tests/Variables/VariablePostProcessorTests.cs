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

        [TestCase("filename=test.json&pageNumber=1&", "filename=test.json&amp;pageNumber=1&amp;")]
        [TestCase("filename=test.json<MARK<is>my<lover>in_RL;", "filename=test.json&lt;MARK&lt;is&gt;my&lt;lover&gt;in_RL;")]
        public void should_html_encode(string inputText, string expectedOutput)
        {
            // given
            var processorType = VariablePostProcessorType.HtmlEncode;

            // when
            var processor = new VariablePostProcessor();
            string outputText = processor.Process(inputText, processorType);

            // then
            Assert.That(outputText, Is.EqualTo(expectedOutput));
        }

        [TestCase("filename%3dtest.json%26pageNumber%3d1%26", "filename=test.json&pageNumber=1&")]
        [TestCase("filename%3Dtest.json%3CMARK%3Cis%3Emy%3Clover%3Ein_RL%3B", "filename=test.json<MARK<is>my<lover>in_RL;")]
        public void should_url_decode(string inputText, string expectedOutput)
        {
            // given
            var processorType = VariablePostProcessorType.UrlDecode;

            // when
            var processor = new VariablePostProcessor();
            string outputText = processor.Process(inputText, processorType);

            // then
            Assert.That(outputText, Is.EqualTo(expectedOutput));
        }

        [TestCase("filename=test.json&pageNumber=1&", "filename%3Dtest.json%26pageNumber%3D1%26")]
        [TestCase("filename=test.json<MARK<is>my<lover>in_RL;", "filename%3Dtest.json%3CMARK%3Cis%3Emy%3Clover%3Ein_RL%3B")]
        public void should_url_encode(string inputText, string expectedOutput)
        {
            // given
            var processorType = VariablePostProcessorType.UrlEncode;

            // when
            var processor = new VariablePostProcessor();
            string outputText = processor.Process(inputText, processorType);

            // then
            Assert.That(outputText, Is.EqualTo(expectedOutput));
        }
    }
}