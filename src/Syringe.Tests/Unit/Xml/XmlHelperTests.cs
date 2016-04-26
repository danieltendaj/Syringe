using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using Syringe.Core.Repositories.XML.Reader;
using Syringe.Core.Xml;
using XmlException = Syringe.Core.Exceptions.XmlException;

namespace Syringe.Tests.Unit.Xml
{
    public class XmlHelperTests
    {
		[Test]
		public void GetOptionalElementValue_should_return_element_value()
		{
			// Arrange
			string xml = GetBasicXml();
			var element = XElement.Parse(xml);

			// Act
			string actualValue = XmlHelper.GetOptionalElementValue(element, "baseurl");

			// Assert
			Assert.That(actualValue, Is.EqualTo("a value"));
		}

		[Test]
		public void GetOptionalElementValue_should_return_empty_string_when_element_does_not_exist()
		{
			// Arrange
			string xml = GetBasicXml();
			var element = XElement.Parse(xml);

			// Act
			string actualValue = XmlHelper.GetOptionalElementValue(element, "foobar");

			// Assert
			Assert.That(actualValue, Is.EqualTo(""));
		}

		[Test]
		public void GetRequiredElementValue_should_return_element_value()
		{
			// Arrange
			string xml = GetBasicXml();
			var element = XElement.Parse(xml);

			// Act
			string actualValue = XmlHelper.GetRequiredElementValue(element, "baseurl");

			// Assert
			Assert.That(actualValue, Is.EqualTo("a value"));
		}

		[Test]
	    public void GetRequiredElementValue_should_throw_exception_for_missing_element()
	    {
		    // Arrange
			string xml = GetBasicXml();
			var element = XElement.Parse(xml);

			// Act + Assert
			Assert.Throws<XmlException>(() => XmlHelper.GetRequiredElementValue(element, "foobar"));
	    }

		[Test]
		public void AttributeAsInt_should_parse_attribute_value()
		{
			// Arrange
			string xml = GetBasicXml();
			var element = XElement.Parse(xml).Elements("foo").First();

			// Act
			int actualValue = XmlHelper.AttributeAsInt(element, "id");

			// Assert
			Assert.That(actualValue, Is.EqualTo(3));
		}

		[Test]
		public void AttributeAsInt_should_use_default_value_when_attribute_doesnt_exist()
		{
			// Arrange
			string xml = GetBasicXml();
			var element = XElement.Parse(xml).Elements("foo").First();

			// Act
			int actualValue = XmlHelper.AttributeAsInt(element, "doesntexist", 99);

			// Assert
			Assert.That(actualValue, Is.EqualTo(99));
		}

		[Test]
		public void ElementAsInt_should_parse_element_value()
		{
			// Arrange
			string xml = GetBasicXml();
			var element = XElement.Parse(xml);

			// Act
			int actualValue = XmlHelper.ElementAsInt(element, "somenumber");

			// Assert
			Assert.That(actualValue, Is.EqualTo(13));
		}

		[Test]
		public void ElementAsInt_should_use_default_value_when_element_doesnt_exist()
		{
			// Arrange
			string xml = GetBasicXml();
			var element = XElement.Parse(xml);

			// Act
			int actualValue = XmlHelper.ElementAsInt(element, "doesntexist", 77);

			// Assert
			Assert.That(actualValue, Is.EqualTo(77));
		}

		[Test]
		public void ElementAsBool_should_parse_element_value()
		{
			// Arrange
			string xml = GetBasicXml();
			var element = XElement.Parse(xml);

			// Act
			bool actualValue = XmlHelper.ElementAsBool(element, "somebool");

			// Assert
			Assert.That(actualValue, Is.True);
		}

		[Test]
		public void ElementAsBool_should_use_default_value_when_element_doesnt_exist()
		{
			// Arrange
			string xml = GetBasicXml();
			var element = XElement.Parse(xml);

			// Act
			bool actualValue = XmlHelper.ElementAsBool(element, "doesntexist", true);

			// Assert
			Assert.That(actualValue, Is.True);
		}

		[Test]
		public void GetRequiredAttribute_should_return_attribute_value()
		{
			// Arrange
			string xml = GetXmlWithAttributes();
			var element = XElement.Parse(xml);

			// Act
			string actualValue = XmlHelper.GetRequiredAttribute(element, "repeat");

			// Assert
			Assert.That(actualValue, Is.EqualTo("1333"));
		}

		[Test]
		public void GetRequiredAttribute_should_throw_exception_when_attribute_does_not_exist()
		{
			// Arrange
			string xml = GetXmlWithAttributes();
			var element = XElement.Parse(xml);

			// Act + Assert
			Assert.Throws<XmlException>(() => XmlHelper.GetRequiredAttribute(element, "blah"));
		}

		[Test]
		public void GetOptionalAttribute_should_return_attribute_value()
		{
			// Arrange
			string xml = GetXmlWithAttributes();
			var element = XElement.Parse(xml);

			// Act
			string actualValue = XmlHelper.GetOptionalAttribute(element, "repeat");

			// Assert
			Assert.That(actualValue, Is.EqualTo("1333"));
		}

		[Test]
		public void GetOptionalAttribute_should_return_empty_string_when_attribute_does_not_exist()
		{
			// Arrange
			string xml = GetXmlWithAttributes();
			var element = XElement.Parse(xml);

			// Act
			string actualValue = XmlHelper.GetOptionalAttribute(element, "blah");

			// Assert
			Assert.That(actualValue, Is.EqualTo(""));
		}

		[Test]
		public void GetOptionalAttribute_should_return_defaultvalue_when_attribute_does_not_exist()
		{
			// Arrange
			string xml = GetXmlWithAttributes();
			var element = XElement.Parse(xml);

			// Act
			string actualValue = XmlHelper.GetOptionalAttribute(element, "blah", "default value");

			// Assert
			Assert.That(actualValue, Is.EqualTo("default value"));
		}

	    private string GetBasicXml()
	    {
		    return @"<?xml version=""1.0"" encoding=""utf-8""?>
		            <root>
						<baseurl>a value</baseurl>
						<foo id=""3"" />
						<somenumber>13</somenumber>
						<somebool>true</somebool>
					</root>";
	    }

		private string GetXmlWithAttributes()
		{
			return @"<?xml version=""1.0"" encoding=""utf-8""?>
		            <tests repeat=""1333"">
							<test
								id=""2""
								verifypositive=""verify this string exists""
								verifynextpositive=""{TIMESTAMP}""
							/>
					</tests>";
		}
    }
}
