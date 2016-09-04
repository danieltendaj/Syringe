using System.Web;
using System.Web.Mvc;
using Syringe.Client;
using Syringe.Core.Configuration;
using Syringe.Core.Extensions;
using Syringe.Web.Configuration;

namespace Syringe.Web.Controllers.Attribute
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

			var provider = new MvcConfigurationProvider(new ConfigLocator());
		    MvcConfiguration mvcConfiguration = provider.Load();

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
			if (_config.ContainsOAuthCredentials())
			{
				return base.AuthorizeCore(httpContext);
			}
			else
			{
				return true;
			}
		}
	}
}