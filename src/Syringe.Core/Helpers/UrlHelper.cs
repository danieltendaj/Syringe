using System;
using System.Globalization;

namespace Syringe.Core.Helpers
{
    public class UrlHelper :IUrlHelper
    {
        public string AddUrlBase(string baseUrl, string content)
        {
            content = content ?? string.Empty;

            if (!baseUrl.EndsWith("/"))
            {
                baseUrl = string.Concat(baseUrl, "/");
            }

            // add base tag to href
            var htmlUpdated = content.Replace("href=\"/", "href=\"" + baseUrl);
            // add base tag to src 
            htmlUpdated = htmlUpdated.Replace("src=\"/", "src=\"" + baseUrl);

            return htmlUpdated;
        }

        public string GetBaseUrl(string url)
        {
            Uri result;
            if (Uri.TryCreate(url, UriKind.Absolute, out result))
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}://{1}/", result.Scheme, result.Host);
            }

            return url;
        }
    }
}
