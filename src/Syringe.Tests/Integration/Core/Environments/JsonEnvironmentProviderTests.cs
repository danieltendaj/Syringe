using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Syringe.Core.Environment;
using Environment = Syringe.Core.Environment.Environment;

namespace Syringe.Tests.Integration.Core.Environments
{
	[TestFixture]
	public class JsonEnvironmentProviderTests
	{
		private string _defaultConfigPath;

		[SetUp]
		public void Setup()
		{
			_defaultConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Integration", "Core", "Environments", "environments.json");
		}

		[Test]
		public void should_return_empty_list_when_config_file_does_not_exist()
		{
			// given
			string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "doesntexist.json");
			var provider = new JsonEnvironmentProvider(configPath);

			// when
			IEnumerable<Environment> environments = provider.GetAll();

			// then
			Assert.That(environments, Is.Not.Null);
			Assert.That(environments.Count(), Is.EqualTo(0));
		}

		[Test]
		public void should_deserialize_json_environments()
		{
			// given
			var provider = new JsonEnvironmentProvider(_defaultConfigPath);

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
			var provider = new JsonEnvironmentProvider(_defaultConfigPath);

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
