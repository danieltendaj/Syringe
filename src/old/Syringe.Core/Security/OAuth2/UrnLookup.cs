namespace Syringe.Core.Security.OAuth2
{
	public class UrnLookup
	{
		public static string GetNamespaceForId()
		{
			return "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
		}

		public static string GetNamespaceForName(string provider)
		{
			provider = provider?.ToLower();

			switch (provider)
			{
				case "github":
					return "urn:github:name";
				default:
					return "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
			}
		}
	}
}