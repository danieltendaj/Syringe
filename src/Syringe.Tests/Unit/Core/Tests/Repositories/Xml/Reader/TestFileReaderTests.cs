using System.IO;
using System.Linq;
using System.Net;
using NUnit.Framework;
using Syringe.Core.Exceptions;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Repositories;
using Syringe.Core.Tests.Repositories.Xml.Reader;
using Syringe.Core.Tests.Variables;

namespace Syringe.Tests.Unit.Core.Tests.Repositories.Xml.Reader
{
	public class TestFileReaderTests
	{
		public virtual string XmlExamplesFolder => typeof(TestFileReaderTests).Namespace + ".XmlExamples.";

		protected virtual ITestFileReader GetTestFileReader()
        {
			return new TestFileReader();
        }

        protected string GetSingleTestExample()
		{
			return TestHelpers.ReadEmbeddedFile("single-test.xml", XmlExamplesFolder); 
		}

        protected string GetFullExample()
		{
			return TestHelpers.ReadEmbeddedFile("full.xml", XmlExamplesFolder); 
		}

		[Test]
		public void Read_should_throw_exception_when_tests_element_is_missing()
		{
			// Arrange
			string xml = @"<?xml version=""1.0"" encoding=""utf-8"" ?><something></something>";
			var stringReader = new StringReader(xml);
			var testFileReader = GetTestFileReader();

			// Act + Assert
			Assert.Throws<TestException>(() => testFileReader.Read(stringReader));
		}

		[Test]
		public void Read_should_parse_test_vars()
		{
			// Arrange
			string xml = GetFullExample();
			var stringReader = new StringReader(xml);
			var testFileReader = GetTestFileReader();

			// Act
			TestFile testFile = testFileReader.Read(stringReader);

			// Assert
            Assert.That(testFile.Variables.Count, Is.EqualTo(5));

            Variable variable = testFile.Variables[0];
            Assert.That(variable.Name, Is.EqualTo("LOGIN_URL"));
            Assert.That(variable.Value, Is.EqualTo("http://myserver/login.php"));
            Assert.That(variable.Environment.Name, Is.EqualTo("DevTeam1"));

            variable = testFile.Variables[1];
            Assert.That(variable.Name, Is.EqualTo("LOGIN_URL"));
            Assert.That(variable.Value, Is.EqualTo("http://myserver2/login.php"));
            Assert.That(variable.Environment.Name, Is.EqualTo("DevTeam2"));

            variable = testFile.Variables[2];
            Assert.That(variable.Name, Is.EqualTo("LOGIN1"));
            Assert.That(variable.Value, Is.EqualTo("bob"));
            Assert.That(variable.Environment.Name, Is.EqualTo("DevTeam2"));

            variable = testFile.Variables[3];
            Assert.That(variable.Name, Is.EqualTo("PASSWD1"));
            Assert.That(variable.Value, Is.EqualTo("sponge"));
            Assert.That(variable.Environment.Name, Is.EqualTo("DevTeam1"));

            variable = testFile.Variables[4];
            Assert.That(variable.Name, Is.EqualTo("SUCCESSFULL_TEST_TEXT"));
            Assert.That(variable.Value, Is.EqualTo("Welcome Bob"));
            Assert.That(variable.Environment.Name, Is.EqualTo("DevTeam2"));
		}

		[Test]
		public void Read_should_parse_description_attribute()
		{
			// Arrange
			string xml = GetSingleTestExample();
			var stringReader = new StringReader(xml);
			var testFileReader = GetTestFileReader();

			// Act
			TestFile testFile = testFileReader.Read(stringReader);

			// Assert
			Test test = testFile.Tests.First();

			Assert.That(test.Description, Is.EqualTo("description"));
		}

		[Test]
		public void Read_should_parse_method_attribute()
		{
			// Arrange
			string xml = GetSingleTestExample();
			var stringReader = new StringReader(xml);
			var testFileReader = GetTestFileReader();

			// Act
			TestFile testFile = testFileReader.Read(stringReader);

			// Assert
			Test test = testFile.Tests.First();

			Assert.That(test.Method, Is.EqualTo("post"));
		}

		[Test]
		public void Read_should_default_method_property_to_get_when_it_doesnt_exist()
		{
			// Arrange
			string xml = GetSingleTestExample();
			xml = xml.Replace(@"method=""post""", "");

			var stringReader = new StringReader(xml);
			var testFileReader = GetTestFileReader();

			// Act
			TestFile testFile = testFileReader.Read(stringReader);

			// Assert
			Test test = testFile.Tests.First();

			Assert.That(test.Method, Is.EqualTo("get"));
		}

		[Test]
		public void Read_should_parse_url_attribute()
		{
			// Arrange
			string xml = GetSingleTestExample();
			var stringReader = new StringReader(xml);
			var testFileReader = GetTestFileReader();

			// Act
			TestFile testFile = testFileReader.Read(stringReader);

			// Assert
			Test test = testFile.Tests.First();
			Assert.That(test.Url, Is.EqualTo("http://myserver"));
		}

		[Test]
		public void Read_should_throw_exception_when_url_attribute_doesnt_exist()
		{
			// Arrange
			string xml = GetSingleTestExample();
			xml = xml.Replace(@"url=""http://myserver""", "");

			var stringReader = new StringReader(xml);
			var testFileReader = GetTestFileReader();

			// Act + Assert
			Assert.Throws<TestException>(() => testFileReader.Read(stringReader));
		}

		[Test]
		public void Read_should_parse_postbody_element()
		{
			// Arrange
			string xml = GetSingleTestExample();
			var stringReader = new StringReader(xml);
			var testFileReader = GetTestFileReader();

			// Act
			TestFile testFile = testFileReader.Read(stringReader);

			// Assert
			Test test = testFile.Tests.First();
			Assert.That(test.PostBody, Is.EqualTo("username=corey&password=welcome"));
		}

		[Test]
		public void Read_should_parse_responsecode_attribute()
		{
			// Arrange
			string xml = GetSingleTestExample();
			var stringReader = new StringReader(xml);
			var testFileReader = GetTestFileReader();
			var expectedCode = HttpStatusCode.NotFound;

			// Act
			TestFile testFile = testFileReader.Read(stringReader);

			// Assert
			Test test = testFile.Tests.First();
			Assert.That(test.ExpectedHttpStatusCode, Is.EqualTo(expectedCode));
		}

		[Test]
		public void Read_should_use_default_responsecode_value_when_attribute_is_empty()
		{
			// Arrange
			string xml = GetSingleTestExample();
			xml = xml.Replace("expectedhttpstatuscode=\"404\"", "");

			var stringReader = new StringReader(xml);
			var testFileReader = GetTestFileReader();
			var expectedCode = HttpStatusCode.OK;

			// Act
			TestFile testFile = testFileReader.Read(stringReader);

			// Assert
			Test test = testFile.Tests.First();
			Assert.That(test.ExpectedHttpStatusCode, Is.EqualTo(expectedCode));
		}

		[Test]
		public void Read_should_use_default_httpcode_value_when_attribute_is_invalid_code()
		{
			// Arrange
			string xml = GetSingleTestExample();
			xml = xml.Replace("expectedhttpstatuscode=\"404\"", "expectedhttpstatuscode=\"20000000\"");

			var stringReader = new StringReader(xml);
			var testFileReader = GetTestFileReader();
			var expectedCode = HttpStatusCode.OK;

			// Act
			TestFile testFile = testFileReader.Read(stringReader);

			// Assert
			Test test = testFile.Tests.First();
			Assert.That(test.ExpectedHttpStatusCode, Is.EqualTo(expectedCode));
		}

		[Test]
		public void Read_should_parse_addheader_attribute()
		{
			// Arrange
			string xml = GetSingleTestExample();
			var stringReader = new StringReader(xml);
			var testFileReader = GetTestFileReader();

			// Act
			TestFile testFile = testFileReader.Read(stringReader);

			// Assert
			Test test = testFile.Tests.First();
			Assert.That(test.Headers[0].Key, Is.EqualTo("mykey"));
			Assert.That(test.Headers[0].Value, Is.EqualTo("12345"));

			Assert.That(test.Headers[1].Key, Is.EqualTo("bar"));
			Assert.That(test.Headers[1].Value, Is.EqualTo("foo"));

			Assert.That(test.Headers[2].Key, Is.EqualTo("emptyvalue"));
			Assert.That(test.Headers[2].Value, Is.EqualTo(""));

			Assert.That(test.Headers[3].Key, Is.EqualTo("Cookie"));
			Assert.That(test.Headers[3].Value, Is.EqualTo("referer=harrispilton.com"));
		}

		[Test]
		public void Read_should_populate_parseresponse()
		{
			// Arrange
			string xml = GetSingleTestExample();
			var stringReader = new StringReader(xml);
			var testFileReader = GetTestFileReader();

			// Act
			TestFile testFile = testFileReader.Read(stringReader);

			// Assert
			Test test = testFile.Tests.First();
            Assert.That(test.CapturedVariables.Count, Is.EqualTo(3));
			Assert.That(test.CapturedVariables[0].Regex, Is.EqualTo("parse 1"));
			Assert.That(test.CapturedVariables[1].Regex, Is.EqualTo("parse 11"));
			Assert.That(test.CapturedVariables[2].Regex, Is.EqualTo("parse 99"));
		}

		[Test]
		public void Read_should_populate_all_assertions()
		{
			// Arrange
			string xml = GetSingleTestExample();
			var stringReader = new StringReader(xml);
			var testFileReader = GetTestFileReader();

			// Act
			TestFile testFile = testFileReader.Read(stringReader);

			// Assert
			Test test = testFile.Tests.First();
            Assert.That(test.Assertions.Count, Is.EqualTo(6));
			Assert.That(test.Assertions[0].Value, Is.EqualTo("positive 1"));
			Assert.That(test.Assertions[1].Value, Is.EqualTo("positive 22"));
			Assert.That(test.Assertions[2].Value, Is.EqualTo("positive 99"));
            Assert.That(test.Assertions[3].Value, Is.EqualTo("negative 1"));
            Assert.That(test.Assertions[4].Value, Is.EqualTo("negative 6"));
            Assert.That(test.Assertions[5].Value, Is.EqualTo("negative 66"));
		}

		[Test]
		public void Read_should_default_to_assertionmethod_regex_when_missing()
		{
			// Arrange
			string xml = GetSingleTestExample();
			var stringReader = new StringReader(xml);
			var testFileReader = GetTestFileReader();

			// Act
			TestFile testFile = testFileReader.Read(stringReader);

			// Assert
			Test test = testFile.Tests.First();

			Assertion assertion1 = test.Assertions[1];
			Assert.That(assertion1.AssertionMethod, Is.EqualTo(AssertionMethod.Regex));
		}

		[Test]
		public void Read_should_populate_all_assertion_properties()
		{
			// Arrange
			string xml = GetSingleTestExample();
			var stringReader = new StringReader(xml);
			var testFileReader = GetTestFileReader();

			// Act
			TestFile testFile = testFileReader.Read(stringReader);

			// Assert
			Test test = testFile.Tests.First();

			Assertion assertion1 = test.Assertions[0];
			Assert.That(assertion1.Value, Is.EqualTo("positive 1"));
			Assert.That(assertion1.Description, Is.EqualTo("1"));
			Assert.That(assertion1.AssertionType, Is.EqualTo(AssertionType.Positive));
			Assert.That(assertion1.AssertionMethod, Is.EqualTo(AssertionMethod.Regex));

			Assertion assertion2 = test.Assertions[3];
			Assert.That(assertion2.Value, Is.EqualTo("negative 1"));
			Assert.That(assertion2.Description, Is.EqualTo("1"));
			Assert.That(assertion2.AssertionType, Is.EqualTo(AssertionType.Negative));
			Assert.That(assertion2.AssertionMethod, Is.EqualTo(AssertionMethod.CSQuery));
        }

        [Test]
        public void Read_should_add_captured_variables()
        {
            // Arrange
            string xml = GetFullExample();
            var stringReader = new StringReader(xml);
            var testFileReader = GetTestFileReader();

            // Act
            TestFile testFile = testFileReader.Read(stringReader);

            // Assert
            Test test = testFile.Tests.ElementAtOrDefault(1);
            Assert.That(test.AvailableVariables.Count, Is.EqualTo(6));
            Assert.That(test.AvailableVariables.Last().Name, Is.EqualTo("test"));
        }

		[Test]
		public void Read_should_parse_beforeExecuteScript_element()
		{
			// Arrange
			string xml = GetSingleTestExample();
			var stringReader = new StringReader(xml);
			var testFileReader = GetTestFileReader();

			// Act
			TestFile testFile = testFileReader.Read(stringReader);

			// Assert
			Test test = testFile.Tests.First();
			Assert.That(test.BeforeExecuteScript, Is.EqualTo("using Amazinglib;string name = \"<the reader>\";"));
		}
	}
}
