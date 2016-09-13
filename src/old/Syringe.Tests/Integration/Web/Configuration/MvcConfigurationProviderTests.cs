using System;
using System.IO;
using Moq;
using NUnit.Framework;
using Syringe.Core.Configuration;
using Syringe.Web;
using Syringe.Web.Configuration;

namespace Syringe.Tests.Integration.Web.Configuration
{
    [TestFixture]
    public class MvcConfigurationProviderTests
    {
        private Mock<IConfigLocator> _configLocatorMock;
        private MvcConfigurationProvider _provider;
        private readonly string _configDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Integration", "Web", "Configuration");

        [SetUp]
        public void Setup()
        {
            _configLocatorMock = new Mock<IConfigLocator>();
            _configLocatorMock
                .Setup(x => x.ResolveConfigFile("websiteconfig.json"))
                .Returns(Path.Combine(_configDirectory, "websiteconfig.json"));

            _provider = new MvcConfigurationProvider(_configLocatorMock.Object);
        }

        [Test]
        public void Load_should_deserialize_from_json_file()
        {
            // given

            // when
            MvcConfiguration configuration = _provider.Load();

            // then
            Assert.That(configuration, Is.Not.Null);
            Assert.That(configuration.ServiceUrl, Is.EqualTo("http://example.com:1981"));
        }

        [Test]
        public void Load_should_not_load_configuration_twice()
        {
            // given

            // when
            MvcConfiguration configuration1 = _provider.Load();
            MvcConfiguration configuration2 = _provider.Load();

            // then
            Assert.That(configuration1, Is.Not.Null);
            Assert.That(configuration1, Is.EqualTo(configuration2));
        }

        [Test]
        public void Load_should_default_serviceurl_to_localhost_if_none_set()
        {
            // given
            string path = Path.Combine(_configDirectory, "empty-websiteconfig.json");
            _configLocatorMock
                .Setup(x => x.ResolveConfigFile("websiteconfig.json"))
                .Returns(path);

            // when
            MvcConfiguration configuration = _provider.Load();

            // then
            Assert.That(configuration, Is.Not.Null);
            Assert.That(configuration.ServiceUrl, Is.EqualTo("http://example.com:1981"));
        }
    }
}