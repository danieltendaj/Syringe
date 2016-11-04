using System.Web;
using System.Web.Mvc;
using Syringe.Core.Configuration;
using Syringe.Core.Extensions;

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
		public IConfiguration Configuration { get; set; }
        
		/// <summary>
		/// For internal testing.
		/// </summary>
		internal bool RunAuthorizeCore(HttpContextBase httpContext)
		{
			return AuthorizeCore(httpContext);
		}

		protected override bool AuthorizeCore(HttpContextBase httpContext)
		{
		    if (Configuration.ContainsOAuthCredentials())
			{
				return base.AuthorizeCore(httpContext);
			}

            return true;
		}
	}
}