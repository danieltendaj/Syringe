namespace Syringe.Web.Configuration
{
	public class MvcConfiguration
	{
		public string ServiceUrl { get; set; }

		internal MvcConfiguration()
		{
			ServiceUrl = "http://localhost:1981";
		}
	}
}