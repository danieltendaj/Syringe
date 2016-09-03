using NUnit.Framework;
using StructureMap;
using Syringe.Client;
using Syringe.Client.RestSharpHelpers;
using Syringe.Core.Configuration;
using Syringe.Core.Helpers;
using Syringe.Core.Security;
using Syringe.Core.Services;
using Syringe.Core.Tests.Variables.Encryption;
using Syringe.Web.Configuration;
using Syringe.Web.DependencyResolution;
using Syringe.Web.Mappers;
using Syringe.Web.Models;

namespace Syringe.Tests.Unit.Web.DependencyResolution
{
    public class DefaultRegistryTests
    {
        private IContainer _container;

        [SetUp]
        public void Setup()
        {
            _container = GetContainer();
        }

        [Test]
        public void should_register_default_configurations()
        {
            AssertDefaultType<MvcConfiguration, MvcConfiguration>();
            AssertDefaultType<IConfigurationService, ConfigurationClient>();

            var provider1 = AssertDefaultType<IMvcConfigurationProvider, MvcConfigurationProvider>();
            var provider2 = AssertDefaultType<IMvcConfigurationProvider, MvcConfigurationProvider>();
            Assert.That(provider1, Is.EqualTo(provider2));
        }

        [Test]
        public void should_register_default_variable_encryptor()
        {
            // given
            var configuration = new JsonConfiguration { EncryptionKey = "Doobee" };

            // when
            _container = GetContainer(configuration);

            // then
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
            AssertDefaultType<ITestService, TestsClient>();
            AssertDefaultType<ITasksService, TasksClient>();
            AssertDefaultType<IHealthCheck, HealthCheck>();
            AssertDefaultType<IEnvironmentsService, EnvironmentsClient>();
        }

        private IContainer GetContainer(IConfiguration configuration = null)
        {
            var defaultRegistry = new DefaultRegistry(configuration);
            var container = new Container(c =>
            {
                c.AddRegistry(defaultRegistry);
            });

            return container;
        }

        private TParent AssertDefaultType<TParent, TConcrete>()
        {
            // given

            // when
            TParent instance = _container.GetInstance<TParent>();

            // then
            Assert.That(instance, Is.TypeOf<TConcrete>());

            return instance;
        }
    }
}
