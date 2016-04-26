using System.Linq;
using System.Xml.Linq;
using XmlException = Syringe.Core.Exceptions.XmlException;

namespace Syringe.Core.Xml
{
	internal class XmlHelper
	{
		public static string GetRequiredElementValue(XElement rootElement, string name)
		{
			var element = rootElement.Elements().FirstOrDefault(x => x.Name.LocalName == name);
			if (element == null)
				throw new XmlException("The element <{0}> is missing", name);

			return element.Value;
		}

		public static string GetOptionalElementValue(XElement rootElement, string name)
		{
			var element = rootElement.Elements().FirstOrDefault(x => x.Name.LocalName == name);
			if (element != null)
				return element.Value;

			return "";
		}

		public static string GetRequiredAttribute(XElement rootElement, string attributeName)
		{
			XAttribute attribute = rootElement.Attribute(attributeName);
			if (attribute == null)
				throw new XmlException("The {0} attribute is missing", attributeName);

			return attribute.Value;
		}

		public static string GetOptionalAttribute(XElement rootElement, string attributeName, string defaultValue = "")
		{
			XAttribute attribute = rootElement.Attribute(attributeName);
			if (attribute != null)
				return attribute.Value;

			return defaultValue;
		}

		public static int AttributeAsInt(XElement element, string attributeName, int defaultValue = 0)
		{
			string value = GetOptionalAttribute(element, attributeName);
			int result = 0;
			if (!int.TryParse(value, out result))
				result = defaultValue;

			return result;
		}

        public static int ElementAsInt(XElement element, string elementName, int defaultValue = 0)
		{
			string value = GetOptionalElementValue(element, elementName);
			int result = 0;
			if (!int.TryParse(value, out result))
				result = defaultValue;

			return result;
		}

		public static bool ElementAsBool(XElement element, string elementName, bool defaultValue = false)
		{
			string value = GetOptionalElementValue(element, elementName);
			bool result = false;
			if (!bool.TryParse(value, out result))
				result = defaultValue;

			return result;
		}
	}
}