using Syringe.Core.Configuration;

namespace Syringe.Core.Extensions
{
	public static class ConfigurationExtensions
	{
		public static bool ContainsOAuthCredentials(this IConfiguration configuration)
		{
			return (!string.IsNullOrEmpty(configuration.Settings.OAuthConfiguration?.GoogleAuthClientId) && !string.IsNullOrEmpty(configuration.Settings.OAuthConfiguration?.GoogleAuthClientSecret))
				   || (!string.IsNullOrEmpty(configuration.Settings.OAuthConfiguration?.MicrosoftAuthClientId) && !string.IsNullOrEmpty(configuration.Settings.OAuthConfiguration?.MicrosoftAuthClientSecret))
				   || (!string.IsNullOrEmpty(configuration.Settings.OAuthConfiguration?.GithubAuthClientId) && !string.IsNullOrEmpty(configuration.Settings.OAuthConfiguration?.GithubAuthClientSecret));
		}
	}
}