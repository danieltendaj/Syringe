using System.Web;
using System.Web.Mvc;
using Syringe.Client;
using Syringe.Core.Configuration;

namespace Syringe.Web.Controllers
{
	// TODO: get rid of bastard DI
	// (example: https://github.com/roadkillwiki/roadkill/blob/master/src/Roadkill.Core/DependencyResolution/MVC/MvcAttributeProvider.cs)
	public class AuthorizeWhenOAuthAttribute : AuthorizeAttribute
	{
		private readonly IConfiguration _config;

		public AuthorizeWhenOAuthAttribute()
		{
			MvcConfiguration mvcConfiguration = MvcConfiguration.Load();
			var configClient = new ConfigurationClient(mvcConfiguration.ServiceUrl);
			_config = configClient.GetConfiguration();
		}

		internal AuthorizeWhenOAuthAttribute(IConfiguration config)
		{
			_config = config;
		}

		protected override bool AuthorizeCore(HttpContextBase httpContext)
		{
			if (HasAuthenticationProviders(_config))
			{
				return base.AuthorizeCore(httpContext);
			}
			else
			{
				return true;
			}
		}

		private bool HasAuthenticationProviders(IConfiguration config)
		{
			return !string.IsNullOrEmpty(config.OAuthConfiguration.GoogleAuthClientId) &&
			       !string.IsNullOrEmpty(config.OAuthConfiguration.GoogleAuthClientSecret) &&
			       !string.IsNullOrEmpty(config.OAuthConfiguration.MicrosoftAuthClientId) &&
			       !string.IsNullOrEmpty(config.OAuthConfiguration.MicrosoftAuthClientSecret) &&
			       !string.IsNullOrEmpty(config.OAuthConfiguration.GithubAuthClientId) &&
			       !string.IsNullOrEmpty(config.OAuthConfiguration.GithubAuthClientSecret);
		}
	}
}