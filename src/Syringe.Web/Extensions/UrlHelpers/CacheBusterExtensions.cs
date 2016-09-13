using System;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Syringe.Web.Extensions.UrlHelpers
{
    public static class CacheBusterExtensions
    {
        internal static Func<UrlHelper, string, string> ResolvePath = (helper, path) => helper.Content(path);

        public static HtmlString GetCacheBuster(this UrlHelper urlHelper, string path)
        {
            return new HtmlString($"{ResolvePath(urlHelper, path)}?v={GetAssemblyVersion()}");
        }

        public static string GetAssemblyVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString().Replace(".", "-");
        }
    }
}