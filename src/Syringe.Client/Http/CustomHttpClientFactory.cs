using System;
using System.Net;
using System.Net.Http;
using Flurl;
using Flurl.Http.Configuration;

namespace Syringe.Client.Http
{
    public class CustomHttpClientFactory : IHttpClientFactory
    {
        public string BaseUrl { get; set; }

        public CustomHttpClientFactory(string baseUrl)
        {
            BaseUrl = baseUrl;
        }

        public HttpClient CreateClient(Url url, HttpMessageHandler handler)
        {
            var client = new HttpClient(CreateMessageHandler());
            client.BaseAddress = new Uri(BaseUrl);

            return client;
        }

        public HttpMessageHandler CreateMessageHandler()
        {
            var messageHandler = new HttpClientHandler()
            {
                AllowAutoRedirect = true,
                CookieContainer = new CookieContainer(),
                ServerCertificateCustomValidationCallback = (w, x, y, z) => true
            };

            return messageHandler;
        }
    }
}