using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Syringe.Core.Logging;

namespace Syringe.Tests.Unit.Core.Logging
{
	[TestFixture]
	public class SimpleLoggerTests
	{
		private SimpleLogger CreateSimpleLogger()
		{
			return new SimpleLogger();
		}

		[Test]
		public void GetLog_should_return_current_log_text()
		{
			// given
			SimpleLogger logger = CreateSimpleLogger();
			string expectedText = "a message";
			logger.Write(expectedText);

			// when
			string actualText = logger.GetLog();

			// then
			Assert.That(actualText, Is.EqualTo(expectedText));
		}

		[Test]
		public void Write_should_append_to_log_with_formatting()
		{
			// given
			SimpleLogger logger = CreateSimpleLogger();
			string expectedText = "a message item1 item2";

			// when
			logger.Write("a message {0} {1}", "item1", "item2");

			// then
			string actualText = logger.LogStringBuilder.ToString();
			Assert.That(actualText, Is.EqualTo(expectedText));
		}

		[Test]
		public void Write_should_swallow_bad_string_formatting()
		{
			// given
			SimpleLogger logger = CreateSimpleLogger();

			// when
			logger.Write("bad formatting {0} {1} {4}");

			// then
			string actualText = logger.LogStringBuilder.ToString();

			// stringbuilder still writes until it gets an exception
			Assert.That(actualText, Is.EqualTo("bad formatting ")); 
		}

		[Test]
		public void WriteLine_should_append_to_log_with_formatting_and_newline()
		{
			// given
			SimpleLogger logger = CreateSimpleLogger();
			string expectedText = "=> a message item1 item2" +Environment.NewLine;

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
			SimpleLogger logger = CreateSimpleLogger();
			string expectedText = "=> message\nSystem.Exception: exception message" + Environment.NewLine;

			// when
			logger.WriteLine(new Exception("exception message"), 0, "message");

			// then
			string actualText = logger.LogStringBuilder.ToString();
			Assert.That(actualText, Is.EqualTo(expectedText));
		}

		[Test]
		public void WriteIndentedLine_should_include_indentation()
		{
			// given
			SimpleLogger logger = CreateSimpleLogger();
			string expectedText = "  => foobar" + Environment.NewLine;

			// when
			logger.WriteIndentedLine("foobar");

			// then
			string actualText = logger.LogStringBuilder.ToString();
			Assert.That(actualText, Is.EqualTo(expectedText));
		}

		[Test]
		public void WriteDoubleIndentedLine_should_include_more_indentation()
		{
			// given
			SimpleLogger logger = CreateSimpleLogger();
			string expectedText = "    => foobar2" + Environment.NewLine;

			// when
			logger.WriteDoubleIndentedLine("foobar2");

			// then
			string actualText = logger.LogStringBuilder.ToString();
			Assert.That(actualText, Is.EqualTo(expectedText));
		}
	}
}
