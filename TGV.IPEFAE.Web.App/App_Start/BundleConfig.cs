using System.Web;
using System.Web.Optimization;

namespace TGV.IPEFAE.Web.App
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.IgnoreList.Clear();

            //bundles.Add(new ScriptBundle("~/bundles/jqueryScript").Include(
            //            "~/Scripts/jquery-{version}.js"
            //            , "~/Scripts/jquery.blockUI.js"
            //            , "~/Scripts/jquery.mask.js"
            //            , "~/Scripts/jquery.slideUnlock.js"
            //            , "~/Scripts/jquery.addItems.js"
            //            , "~/Scripts/jquery.cascade.js"
            //            , "~/Scripts/jquery.jscroll.js"
            //            ));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.validate*"));

            //// Use the development version of Modernizr to develop with and learn from. Then, when you're
            //// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizrScript").Include(
            //            "~/Scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrapScript").Include(
            //          "~/Scripts/bootstrap.js",
            //          "~/Scripts/respond.js"));

            //bundles.Add(new StyleBundle("~/bundles/ContentCss").Include(
            //          "~/Content/bootstrap.css"
            //          , "~/Content/jquery.slideUnlock.css"
            //          , "~/Content/site.css"));
        }
    }
}
