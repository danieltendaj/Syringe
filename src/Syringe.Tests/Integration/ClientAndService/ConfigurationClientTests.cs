using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Syringe.Client;
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
    }
}
