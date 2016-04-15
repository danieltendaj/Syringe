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
			    return string.IsNullOrEmpty(Configuration.OAuthConfiguration.MicrosoftAuthClientId) &&
			           string.IsNullOrEmpty(Configuration.OAuthConfiguration.GoogleAuthClientId) &&
			           string.IsNullOrEmpty(Configuration.OAuthConfiguration.GithubAuthClientId);
		    }
	    }
    }
}