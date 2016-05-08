using NUnit.Framework;
using Syringe.Client;

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
			ServiceStarter.OwinServer.Dispose();
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
	}
}
