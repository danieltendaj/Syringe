using Moq;
using NUnit.Framework;
using StructureMap;
using Syringe.Client;
using Syringe.Client.RestSharpHelpers;
using Syringe.Core.Configuration;
using Syringe.Core.Helpers;
using Syringe.Core.Security;
using Syringe.Core.Services;
using Syringe.Core.Tests.Variables.Encryption;
using Syringe.Tests.StubsMocks;
using Syringe.Web;
using Syringe.Web.DependencyResolution;
using Syringe.Web.Mappers;
using Syringe.Web.Models;

namespace Syringe.Tests.Unit.Web.DependencyResolution
{
	public class DefaultRegistryTests
	{
		private IContainer GetContainer(MvcConfiguration mvcConfig, IConfigurationService configService)
		{
			var defaultRegistry = new DefaultRegistry(mvcConfig, configService);
			var container = new Container(c =>
			{
				c.AddRegistry(defaultRegistry);
			});

			return container;
		}

		private void AssertDefaultType<TParent, TConcrete>(IContainer container = null)
		{
			// given
			if (container == null)
				container = GetContainer(new MvcConfiguration(), new ConfigurationServiceMock());

			// when
			TParent instance = container.GetInstance<TParent>();

			// then
			Assert.That(instance, Is.TypeOf<TConcrete>());
		}

		[Test]
		public void should_inject_mvcConfiguration()
		{
			// given
			var mvcConfig = new MvcConfiguration();
			mvcConfig.ServiceUrl = "http://vista-chinesa.org";
			IContainer container = GetContainer(mvcConfig, null);

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
			IContainer container = GetContainer(mvcConfig, null);

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
			var configServiceMock = new ConfigurationServiceMock();
			configServiceMock.Configuration = configuration;

			IContainer container = GetContainer(new MvcConfiguration(), configServiceMock);

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
			IContainer container = GetContainer(new MvcConfiguration(), configServiceMock);

			// when
			var instance1 = container.GetInstance<IConfiguration>();

			configServiceMock.Configuration = differentConfig;
			var instance2 = container.GetInstance<IConfiguration>();

			// then
			Assert.That(instance1, Is.EqualTo(instance2));
		}

		[Test]
		public void should_register_RijndaelEncryption_for_IEncryption_with_password_from_config()
		{
			// given
			var configuration = new JsonConfiguration()
			{
				EncryptionKey = "amygdala"
			};
			var configServiceMock = new ConfigurationServiceMock();
			configServiceMock.Configuration = configuration;

			IContainer container = GetContainer(new MvcConfiguration(), configServiceMock);

			// when
			var encryption = container.GetInstance<IEncryption>() as RijndaelEncryption;

			// then
			Assert.That(encryption, Is.Not.Null);
			Assert.That(encryption.Password, Is.EqualTo(configuration.EncryptionKey));
		}

		[Test]
		public void should_register_default_variable_encryptor()
		{
			AssertDefaultType<IVariableEncryptor, VariableEncryptor>();
		}

		[Test]
		public void should_register_model_helpers()
		{
			AssertDefaultType<IRunViewModel, RunViewModel>();
			AssertDefaultType<ITestFileMapper, TestFileMapper>();
			AssertDefaultType<IUserContext, UserContext>();
			AssertDefaultType<IUrlHelper, UrlHelper>();
		}

		[Test]
		public void should_register_rest_clients()
		{
			AssertDefaultType<IRestSharpClientFactory, RestSharpClientFactory>();
			AssertDefaultType<ITestService, TestsClient>();
			AssertDefaultType<ITasksService, TasksClient>();
			AssertDefaultType<IHealthCheck, HealthCheck>();
			AssertDefaultType<IEnvironmentsService, EnvironmentsClient>();
		}

		[Test]
		public void should_create_rest_clients_with_serviceurl()
		{
			// given
			string serviceUrl = "http://www.example.com";

			var mvcConfig = new MvcConfiguration() {ServiceUrl = serviceUrl};
			var container = GetContainer(mvcConfig, new ConfigurationServiceMock());

			// when
			TestsClient testClient = container.GetInstance<ITestService>() as TestsClient;
			TasksClient taskClient = container.GetInstance<ITasksService>() as TasksClient;
			HealthCheck healthClient = container.GetInstance<IHealthCheck>() as HealthCheck;
			EnvironmentsClient environmentClient = container.GetInstance<IEnvironmentsService>() as EnvironmentsClient;

			// then
			Assert.That(testClient.ServiceUrl, Is.EqualTo(serviceUrl));
			Assert.That(taskClient.ServiceUrl, Is.EqualTo(serviceUrl));
			Assert.That(healthClient.ServiceUrl, Is.EqualTo(serviceUrl));
			Assert.That(environmentClient.ServiceUrl, Is.EqualTo(serviceUrl));
		}
	}
}
