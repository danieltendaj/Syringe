namespace Syringe.Core.Helpers
{
    public interface IUrlHelper
    {
        string AddUrlBase(string baseUrl, string content);
        string GetBaseUrl(string url);
    }
}