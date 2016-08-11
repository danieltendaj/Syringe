using Microsoft.AspNet.SignalR;
using Moq;
using NUnit.Framework;
using StructureMap;
using Syringe.Core.Configuration;
using Syringe.Service;
using Syringe.Service.Controllers.Hubs;
using Syringe.Service.DependencyResolution;
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
			AssertDefaultType<IDependencyResolver, StructureMapSignalRDependencyResolver>();
			AssertDefaultType<System.Web.Http.Dependencies.IDependencyResolver, StructureMapResolver>();
			AssertDefaultType<Startup, Startup>();
			AssertDefaultType<TaskMonitorHub, TaskMonitorHub>();
			AssertDefaultType<IConfigurationStore, ConfigurationStoreMock>();
			AssertDefaultType<IConfiguration, JsonConfiguration>();
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
	}
}
