using Moq;
using NUnit.Framework;
using StructureMap;
using Syringe.Client;
using Syringe.Core.Configuration;
using Syringe.Core.Services;
using Syringe.Tests.StubsMocks;
using Syringe.Web;
using Syringe.Web.DependencyResolution;

namespace Syringe.Tests.Unit.Web.DependencyResolution
{
	public class DefaultRegistryTests
	{
		private IContainer GetContainer(MvcConfiguration mvcConfig = null, IConfigurationService configService = null)
		{
			if (mvcConfig == null)
				mvcConfig = new MvcConfiguration();

			var defaultRegistry = new DefaultRegistry(mvcConfig, configService);
			var container = new Container(c =>
			{
				c.AddRegistry(defaultRegistry);
			});

			return container;
		}

		[Test]
		public void should_inject_mvcConfiguration()
		{
			// given
			var mvcConfig = new MvcConfiguration();
			mvcConfig.ServiceUrl = "http://vista-chinesa.org";
			IContainer container = GetContainer(mvcConfig);

			// when
			var instance = container.GetInstance<MvcConfiguration>();

			// then
			Assert.That(instance, Is.Not.Null);
			Assert.That(instance, Is.EqualTo(mvcConfig));
		}

		[Test]
		public void should_inject_service_url_to_configurationservice_and_return_configurationclient()
		{
			// given
			string expectedUrl = "http://www.example.com";
			var mvcConfig = new MvcConfiguration();
			mvcConfig.ServiceUrl = expectedUrl;
			IContainer container = GetContainer(mvcConfig);

			// when
			var instance = container.GetInstance<IConfigurationService>();

			// then
			ConfigurationClient client = instance as ConfigurationClient;

			Assert.That(client, Is.Not.Null);
			Assert.That(client.ServiceUrl, Is.EqualTo(expectedUrl));
		}

		[Test]
		public void should_return_iconfiguration_from_service()
		{
			// given
			var configuration = new JsonConfiguration()
			{
				WebsiteUrl = "http://www.telegram.org"
			};
			var configServiceMock = new Mock<IConfigurationService>();
			configServiceMock.Setup(x => x.GetConfiguration()).Returns(configuration);

			IContainer container = GetContainer(null, configServiceMock.Object);

			// when
			var instance = container.GetInstance<IConfiguration>();

			// then
			Assert.That(instance, Is.Not.Null);
			Assert.That(instance, Is.EqualTo(configuration));
		}

		[Test]
		public void iconfiguration_should_be_singleton_instance_from_service()
		{
			// given
			var jsonConfig = new JsonConfiguration() { WebsiteUrl = "a" };
			var differentConfig = new JsonConfiguration() { WebsiteUrl = "b" };
			var configServiceMock = new ConfigurationServiceMock();
			configServiceMock.Configuration = jsonConfig;

			IContainer container = GetContainer(null, configServiceMock);

			// when
			var instance1 = container.GetInstance<IConfiguration>();

			configServiceMock.Configuration = differentConfig;
			var instance2 = container.GetInstance<IConfiguration>();

			// then
			Assert.That(instance1, Is.EqualTo(instance2));
		}
	}
}
