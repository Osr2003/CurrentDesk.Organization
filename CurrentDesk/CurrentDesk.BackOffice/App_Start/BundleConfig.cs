using System.Web;
using System.Web.Optimization;

namespace CurrentDesk.BackOffice
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/uniform").Include(
                "~/Scripts/jquery.uniform.js"));

            bundles.Add(new ScriptBundle("~/bundles/choosen").Include(
               "~/Scripts/chosen.jquery.js"));

            bundles.Add(new ScriptBundle("~/bundles/upload").Include(
              "~/Scripts/jquery.upload.js"));

            bundles.Add(new ScriptBundle("~/bundles/tip").Include(
                "~/Scripts/jquery.tipTip.minified.js"));

            bundles.Add(new ScriptBundle("~/bundles/morris").Include(
                "~/Scripts/MorrisPlugin/morris.js", "~/Scripts/MorrisPlugin/raphael-2.1.0.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqgrid").Include(
             "~/Scripts/jquery.jqGrid-4.4.4/js/jquery.jqGrid.js", "~/Scripts/jquery.jqGrid-4.4.4/js/i18n/grid.locale-en.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap-datepicker").Include(
                "~/Scripts/bootstrap-datepicker.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));



            //bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css", "~/Content/style.css", "~/Content/uniform.css"));
            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/SignUpStyle.css"));
            bundles.Add(new StyleBundle("~/Content/commonStyle").Include("~/Content/Common.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));
        }
    }
}