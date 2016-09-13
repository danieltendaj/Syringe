using NUnit.Framework;
using Syringe.Core.Configuration;
using Syringe.Core.Extensions;

namespace Syringe.Tests.Unit.Core.Configuration
{
    [TestFixture]
    public class ConfigurationExtensionTests
    {
        [Test]
        public void shouldnt_detect_authentication_when_no_auth_is_provided()
        {
            // given
            var config = new JsonConfiguration();

            // when
            bool result = config.ContainsOAuthCredentials();

            // then
            Assert.That(result, Is.False);
        }

        [Test]
        public void should_detect_configuration_when_google_auth_is_provided()
        {
            // given
            var config = new JsonConfiguration
            {
                OAuthConfiguration = new OAuthConfiguration
                {
                    GoogleAuthClientId = "data",
                    GoogleAuthClientSecret = "moar data"
                }
            };

            // when
            bool result = config.ContainsOAuthCredentials();

            // then
            Assert.That(result, Is.True);
        }

        [Test]
        public void should_detect_configuration_when_microsoft_auth_is_provided()
        {
            // given
            var config = new JsonConfiguration
            {
                OAuthConfiguration = new OAuthConfiguration
                {
                    MicrosoftAuthClientId = "data",
                    MicrosoftAuthClientSecret = "moar data"
                }
            };

            // when
            bool result = config.ContainsOAuthCredentials();

            // then
            Assert.That(result, Is.True);
        }

        [Test]
        public void should_detect_configuration_when_github_auth_is_provided()
        {
            // given
            var config = new JsonConfiguration
            {
                OAuthConfiguration = new OAuthConfiguration
                {
                    GithubAuthClientId = "data",
                    GithubAuthClientSecret = "moar data"
                }
            };

            // when
            bool result = config.ContainsOAuthCredentials();

            // then
            Assert.That(result, Is.True);
        }
    }
}