using Syringe.Core.Configuration;

namespace Syringe.Web.Models
{
	public class AuthenticationViewModel
	{
		public Settings Settings { get; set; }
		public string ReturnUrl { get; set; }

		public bool IsOAuthConfigEmpty
		{
			get
			{
				return string.IsNullOrEmpty(Settings.OAuthConfiguration.MicrosoftAuthClientId) &&
					   string.IsNullOrEmpty(Settings.OAuthConfiguration.GoogleAuthClientId) &&
					   string.IsNullOrEmpty(Settings.OAuthConfiguration.GithubAuthClientId);
			}
		}
	}
}