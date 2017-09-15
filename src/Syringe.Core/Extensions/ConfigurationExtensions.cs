using Syringe.Core.Configuration;

namespace Syringe.Core.Extensions
{
	public static class ConfigurationExtensions
	{
		public static bool ContainsOAuthCredentials(this Settings settings)
		{
			return (!string.IsNullOrEmpty(settings.OAuthConfiguration?.GoogleAuthClientId) && !string.IsNullOrEmpty(settings.OAuthConfiguration?.GoogleAuthClientSecret))
				   || (!string.IsNullOrEmpty(settings.OAuthConfiguration?.MicrosoftAuthClientId) && !string.IsNullOrEmpty(settings.OAuthConfiguration?.MicrosoftAuthClientSecret))
				   || (!string.IsNullOrEmpty(settings.OAuthConfiguration?.GithubAuthClientId) && !string.IsNullOrEmpty(settings.OAuthConfiguration?.GithubAuthClientSecret));
		}
	}
}