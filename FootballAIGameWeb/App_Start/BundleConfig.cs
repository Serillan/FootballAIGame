using System.Web;
using System.Web.Optimization;

namespace FootballAIGameWeb
{
    public class BundleConfig
    {
        /// <summary>
        /// Registers the bundles. Bundling is a feature that combines multiple files into a single file.
        /// Also it performs a variety of different code optimizations.
        /// </summary>
        /// <param name="bundles">The bundles collection.</param>
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/libraries").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/datatables/jquery.datatables.js",
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/respond.js",
                        "~/Scripts/datatables/datatables.bootstrap.js",
                        "~/Scripts/toastr.js",
                        "~/Scripts/jquery-editable-select.js",
                        "~/Scripts/jquery.binarytransport.js"));

            bundles.Add(new ScriptBundle("~/bundles/custom").Include(
                        "~/Scripts/CustomScripts/clickable-row.js",
                        "~/Scripts/CustomScripts/selectable-row.js",
                        "~/Scripts/CustomScripts/initialization.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/datatables/css/datatables.bootstrap.css",
                      "~/Content/bootstrap-cerulean.css",
                      "~/Content/toastr.css",
                      "~/Content/site.css",
                      "~/Content/jquery-editable-select.css"));
        }
    }
}
