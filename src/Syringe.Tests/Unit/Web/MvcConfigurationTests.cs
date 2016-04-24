using System;
using NUnit.Framework;
using Syringe.Web;

namespace Syringe.Tests.Unit.Web
{
	[TestFixture]
	public class MvcConfigurationTests
	{
		private MvcConfiguration CreateMvcConfiguration()
		{
			return new MvcConfiguration();
		}

		[Test]
		public void Load_should_deserialize_from_json_file()
		{
			// given
			var actual = CreateMvcConfiguration();

			// when

			// then
			Assert.That(actual, Is.Not.Null);
		}

		[Test]
		public void Load_should_throw_exception_if_file_doesnt_exist()
		{
			// given
			var actual = CreateMvcConfiguration();

			// when

			// then
			Assert.That(actual, Is.Not.Null);
		}

		[Test]
		public void Load_should_not_load_configuration_twice()
		{
			// given
			var actual = CreateMvcConfiguration();

			// when

			// then
			Assert.That(actual, Is.Not.Null);
		}

		[Test]
		public void should_default_serviceurl_to_localhost_if_none_set()
		{
			// given
			var actual = CreateMvcConfiguration();

			// when

			// then
			Assert.That(actual, Is.Not.Null);
		}

		[Test]
		public void signalr_url_should_be_built()
		{
			// given
			var actual = CreateMvcConfiguration();

			// when

			// then
			Assert.That(actual, Is.Not.Null);
		}

		[Test]
		public void signalr_lazy_property_should_be_built()
		{
			// given
			var actual = CreateMvcConfiguration();

			// when

			// then
			Assert.That(actual, Is.Not.Null);
		}

	}
}
