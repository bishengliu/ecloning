using System.Web;
using System.Web.Optimization;

namespace ecloning
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",                        
                        "~/Scripts/chosen.jquery.js",
                        "~/Scripts/jquery-ui-{version}.js",                        
                        "~/Scripts/jquery.tokeninput.js"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/bootstrap-datepicker.js",
                      "~/Scripts/bootstrap-datepicker.min.js",
                      "~/Scripts/datepicker.js",
                      "~/Scripts/respond.js"));
            bundles.Add(new ScriptBundle("~/bundles/d3").Include(
                      "~/Scripts/d3/d3*"));

            bundles.Add(new ScriptBundle("~/bundles/editable").Include(
                      "~/Scripts/bootstrap3-editable/js/bootstrap-editable.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/giraffe").Include(
                      "~/Scripts/giraffe/analyze.js",
                      "~/Scripts/giraffe/bio.js",
                      "~/Scripts/giraffe/draw.js",
                      "~/Scripts/giraffe/raphael-min.js",
                      "~/Scripts/giraffe/scale.raphael.js"));

            bundles.Add(new ScriptBundle("~/bundles/giraffe2").Include(
                        "~/Scripts/giraffe/analyze-2.0.js",
                        "~/Scripts/giraffe/analyze_page-2.0.js",
                        "~/Scripts/giraffe/bio-2.0.js",
                        "~/Scripts/giraffe/draw-2.0.js",
                        "~/Scripts/giraffe/raphael-min-2.0.js",
                        "~/Scripts/giraffe/jquery.ba-replacetext.mim-2.0.js",
                        "~/Scripts/giraffe/scale.raphael-2.0.js"));

            bundles.Add(new ScriptBundle("~/bundles/nvd3").Include(
                      "~/Scripts/nv.d3.min.js"));

            //less bundle
            bundles.Add(new LessBundle("~/Content/less").Include(
                "~/Content/less/*.less"));

            bundles.Add(new StyleBundle("~/Content/biotools").Include(
                    "~/Content/biotools/metisMenu.min.css",
                    "~/Content/biotools/timeline.css",
                    "~/Content/biotools/morris.css",
                    "~/Content/sb-admin-2.css"));

            bundles.Add(new ScriptBundle("~/bundles/biotools").Include(
                        "~/Scripts/biotools/morris.min.js",
                        "~/Scripts/biotools/raphael-min.js",
                        "~/Scripts/biotools/sb-admin-2.js",
                      "~/Scripts/biotools/metisMenu.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/font-awesome/font-awesome.min.css",
                      "~/Content/bootstrap-datepicker.min.css",
                      "~/Content/nv.d3.css",
                      "~/Content/token-input.css",
                      "~/Content/token-input-facebook.css",
                      "~/Content/token-input-mac.css",
                      "~/Content/chosen.css",
                      "~/Content/bootstrap3-editable/css/bootstrap-editable.css",
                      "~/Content/dataTables.bootstrap.min.css",
                      "~/Content/site.css"));

        }
    }
}
