using RestSharp;

namespace Syringe.Core.Tests.Scripting
{
	public class RequestGlobals
	{
		public Test Test { get; set; }
		public IRestRequest Request { get; set; }
	}
}