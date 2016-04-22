using System;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using Syringe.Client;
using Syringe.Core.Configuration;
using Syringe.Core.Security;
using Syringe.Core.Security.OAuth2;
using Syringe.Web.Models;

namespace Syringe.Web.Controllers
{
    public class AuthenticationController : Controller
    {
        public ActionResult Login(string returnUrl)
        {
            var model = new AuthenticationViewModel();

			IConfiguration config = GetConfig();
	        model.Configuration = config;
			model.ReturnUrl = returnUrl;

            return View(model);
		}

	    private static IConfiguration GetConfig()
	    {
		    var mvcConfiguration = MvcConfiguration.Load();
			var configClient = new ConfigurationClient(mvcConfiguration.ServiceUrl);
		    IConfiguration config = configClient.GetConfiguration();
		    return config;
	    }

	    [HttpPost]
		public ActionResult Login(string provider, string returnUrl)
		{
			// Request a redirect to the external login provider
			return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Authentication", new { provider = provider, returnUrl = returnUrl }));
		}

		public ActionResult Logout()
	    {
			FormsAuthentication.SignOut();
			Response.Cookies["SyringeOAuth"].Expires = DateTime.Now.AddDays(-1);
			Response.Cookies[".AspNet.Cookies"].Expires = DateTime.Now.AddDays(-1);

		    return Redirect("/");
	    }

		public ActionResult Noop()
		{
			// Just for you Microsoft
			return Redirect("/");
		}

		public ActionResult ClaimsDebug()
		{
			var claims = ClaimsPrincipal.Current.Claims.ToList();

			string debugInfo = "";
			foreach (Claim claim in claims)
			{
				debugInfo += $"{claim.Type} : {claim.Value}\n";
			}

			return Content(debugInfo);
		}

		public ActionResult ExternalLoginCallback(string returnUrl, string provider)
		{
			var claims = ClaimsPrincipal.Current.Claims.ToList();
			var nameIdentifier = claims.FirstOrDefault(x => x.Type.Equals(UrnLookup.GetNamespaceForName(provider), StringComparison.InvariantCultureIgnoreCase));
			var uidIdentifier = claims.FirstOrDefault(x => x.Type.Equals(UrnLookup.GetNamespaceForId(provider), StringComparison.InvariantCultureIgnoreCase));

			if (nameIdentifier == null || uidIdentifier == null)
			{
				string debugInfo = "";
				foreach (Claim claim in claims)
				{
					debugInfo += $"{claim.Type} : {claim.Value}\n";
				}

				throw new InvalidOperationException("The OAuth provider didn't provide a name or nameidentifier:\n " + debugInfo);
			}

			string id = uidIdentifier.Value;
			string name = nameIdentifier.Value;

			string userData = JsonConvert.SerializeObject(new UserContext() {FullName = name, Id = id});
			string encryptedData = FormsAuthentication.Encrypt(new FormsAuthenticationTicket(1, "Syringe", DateTime.Now, DateTime.UtcNow.AddDays(1), true, userData));

			// Add UserData to the forms auth cookie by setting the cookie manually.
			Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encryptedData)
			{
				Expires = DateTime.Now.AddDays(1)
			});

			return Redirect(returnUrl);
		}

		private class ChallengeResult : HttpUnauthorizedResult
		{
			private readonly string _loginProvider;
			private readonly string _redirectUri;

			public ChallengeResult(string provider, string redirectUri)
			{
				_loginProvider = provider;
				_redirectUri = redirectUri;
			}

			public override void ExecuteResult(ControllerContext context)
			{
				var properties = new AuthenticationProperties() { RedirectUri = _redirectUri };
				context.HttpContext.GetOwinContext().Authentication.Challenge(properties, _loginProvider);
			}
		}
	}
}