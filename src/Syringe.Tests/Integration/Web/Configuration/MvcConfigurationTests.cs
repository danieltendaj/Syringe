using NUnit.Framework;
using Syringe.Web;
using Syringe.Web.Configuration;

namespace Syringe.Tests.Integration.Web.Configuration
{
	[TestFixture]
	public class MvcConfigurationTests
	{
		[Test]
		public void signalr_url_should_be_built()
		{
			// given
			MvcConfiguration configuration = new MvcConfiguration();

			// when + then
			Assert.That(configuration.SignalRUrl, Is.Not.Null);
			Assert.That(configuration.SignalRUrl, Is.EqualTo(configuration.ServiceUrl +"/signalr"));
		}
	}
}
