using System;
using Moq;
using NUnit.Framework;
using Octopus.Client;
using StructureMap;
using Syringe.Core.Configuration;
using Syringe.Core.Environment;
using Syringe.Core.Environment.Octopus;
using Syringe.Core.IO;
using Syringe.Core.Runner.Logging;
using Syringe.Core.Tests.Repositories;
using Syringe.Core.Tests.Repositories.Json.Reader;
using Syringe.Core.Tests.Repositories.Json.Writer;
using Syringe.Core.Tests.Results.Repositories;
using Syringe.Core.Tests.Variables.Encryption;
using Syringe.Core.Tests.Variables.ReservedVariables;
using Syringe.Service;
using Syringe.Service.DependencyResolution;
using Syringe.Service.Parallel;
using Syringe.Tests.StubsMocks;

namespace Syringe.Tests.Unit.Service.DependencyResolution
{
	public class DefaultRegistryTests
	{
		private IContainer GetContainer(IConfigurationStore store)
		{
			var defaultRegistry = new DefaultRegistry(store);
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
				container = GetContainer(new ConfigurationStoreMock());

			// when
			TParent instance = container.GetInstance<TParent>();

			// then
			Assert.That(instance, Is.TypeOf<TConcrete>());
		}

		[Test]
		public void should_inject_default_types()
		{
			AssertDefaultType<System.Web.Http.Dependencies.IDependencyResolver, StructureMapResolver>();
			AssertDefaultType<Startup, Startup>();
			AssertDefaultType<IConfigurationStore, ConfigurationStoreMock>(); // ConfigurationStoreMock from this test
			AssertDefaultType<IConfiguration, JsonConfiguration>();
			AssertDefaultType<IVariableEncryptor, VariableEncryptor>();

			AssertDefaultType<ITestFileResultRepository, MongoTestFileResultRepository>();
			AssertDefaultType<ITestFileQueue, ParallelTestFileQueue>();
            AssertDefaultType<ITestFileRunnerLogger, TestFileRunnerLogger>();
        }

        [Test]
		public void configurationstore_should_be_called()
		{
			// given
			var configuration = new JsonConfiguration()
			{
				WebsiteUrl = "http://www.ee.i.eee.io"
			};

			var configStoreMock = new Mock<IConfigurationStore>();
			configStoreMock.Setup(x => x.Load())
				.Returns(configuration)
				.Verifiable("Load wasn't called");

			IContainer container = GetContainer(configStoreMock.Object);

			// when
			var instance = container.GetInstance<IConfiguration>();

			// then
			configStoreMock.Verify(x => x.Load(), Times.Once);

			Assert.That(instance, Is.Not.Null);
			Assert.That(instance, Is.EqualTo(configuration));
		}

		[Test]
		public void should_inject_key_for_encryption()
		{
			// given
			var configuration = new JsonConfiguration()
			{
				EncryptionKey = "my-password"
			};

			var configStore = new ConfigurationStoreMock();
			configStore.Configuration = configuration;
			IContainer container = GetContainer(configStore);

			// when
			var encryptionInstance = container.GetInstance<IEncryption>() as AesEncryption;

			// then
			Assert.That(encryptionInstance, Is.Not.Null);
			Assert.That(encryptionInstance.Password, Is.EqualTo("my-password"));
		}

		[Test]
		public void should_inject_context_into_testfile_repository()
		{
			// given
			IContainer container = GetContainer(new ConfigurationStoreMock());

			// when
			var instance = container.GetInstance<ITestFileResultRepositoryFactory>() as TestFileResultRepositoryFactory;

			// then
			Assert.That(instance, Is.Not.Null);
			Assert.That(instance.Context, Is.Not.Null);
			Assert.That(instance.Context, Is.InstanceOf(typeof(IContext)));
		}

		[Test]
		public void itaskobserver_should_be_cast_to_itestfilequeue()
		{
			// given
			IContainer container = GetContainer(new ConfigurationStoreMock());

			// when
			var instance = container.GetInstance<ITaskObserver>() as ITestFileQueue;

			// then
			Assert.That(instance, Is.Not.Null);
			Assert.That(instance, Is.InstanceOf(typeof(ITestFileQueue)));
		}

		[Test]
		public void reservedvariableprovider_should_have_placeholder_environment()
		{
			// given
			IContainer container = GetContainer(new ConfigurationStoreMock());

			// when
			var instance = container.GetInstance<IReservedVariableProvider>() as ReservedVariableProvider;

			// then
			Assert.That(instance, Is.Not.Null);
			Assert.That(instance.Environment, Is.EqualTo("<environment here>"));
		}

		[Test]
		public void should_inject_types_for_testfile_reader_writers()
		{
			AssertDefaultType<IFileHandler, FileHandler>();
			AssertDefaultType<ITestRepository, TestRepository>();
			AssertDefaultType<ITestFileReader, TestFileReader>();
			AssertDefaultType<ITestFileWriter, TestFileWriter>();
		}

		[Test]
		public void should_use_octopus_environment_provider_when_keys_exist()
		{
			// given
			var config = new JsonConfiguration()
			{
				OctopusConfiguration = new OctopusConfiguration()
				{
					OctopusApiKey = "I've got the key",
					OctopusUrl = "http://localhost"
				}
			};
			var configStoreMock = new ConfigurationStoreMock();
			configStoreMock.Configuration = config;
			IContainer container = GetContainer(configStoreMock);

			// when + then
			AssertDefaultType<IOctopusRepositoryFactory, OctopusRepositoryFactory>(container);

			var octopusRepository = container.GetInstance<IOctopusRepository>() as OctopusRepository;
			Assert.That(octopusRepository, Is.Not.Null);

			AssertDefaultType<IEnvironmentProvider, OctopusEnvironmentProvider>(container);
		}
	}
}
