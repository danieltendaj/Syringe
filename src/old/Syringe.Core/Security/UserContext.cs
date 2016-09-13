using System;
using System.Web;
using System.Web.Security;
using Newtonsoft.Json;

namespace Syringe.Core.Security
{
	public class UserContext : IUserContext
	{
		public string Id { get; set; }
		public string FullName { get; set; }
		public bool IsGuest { get; set; }

		public static UserContext GetFromFormsAuth(HttpContextBase httpContext)
		{
			if (httpContext == null || httpContext.Request.Cookies[FormsAuthentication.FormsCookieName] == null)
				return new UserContext() { Id = "Guest", FullName = "Guest", IsGuest = true};

			string cookie = httpContext.Request.Cookies[FormsAuthentication.FormsCookieName].Value;
			FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie);
			string data = ticket.UserData;

			if (string.IsNullOrEmpty(data))
				throw new InvalidOperationException("The user cookie data is invalid. Please clear all cookies for this domain and re-login.");

			UserContext context = JsonConvert.DeserializeObject<UserContext>(data);

			return context;
		}

		public static string GetUserName(HttpContextBase httpContext)
		{
			UserContext context = GetFromFormsAuth(httpContext);
			return context.FullName;
		}
	}
}
