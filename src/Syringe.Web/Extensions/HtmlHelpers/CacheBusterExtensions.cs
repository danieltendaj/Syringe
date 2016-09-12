using System;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Syringe.Web.Extensions.HtmlHelpers
{
    public static class CacheBusterExtensions
    {
        internal static Func<string, string> MapServerPath = HostingEnvironment.MapPath;

        public static HtmlString GetCacheBuster(this HtmlHelper htmlHelper, string path)
        {
            return new HtmlString($"{MapServerPath(path)}?v={GetAssemblyVersion()}");
        }

        public static string GetAssemblyVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString().Replace(".", "-");
        }
    }
}