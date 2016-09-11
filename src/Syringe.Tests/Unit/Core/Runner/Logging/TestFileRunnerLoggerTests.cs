using System;
using NUnit.Framework;
using Syringe.Core.Runner.Logging;

namespace Syringe.Tests.Unit.Core.Runner.Logging
{
	[TestFixture]
	public class TestFileRunnerLoggerTests
	{
		private TestFileRunnerLogger CreateLogger()
		{
			return new TestFileRunnerLogger();
		}

	    private string ExpectedMessage(string message)
	    {
            string now = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
	        return $"{now}  |  {message}";
	    }

		[Test]
		public void GetLog_should_return_current_log_text()
		{
			// given
			TestFileRunnerLogger logger = CreateLogger();
			string expectedText = ExpectedMessage("a message");
			logger.Write("a message");

			// when
			string actualText = logger.GetLog();

			// then
			Assert.That(actualText, Is.EqualTo(expectedText));
		}

		[Test]
		public void Write_should_append_to_log_with_formatting()
		{
			// given
			TestFileRunnerLogger logger = CreateLogger();
			string expectedText = ExpectedMessage("a message item1 item2");

			// when
			logger.Write("a message {0} {1}", "item1", "item2");

			// then
			string actualText = logger.LogStringBuilder.ToString();
			Assert.That(actualText, Is.EqualTo(expectedText));
		}

        [Test]
        public void AppendTextLine_should_not_add_timestamp()
        {
            // given
            TestFileRunnerLogger logger = CreateLogger();
            string expectedText = "a message" +Environment.NewLine;

            // when
            logger.AppendTextLine("a message");

            // then
            string actualText = logger.LogStringBuilder.ToString();
            Assert.That(actualText, Is.EqualTo(expectedText));
        }

        [Test]
		public void Write_should_swallow_bad_string_formatting()
		{
			// given
			TestFileRunnerLogger logger = CreateLogger();
            string expectedText = ExpectedMessage("Logger caught a formatting exception. Message: 'bad formatting {0} {1} {4}' args.length: 1");

            // when
            logger.Write("bad formatting {0} {1} {4}", 1);

			// then
			string actualText = logger.LogStringBuilder.ToString();

			// stringbuilder still writes until it gets an exception
			Assert.That(actualText, Is.EqualTo(expectedText)); 
		}

		[Test]
		public void WriteLine_should_append_to_log_with_formatting_and_newline()
		{
			// given
			TestFileRunnerLogger logger = CreateLogger();
			string expectedText = ExpectedMessage("a message item1 item2" +Environment.NewLine);

			// when
			logger.WriteLine("a message {0} {1}", "item1", "item2");

			// then
			string actualText = logger.LogStringBuilder.ToString();
			Assert.That(actualText, Is.EqualTo(expectedText));
		}

		[Test]
		public void WriteLine_should_append_exception_type_and_message()
		{
			// given
			TestFileRunnerLogger logger = CreateLogger();
			string expectedText = ExpectedMessage(string.Format("message{0}System.Exception: exception message{0}", Environment.NewLine));

			// when
			logger.WriteLine(new Exception("exception message"), "message");

			// then
			string actualText = logger.LogStringBuilder.ToString();
			Assert.That(actualText, Is.EqualTo(expectedText));
		}
	}
}
