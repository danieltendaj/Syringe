using System;
using System.Collections.Generic;
using System.Net;
using NUnit.Framework;
using Org.XmlUnit.Builder;
using Org.XmlUnit.Constraints;
using Syringe.Core.Tests;
using Syringe.Core.Xml.Writer;

namespace Syringe.Tests.Unit.Xml
{
	public class TestFileWriterTests
	{
		public static string XmlExamplesFolder = typeof(TestFileReaderTests).Namespace + ".XmlExamples.Writer.";

		[Test]
		public void write_should_add_variables()
		{
			// Arrange
			string expectedXml = TestHelpers.ReadEmbeddedFile("variables.xml", XmlExamplesFolder);

			TestFile testFile = CreateTestFile();
			testFile.Variables.Add(new Variable("name1", "value1", "env1"));
			testFile.Variables.Add(new Variable("name2", "value2", "env2"));
			TestFileWriter xmlWriter = CreateTestFileWriter();

			// Act
			string actualXml = xmlWriter.Write(testFile);

			// Assert
			Assert.That(Input.FromString(actualXml), CompareConstraint.IsIdenticalTo(Input.FromString(expectedXml)));
		}

		[Test]
		public void write_should_add_all_test_attributes()
		{
			// Arrange
			string expectedXml = TestHelpers.ReadEmbeddedFile("all-attributes.xml", XmlExamplesFolder);

			var test = new Test()
			{
				Position = 1,
				ShortDescription = "short description",
				Url = "http://myserver",
				Method = "post",
				PostType = "text/xml",
				VerifyResponseCode = HttpStatusCode.Accepted,
				ErrorMessage = "my error message",
			};
			TestFile testFile = CreateTestFile(test);
			TestFileWriter xmlWriter = CreateTestFileWriter();

			// Act
			string actualXml = xmlWriter.Write(testFile);

			// Assert
			Assert.That(Input.FromString(actualXml), CompareConstraint.IsIdenticalTo(Input.FromString(expectedXml)));			
		}

		[Test]
		public void write_should_add_header_key_and_values_with_cdata_encoding_when_value_has_html_entities()
		{
			// Arrange
			string expectedXml = TestHelpers.ReadEmbeddedFile("headers.xml", XmlExamplesFolder);

			var test = new Test() { Position = 0 };
			test.AddHeader("key1", "value1");
			test.AddHeader("key2", "some <marvellous> HTML &&&&.");

			TestFile testFile = CreateTestFile(test);
			TestFileWriter xmlWriter = CreateTestFileWriter();

			// Act
			string actualXml = xmlWriter.Write(testFile);

			// Assert
			Assert.That(Input.FromString(actualXml), CompareConstraint.IsIdenticalTo(Input.FromString(expectedXml)));
		}

		[Test]
		public void write_should_add_postbody_with_cdata()
		{
			// Arrange
			string expectedXml = TestHelpers.ReadEmbeddedFile("postbody.xml", XmlExamplesFolder);

			var testCase = new Test() { Position = 0};
			testCase.PostBody = "username=corey&password=welcome&myhtml=<body></body>";
			TestFile testFile = CreateTestFile(testCase);
			TestFileWriter xmlWriter = CreateTestFileWriter();

			// Act
			string actualXml = xmlWriter.Write(testFile);

			// Assert
			Assert.That(Input.FromString(actualXml), CompareConstraint.IsIdenticalTo(Input.FromString(expectedXml)));
		}

		[Test]
		public void write_should_add_capturedvariables()
		{
			// Arrange
			string expectedXml = TestHelpers.ReadEmbeddedFile("capturedvariables.xml", XmlExamplesFolder);

			var test = new Test() { Position = 0 };
            test.CapturedVariables.Add(new CapturedVariable("1", "here is (.*?) regex"));
            test.CapturedVariables.Add(new CapturedVariable("2", "plain text"));
            test.CapturedVariables.Add(new CapturedVariable("3", "This is encoded <test> &."));

			TestFile testFile = CreateTestFile(test);
			TestFileWriter xmlWriter = CreateTestFileWriter();

			// Act
			string actualXml = xmlWriter.Write(testFile);

			// Assert
			Assert.That(Input.FromString(actualXml), CompareConstraint.IsIdenticalTo(Input.FromString(expectedXml)));
		}

		[Test]
		public void write_should_add_verifications()
		{
			// Arrange
			string expectedXml = TestHelpers.ReadEmbeddedFile("assertions.xml", XmlExamplesFolder);

			var test = new Test() { Position = 0 };
            test.Assertions.Add(new Assertion("p90-1", "regex1", AssertionType.Positive));
            test.Assertions.Add(new Assertion("p90-2", "regex2", AssertionType.Positive));
            test.Assertions.Add(new Assertion("p90-3", "this 3rd positive needs CDATA as it has <html> & stuff in it (.*)", AssertionType.Positive));

			test.Assertions.Add(new Assertion("negev1", "regex1", AssertionType.Negative));
			test.Assertions.Add(new Assertion("negev2", "regex2", AssertionType.Negative));
			test.Assertions.Add(new Assertion("negev3", "this 3rd negative needs CDATA as it has <html> & stuff in it (.*)", AssertionType.Negative));

			TestFile testFile = CreateTestFile(test);
			TestFileWriter xmlWriter = CreateTestFileWriter();

			// Act
			string actualXml = xmlWriter.Write(testFile);

			// Assert
			Assert.That(Input.FromString(actualXml), CompareConstraint.IsIdenticalTo(Input.FromString(expectedXml)));
		}

		[Test]
		public void write_should_write_large_files()
		{
			// Arrange
			string expectedXml = TestHelpers.ReadEmbeddedFile("large-file.xml", XmlExamplesFolder);

			var testFile = new TestFile();
			var list = new List<Test>();		

			for (int i = 0; i < 100; i++)
			{
				var test = new Test()
				{
                    Position = 0,
					ShortDescription = "short description" +i,
					Url = "http://myserver",
					Method = "post",
					PostType = "text/xml",
					VerifyResponseCode = HttpStatusCode.Accepted,
					ErrorMessage = "my error message",
				};

				list.Add(test);
			}

			testFile.Tests = list;
			TestFileWriter xmlWriter = CreateTestFileWriter();

			// Act
			string actualXml = xmlWriter.Write(testFile);

			// Assert
			Assert.That(Input.FromString(actualXml), CompareConstraint.IsIdenticalTo(Input.FromString(expectedXml)));
		}

		private TestFileWriter CreateTestFileWriter()
		{
			return new TestFileWriter();
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
