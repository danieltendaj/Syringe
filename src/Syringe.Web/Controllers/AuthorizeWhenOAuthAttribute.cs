using System.Web;
using System.Web.Mvc;
using Syringe.Client;
using Syringe.Core.Configuration;

namespace Syringe.Web.Controllers
{
	/// <summary>
	/// Sets the method/class to require authentication, if an OAuth provider is set in the configuration file.
	///
	/// If none is set, then anonymous authentication is allowed. UserContext.GetFromFormsAuth sets the
	/// logged in user as Guest"
	/// </summary>
	public class AuthorizeWhenOAuthAttribute : AuthorizeAttribute
	{
		private readonly IConfiguration _config;

		public AuthorizeWhenOAuthAttribute()
		{
			//
			// TODO: get rid of bastard DI. This will require DI wireup of the attribute:
			// (example: https://github.com/roadkillwiki/roadkill/blob/master/src/Roadkill.Core/DependencyResolution/MVC/MvcAttributeProvider.cs)
			//

			MvcConfiguration mvcConfiguration = MvcConfiguration.Load();
			var configClient = new ConfigurationClient(mvcConfiguration.ServiceUrl);
			_config = configClient.GetConfiguration();
		}

		internal AuthorizeWhenOAuthAttribute(IConfiguration config)
		{
			_config = config;
		}

		/// <summary>
		/// For internal testing.
		/// </summary>
		internal bool RunAuthorizeCore(HttpContextBase httpContext)
		{
			return AuthorizeCore(httpContext);
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
			return (!string.IsNullOrEmpty(config.OAuthConfiguration.GoogleAuthClientId) && !string.IsNullOrEmpty(config.OAuthConfiguration.GoogleAuthClientSecret))
					|| (!string.IsNullOrEmpty(config.OAuthConfiguration.MicrosoftAuthClientId) && string.IsNullOrEmpty(config.OAuthConfiguration.MicrosoftAuthClientSecret))
			        || (!string.IsNullOrEmpty(config.OAuthConfiguration.GithubAuthClientId) && !string.IsNullOrEmpty(config.OAuthConfiguration.GithubAuthClientSecret));
		}
	}
}