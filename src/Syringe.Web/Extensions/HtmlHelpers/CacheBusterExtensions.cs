using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Syringe.Web.Extensions.HtmlHelpers
{
    public static class CacheBusterExtensions
    {
        public static HtmlString GetCacheBuster(this HtmlHelper htmlHelper)
        {
            return new HtmlString($"?v={GetAssemblyVersion()}");
        }

        public static string GetAssemblyVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString().Replace(".", "-");
        }
    }
}