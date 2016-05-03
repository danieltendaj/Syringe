using System;
using System.IO;
using NUnit.Framework;
using Syringe.Core.Exceptions;
using Syringe.Web;

namespace Syringe.Tests.Integration.Web
{
	[TestFixture]
	public class MvcConfigurationTests
	{
		[SetUp]
		public void Setup()
		{
			// Stop caching of the configuration between tests
			MvcConfiguration.Configuration = null;
		}

		[Test]
		public void Load_should_deserialize_from_json_file()
		{
			// given
			string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Integration", "Web", "websiteconfig.json");

			// when
			MvcConfiguration configuration = MvcConfiguration.Load(path);

			// then
			Assert.That(configuration, Is.Not.Null);
			Assert.That(configuration.ServiceUrl, Is.EqualTo("http://example.com:1981"));
		}

		[Test]
		public void Load_should_throw_exception_if_file_doesnt_exist()
		{
			// given, when + then
			Assert.Throws<ConfigurationException>(() => MvcConfiguration.Load("file-doesnt-exist.json"));
		}

		[Test]
		public void Load_should_not_load_configuration_twice()
		{
			// given
			string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Integration", "Web", "websiteconfig.json");

			// when
			MvcConfiguration configuration = MvcConfiguration.Load(path);
			configuration = MvcConfiguration.Load("this file doesnt exist but it doesnt matter as it's cached");

			// then
			Assert.That(configuration, Is.Not.Null);
			Assert.That(configuration.ServiceUrl, Is.EqualTo("http://example.com:1981"));
		}

		[Test]
		public void Load_should_default_serviceurl_to_localhost_if_none_set()
		{
			// given
			string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Integration", "Web", "empty-websiteconfig.json");

			// when
			MvcConfiguration configuration = MvcConfiguration.Load(path);

			// then
			Assert.That(configuration, Is.Not.Null);
			Assert.That(configuration.ServiceUrl, Is.EqualTo("http://example.com:1981"));
		}

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
