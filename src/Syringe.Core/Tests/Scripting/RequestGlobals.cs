using System.Net.Http;
using Syringe.Core.Configuration;

namespace Syringe.Core.Tests.Scripting
{
	public class RequestGlobals
	{
		public Test Test { get; set; }
		public HttpRequestMessage Request { get; set; }
		public Settings Settings { get; set; }
	}
}