using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Moq;
using NUnit.Framework;
using Syringe.Core.Configuration;
using Syringe.Core.Environment;
using Syringe.Core.Environment.Json;
using Environment = Syringe.Core.Environment.Environment;

namespace Syringe.Tests.Integration.Core.Environments
{
	[TestFixture]
	public class JsonEnvironmentProviderTests
	{
		private string _defaultConfigPath;
	    private Mock<IConfigLocator> _configLocatorMock;

		[SetUp]
		public void Setup()
		{
			_defaultConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Integration", "Core", "Environments", "environments.json");
            _configLocatorMock = new Mock<IConfigLocator>();
		    _configLocatorMock
		        .Setup(x => x.ResolveConfigFile("environments.json"))
		        .Returns(_defaultConfigPath);
		}
        
		[Test]
		public void should_deserialize_json_environments()
		{
			// given
			var provider = new JsonEnvironmentProvider(_configLocatorMock.Object);

			// when
			IEnumerable<Environment> environments = provider.GetAll();

			// then
			Assert.That(environments, Is.Not.Null);
			Assert.That(environments.Count(), Is.EqualTo(5));
		}

		[Test]
		public void should_deserialize_json_environments_and_return_using_order()
		{
			// given
			var provider = new JsonEnvironmentProvider(_configLocatorMock.Object);

			// when
			List<Environment> environments = provider.GetAll().ToList();

			// then
			Assert.That(environments[0].Name, Is.EqualTo("DevTeam1"));
			Assert.That(environments[1].Name, Is.EqualTo("DevTeam2"));
			Assert.That(environments[2].Name, Is.EqualTo("UAT"));
			Assert.That(environments[3].Name, Is.EqualTo("Staging"));
			Assert.That(environments[4].Name, Is.EqualTo("Production"));
		}
	}
}
