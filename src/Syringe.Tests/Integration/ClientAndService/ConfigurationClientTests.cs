using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Syringe.Client;
using Syringe.Core.Tests.Scripting;
using Syringe.Core.Tests.Variables;

namespace Syringe.Tests.Integration.ClientAndService
{
	[TestFixture]
	public class ConfigurationClientTests
	{
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			ServiceStarter.StartSelfHostedOwin();
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown()
        {
            ServiceStarter.StopSelfHostedOwin();
        }

		[SetUp]
		public void Setup()
		{
			ServiceStarter.RecreateTestFileDirectory();
			ServiceStarter.RecreateScriptSnippetDirectories();
		}

		[Test]
		public void GetConfiguration_should_get_full_config_object()
		{
			// given
			var client = new ConfigurationClient(ServiceStarter.BaseUrl);

			// when
			var config = client.GetConfiguration();

			// then
			Assert.That(config, Is.Not.Null);
		}

	    [Test]
	    public void GetSystemVariables_should_return_expected_variables()
	    {
            // given
            var client = new ConfigurationClient(ServiceStarter.BaseUrl);

            // when
            IEnumerable<Variable> variables = client.GetSystemVariables();

            // then
            Assert.That(variables, Is.Not.Null);
            Assert.That(variables, Is.Not.Empty);
            Assert.That(variables.FirstOrDefault(x => x.Name == "_randomNumber"), Is.Not.Null);
        }

		[Test]
		public void GetScriptSnippetFilenames_should_return_expected_variables()
		{
			// given
			var client = new ConfigurationClient(ServiceStarter.BaseUrl);

			// when
			IEnumerable<string> snippetFilenames = client.GetScriptSnippetFilenames(ScriptSnippetType.BeforeExecute);

			// then
			Assert.That(snippetFilenames, Is.Not.Null);
			Assert.That(snippetFilenames, Is.Not.Empty);
			Assert.That(snippetFilenames, Contains.Item("snippet1.snippet"));
		}
	}
}
