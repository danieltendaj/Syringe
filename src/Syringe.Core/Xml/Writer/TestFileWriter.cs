using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Syringe.Core.Tests;

namespace Syringe.Core.Xml.Writer
{
    public class TestFileWriter : ITestFileWriter
    {
        public string Write(TestFile testFile)
        {
            var stringBuilder = new StringBuilder();
            using (var stringWriter = new Utf8StringWriter(stringBuilder))
            {
                var settings = new XmlWriterSettings
                {
                    IndentChars = "\t",
                    Indent = true
                };

                using (XmlWriter xmlWriter = XmlTextWriter.Create(stringWriter, settings))
                {
                    XElement testsElement = new XElement("tests");

                    if (testFile.Variables.Count > 0)
                    {
                        XElement variableElements = new XElement("variables");

                        foreach (var variable in testFile.Variables)
                        {
                            XElement variableElement = new XElement("variable");
                            variableElement.Add(new XAttribute("name", variable.Name));
                            variableElement.Value = variable.Value;

							variableElement.Add(new XAttribute("environment", variable.Environment.Name));
							variableElement.Value = variable.Value;

							variableElements.Add(variableElement);
                        }

                        testsElement.Add(variableElements);
                    }

                    foreach (Test test in testFile.Tests)
                    {
                        XElement headersElement = GetHeadersElement(test);
                        XElement postbodyElement = GetPostBodyElement(test);
                        XElement parseResponsesElement = GetParseResponsesElement(test);
                        XElement assertionElement = GetAssertionsElement(test);

                        XElement testElement = GetTestElement(test);
                        testElement.Add(headersElement);
                        testElement.Add(postbodyElement);
                        testElement.Add(parseResponsesElement);
                        testElement.Add(assertionElement);

                        testsElement.Add(testElement);
                    }

                    XDocument doc = new XDocument(testsElement);
                    doc.WriteTo(xmlWriter);
                }

                return stringBuilder.ToString();
            }
        }

        private XElement GetTestElement(Test test)
        {
            XElement element = new XElement("test");

            element.Add(new XAttribute("description", test.Description ?? ""));
            element.Add(new XAttribute("url", test.Url ?? ""));
            element.Add(new XAttribute("method", test.Method ?? ""));
            element.Add(new XAttribute("posttype", test.PostType ?? ""));
            element.Add(new XAttribute("verifyresponsecode", (int)test.VerifyResponseCode));
            element.Add(new XAttribute("errormessage", test.ErrorMessage ?? ""));

            return element;
        }

        private XElement GetHeadersElement(Test test)
        {
            XElement headerElement = new XElement("headers");

            foreach (HeaderItem keyValuePair in test.Headers)
            {
                if (!string.IsNullOrEmpty(keyValuePair.Key))
                {
                    XElement element = new XElement("header");
                    element.Add(new XAttribute("name", keyValuePair.Key));

                    AddCDataToElementValue(keyValuePair.Value, element);

                    headerElement.Add(element);
                }
            }

            return headerElement;
        }

        private XElement GetPostBodyElement(Test test)
        {
            XElement postBodyElement = new XElement("postbody");

            if (!string.IsNullOrEmpty(test.PostBody))
                postBodyElement.Add(new XCData(test.PostBody));

            return postBodyElement;
        }

        private XElement GetParseResponsesElement(Test test)
        {
            XElement parseresponsesElement = new XElement("capturedvariables");

            foreach (CapturedVariable item in test.CapturedVariables)
            {
                if (!string.IsNullOrEmpty(item.Regex))
                {
                    XElement element = new XElement("variable");
                    element.Add(new XAttribute("name", item.Name));
                    element.Value = item.Regex;

                    parseresponsesElement.Add(element);
                }
            }

            return parseresponsesElement;
        }

        private XElement GetAssertionsElement(Test test)
        {
            XElement headerElement = new XElement("assertions");

            foreach (Assertion verifyItem in test.Assertions)
            {
                if (!string.IsNullOrEmpty(verifyItem.Description))
                {
                    XElement element = new XElement("assertion");
                    element.Add(new XAttribute("description", verifyItem.Description));
                    element.Add(new XAttribute("type", verifyItem.AssertionType.ToString().ToLower()));

                    AddCDataToElementValue(verifyItem.Regex, element);

                    headerElement.Add(element);
                }
            }

            return headerElement;
        }

        private static void AddCDataToElementValue(string value, XElement element)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (value.Contains("&") ||
                    value.Contains("<") || value.Contains(">"))
                {
                    element.Add(new XCData(value));
                }
                else
                {
                    element.Value = value;
                }
            }
        }
    }
}