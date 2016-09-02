using System;

namespace Syringe.Web.Configuration
{
	public class MvcConfiguration
	{
		private readonly Lazy<string> _lazySignalRUrl;

		public string ServiceUrl { get; set; }
		public string SignalRUrl => _lazySignalRUrl.Value;

		internal MvcConfiguration()
		{
			_lazySignalRUrl = new Lazy<string>(BuildSignalRUrl);
			ServiceUrl = "http://localhost:1981";
		}
        
		private string BuildSignalRUrl()
		{
			var builder = new UriBuilder(ServiceUrl);
			builder.Path = "signalr";
			return builder.ToString();
		}
	}
}