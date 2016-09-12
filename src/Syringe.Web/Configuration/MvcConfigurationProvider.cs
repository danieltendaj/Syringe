using System.Collections.Specialized;
using System.Configuration;

namespace Syringe.Web.Configuration
{
    public class MvcConfigurationProvider : IMvcConfigurationProvider
    {
        private readonly NameValueCollection _configurationCollection;
        private MvcConfiguration _configuration;

        public MvcConfigurationProvider(NameValueCollection configurationCollection)
        {
            _configurationCollection = configurationCollection;
        }

        public MvcConfiguration Load()
        {
            if (_configuration == null)
            {
                string serviceUrl = _configurationCollection["serviceUrl"];
                if (string.IsNullOrEmpty(serviceUrl))
                {
                    serviceUrl = "http://localhost:1981";
                }
                _configuration = new MvcConfiguration { ServiceUrl = serviceUrl };
            }

            return _configuration;
        }
    }
}