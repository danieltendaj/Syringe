namespace Syringe.Core.Configuration
{
	public class OAuthConfiguration
	{
		public string GithubAuthClientId { get; set; }
        public string GithubAuthClientSecret { get; set; }

        public string GithubEnterpriseAuthorizationEndpoint { get; set; }
        public string GithubEnterpriseTokenEndpoint { get; set; }
        public string GithubEnterpriseUserInfoEndpoint { get; set; }

        public string GoogleAuthClientId { get; set; }
		public string GoogleAuthClientSecret { get; set; }

		public string MicrosoftAuthClientId { get; set; }
		public string MicrosoftAuthClientSecret { get; set; }

		public OAuthConfiguration()
		{
			GithubAuthClientId = "";
			GithubAuthClientSecret = "";
			GoogleAuthClientId = "";
			GoogleAuthClientSecret = "";
			MicrosoftAuthClientId = "";
			MicrosoftAuthClientSecret = "";
		}

	    public bool ContainsGithubEnterpriseSettings()
	    {
	        return !string.IsNullOrEmpty(GithubEnterpriseAuthorizationEndpoint) &&
	               !string.IsNullOrEmpty(GithubEnterpriseTokenEndpoint) &&
	               !string.IsNullOrEmpty(GithubEnterpriseUserInfoEndpoint);
	    }
	}
}