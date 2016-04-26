using System.Web.Optimization;

namespace Syringe.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/signalR").Include(
                    "~/Scripts/jquery-{version}.js",
                    "~/Scripts/jquery.signalR-{version}.js")
        );

        }
    }
}
