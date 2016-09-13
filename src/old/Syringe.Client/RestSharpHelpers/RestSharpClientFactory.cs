using RestSharp;

namespace Syringe.Client.RestSharpHelpers
{
    public class RestSharpClientFactory : IRestSharpClientFactory
    {
        public IRestClient Create(string serviceUrl)
        {
            return new RestClient(serviceUrl);
        }
    }
}