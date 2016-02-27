﻿using System.IO;
using NUnit.Framework;
using Syringe.Core.Exceptions;
using Syringe.Core.Extensions;
using Syringe.Core.TestCases.Configuration;

namespace Syringe.Tests.Unit.Configuration
{
    public class LegacyConfigReaderTests
    {
		[Test]
		public void Read_should_throw_exception_when_root_element_is_missing()
		{
			// Arrange
			string xml = @"<?xml version=""1.0"" encoding=""utf-8"" ?><something></something>";
			var stringReader = new StringReader(xml);
			var configReader = new LegacyConfigReader();

			// Act + Assert
			Assert.Throws<ConfigurationException>(() => configReader.Read(stringReader));
		}

		[Test]
		public void Read_should_populate_known_properties()
	    {
		    // Arrange
			var configReader = new LegacyConfigReader();
			string xml = GetConfigXml();
			StringReader stringReader = new StringReader(xml);

			// Act
			Config config = configReader.Read(stringReader);

			// Assert
			Assert.IsNotNull(config.Variables);
			Assert.AreEqual("http://www.google.com", config.BaseUrl);
			Assert.AreEqual("my proxy", config.Proxy);
			Assert.AreEqual("AOL IE 6", config.Useragent);
			Assert.AreEqual("http://username:password@127.0.0.1:8080", config.Httpauth);
			Assert.AreEqual(LogType.OnFail, config.GlobalHttpLog);
			Assert.AreEqual("My config comment", config.Comment);
			Assert.AreEqual(44, config.Timeout);
			Assert.AreEqual(88, config.GlobalTimeout);
	    }

	    [Test]
	    public void Read_should_populate_custom_variables()
	    {
		    // Arrange
		    var configReader = new LegacyConfigReader();
		    string xml = GetConfigXmlWithCustomVariables();
		    StringReader stringReader = new StringReader(xml);

		    // Act
		    Config config = configReader.Read(stringReader);

		    // Assert
			Assert.AreEqual("http://www.google.com", config.Variables.ByName("baseurl"));
			Assert.AreEqual("http://www.bing.com", config.Variables.ByName("baseurl1"));
			Assert.AreEqual("http://www.yahoo.com", config.Variables.ByName("baseurl88"));
			Assert.AreEqual("http://www.dogpile.com", config.Variables.ByName("myvariable"));
	    }

	    private string GetConfigXml()
	    {
		    return @"<?xml version=""1.0"" encoding=""utf-8""?>
		            <root>
						<baseurl>http://www.google.com</baseurl>
						<proxy>my proxy</proxy>
						<useragent>AOL IE 6</useragent>
						<httpauth>http://username:password@127.0.0.1:8080</httpauth>
						<globalhttplog>onfail</globalhttplog>
						<comment>My config comment</comment>
						<timeout>44</timeout>
						<globaltimeout>88</globaltimeout>
					</root>";
	    }

		private string GetConfigXmlWithCustomVariables()
		{
			return @"<?xml version=""1.0"" encoding=""utf-8""?>
		            <root>
						<proxy>my proxy</proxy>
						<baseurl>http://www.google.com</baseurl>
						<baseurl1>http://www.bing.com</baseurl1>
						<baseurl88>http://www.yahoo.com</baseurl88>
						<myvariable>http://www.dogpile.com</myvariable>
					</root>";
		}
    }
}
