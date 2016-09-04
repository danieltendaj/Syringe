using Syringe.Core.Configuration;

namespace Syringe.Core.Extensions
{
    public static class ConfigurationExtensions
    {
        public static bool ContainsOAuthCredentials(this IConfiguration configuration)
        {
            return (!string.IsNullOrEmpty(configuration.OAuthConfiguration?.GoogleAuthClientId) && !string.IsNullOrEmpty(configuration.OAuthConfiguration?.GoogleAuthClientSecret))
                   || (!string.IsNullOrEmpty(configuration.OAuthConfiguration?.MicrosoftAuthClientId) && !string.IsNullOrEmpty(configuration.OAuthConfiguration?.MicrosoftAuthClientSecret))
                   || (!string.IsNullOrEmpty(configuration.OAuthConfiguration?.GithubAuthClientId) && !string.IsNullOrEmpty(configuration.OAuthConfiguration?.GithubAuthClientSecret));
        }
    }
}