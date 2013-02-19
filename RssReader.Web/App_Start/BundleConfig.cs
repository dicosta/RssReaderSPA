using System.Web;
using System.Web.Optimization;

namespace RssReader.Web
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            #region Scripts Bundles

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            /*
            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));
            */
            
            bundles.Add(new ScriptBundle("~/bundles/jquery-validate").Include(
            "~/Scripts/jquery.validate.js",
            "~/scripts/jquery.validate.unobtrusive.js",
            "~/Scripts/jquery.validate.unobtrusive-custom-for-bootstrap*"
            ));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/bootstrap.js"));           

            bundles.Add(new ScriptBundle("~/bundles/ajaxlogin").Include(
                "~/Scripts/app/ajaxlogin.js"));

            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                "~/Scripts/angular.js",
                "~/Scripts/angular-ui.js"));

            bundles.Add(new ScriptBundle("~/bundles/plugins").Include(
                "~/Scripts/spin.js",
                "~/Scripts/select2.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                "~/Scripts/app/services.js",
                "~/Scripts/app/directives.js",
                "~/Scripts/app/filters.js",
                "~/Scripts/app/app.js",
                "~/Scripts/app/controllers.js"
                ));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            /*
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));
            */

            #endregion

            #region Style Bundles

            bundles.Add(new StyleBundle("~/content/css").Include(
                "~/Content/RssReader.css",
                "~/Content/bootstrap.css",
                "~/Content/select2.css",
                "~/Content/bootstrap-responsive.css"));

            /*
            bundles.Add(new StyleBundle("~/content/rssreaderstyle").Include(
                "~/Content/RssReader.css"
                ));

            bundles.Add(new StyleBundle("~/content/css").Include(
                "~/Content/bootstrap.css",
                "~/Content/select2.css"
            ));

            bundles.Add(new StyleBundle("~/content/css-responsive").Include(
                "~/Content/bootstrap-responsive.css"
                ));

            
            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(a  
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
             */

            #endregion
        }
    }
}