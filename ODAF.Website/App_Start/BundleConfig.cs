using BundleTransformer.Core.Orderers;
using BundleTransformer.Core.Transformers;
using System.Web;
using System.Web.Optimization;

namespace ODAF.Website
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            /*
      * Styles
      */
            bundles.Add(new StyleBundle("~/bundles/foundation/style").Include(
                "~/Content/Styles/foundation/foundation.css"));

            bundles.Add(new StyleBundle("~/bundles/themes/base/css").Include(
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

            var cssTransformer = new CssTransformer();
            var nullOrderer = new NullOrderer();

            var css = new StyleBundle("~/bundles/style").Include(
                "~/Content/Styles/Variables.less",
                "~/Content/Styles/Layout.less",
                "~/Content/Styles/Icons.less",
                "~/Content/Styles/Index.less",
                "~/Content/Styles/Site.less");

            css.Transforms.Add(cssTransformer);
            css.Orderer = nullOrderer;

            bundles.Add(css);


            /*
             * Scripts
             */
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Content/Scripts/vendor/modernizr.js"));

            bundles.Add(new ScriptBundle("~/bundles/foundation/scripts").Include(
                "~/Content/Scripts/vendor/foundation.js",
                "~/Content/Scripts/vendor/app.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                "~/Content/Scripts/vendor/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/layout").Include(
                "~/Content/Scripts/Shared/_Layout.js"));
        }
    }
}
