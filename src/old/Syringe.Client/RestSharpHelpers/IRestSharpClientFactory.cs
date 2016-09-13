using RestSharp;

namespace Syringe.Client.RestSharpHelpers
{
    public interface IRestSharpClientFactory
    {
        IRestClient Create(string serviceUrl);
    }
}