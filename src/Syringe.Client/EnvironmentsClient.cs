using System.Collections.Generic;
using Syringe.Client.Http;
using Syringe.Core.Services;
using Environment = Syringe.Core.Environment.Environment;

namespace Syringe.Client
{
    public class EnvironmentsClient : IEnvironmentsService
    {
        internal readonly string ServiceUrl;
        private readonly FlurlWrapper _wrapper;

        public EnvironmentsClient(string serviceUrl)
        {
            ServiceUrl = serviceUrl;
            var factory = new CustomHttpClientFactory(serviceUrl);
            _wrapper = new FlurlWrapper(factory, "/api/environments");
        }

        public IEnumerable<Environment> Get()
        {
            return _wrapper.Get<List<Environment>>("").Result;
        }
    }
}