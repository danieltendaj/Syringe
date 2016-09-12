using System.Collections.Specialized;
using NUnit.Framework;
using Syringe.Web.Configuration;

namespace Syringe.Tests.Integration.Web.Configuration
{
    [TestFixture]
    public class MvcConfigurationProviderTests
    {
        [Test]
        public void Load_should_grab_config_from_app_config()
        {
            // given
            var appConfig = new NameValueCollection
            {
                { "serviceUrl", "expectedValue"}
            };
            var provider = new MvcConfigurationProvider(appConfig);

            // when
            MvcConfiguration configuration = provider.Load();

            // then
            Assert.That(configuration, Is.Not.Null);
            Assert.That(configuration.ServiceUrl, Is.EqualTo("expectedValue"));
        }

        [Test]
        public void Load_should_use_default_value_if_it_is_missing()
        {
            // given
            var appConfig = new NameValueCollection();
            var provider = new MvcConfigurationProvider(appConfig);

            // when
            MvcConfiguration configuration = provider.Load();

            // then
            Assert.That(configuration, Is.Not.Null);
            Assert.That(configuration.ServiceUrl, Is.EqualTo("http://localhost:1981"));
        }
    }
}