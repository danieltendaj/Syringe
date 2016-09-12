using NUnit.Framework;
using Syringe.Web.Configuration;

namespace Syringe.Tests.Integration.Web.Configuration
{
	[TestFixture]
	public class MvcConfigurationTests
	{
		[Test]
		public void serviceurl_should_have_default_value()
		{
			// given
			MvcConfiguration configuration = new MvcConfiguration();

			// when + then
			Assert.That(configuration.ServiceUrl, Is.EqualTo("http://localhost:1981"));
		}
	}
}
