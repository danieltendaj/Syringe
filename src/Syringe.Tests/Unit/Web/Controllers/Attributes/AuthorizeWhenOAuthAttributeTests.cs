using FakeN.Web;
using NUnit.Framework;
using Syringe.Core.Configuration;
using Syringe.Web.Controllers.Attribute;

namespace Syringe.Tests.Unit.Web.Controllers.Attributes
{
    [TestFixture]
    public class AuthorizeWhenOAuthAttributeTests
    {
        private JsonConfiguration _config;
        private AuthorizeWhenOAuthAttribute _attribute;

        [SetUp]
        public void Setup()
        {
            _config = new JsonConfiguration();
            _attribute = new AuthorizeWhenOAuthAttribute { Configuration = _config };
        }

        [Test]
        public void should_allow_anonymous_when_no_providers_set()
        {
            // given

            // when
            bool result = _attribute.RunAuthorizeCore(new FakeHttpContext());

            // then
            Assert.True(result);
        }

        [Test]
        public void should_authorise_when_provider_set_and_user_is_authenticated()
        {
            // given
            _config.OAuthConfiguration.GithubAuthClientId = "github1";
            _config.OAuthConfiguration.GithubAuthClientSecret = "github2";

            var fakeHttpContext = new FakeHttpContext();
            ((MutableIdentity)fakeHttpContext.User.Identity).IsAuthenticated = true;

            // when
            bool result = _attribute.RunAuthorizeCore(fakeHttpContext);

            // then
            Assert.True(result);
        }
    }
}
