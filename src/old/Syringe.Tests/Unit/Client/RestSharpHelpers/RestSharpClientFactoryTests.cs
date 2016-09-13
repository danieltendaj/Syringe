using NUnit.Framework;
using RestSharp;
using Syringe.Client.RestSharpHelpers;

namespace Syringe.Tests.Unit.Client.RestSharpHelpers
{
    [TestFixture]
    public class RestSharpClientFactoryTests
    {
        [Test]
        public void should_return_unique_client()
        {
            // given
            var factory = new RestSharpClientFactory();
            const string baseUrl = "http://some-base-url.com/";

            // when
            IRestClient factory1 = factory.Create(baseUrl);
            IRestClient factory2 = factory.Create(baseUrl);

            // then
            Assert.That(factory1, Is.Not.Null);
            Assert.That(factory1.BaseUrl.AbsoluteUri, Is.EqualTo(baseUrl));

            Assert.That(factory2, Is.Not.Null);
            Assert.That(factory2.BaseUrl.AbsoluteUri, Is.EqualTo(baseUrl));

            Assert.That(factory1, Is.Not.EqualTo(factory2));
        }
    }
}