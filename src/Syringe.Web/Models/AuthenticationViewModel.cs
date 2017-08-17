using Syringe.Core.Configuration;

namespace Syringe.Web.Models
{
	public class AuthenticationViewModel
	{
		public IConfiguration Configuration { get; set; }
		public string ReturnUrl { get; set; }

		public bool IsOAuthConfigEmpty
		{
			get
			{
				return string.IsNullOrEmpty(Configuration.Settings.OAuthConfiguration.MicrosoftAuthClientId) &&
					   string.IsNullOrEmpty(Configuration.Settings.OAuthConfiguration.GoogleAuthClientId) &&
					   string.IsNullOrEmpty(Configuration.Settings.OAuthConfiguration.GithubAuthClientId);
			}
		}
	}
}