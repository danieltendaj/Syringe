using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Syringe.Client;
using Syringe.Core.Environment;

namespace Syringe.Tests.Integration.ClientAndService
{
	[TestFixture]
	public class EnvironmentsClientTests
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
		public void Get_should_return_list_of_environments()
		{
			// This test relies on the environments.json in the service always having some environments. 

			// given
			var client = new EnvironmentsClient(ServiceStarter.BaseUrl);

			// when
			IEnumerable<Environment> environments = client.Get();

			// then
			Assert.That(environments, Is.Not.Null);
			Assert.That(environments.Count(), Is.GreaterThan(0));
		}
	}
}
