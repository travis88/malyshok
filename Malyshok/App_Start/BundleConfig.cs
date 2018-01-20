using System.Web;
using System.Web.Optimization;

namespace Disly
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // --------- Скрипты ---------            
            bundles.Add(new ScriptBundle("~/bundles/script").Include(
                "~/Content/plugins/bootstrap/js/bootstrap.min.js",
                "~/Content/plugins/bootstrap/js/bootstrap-toggle.js",
                "~/Content/plugins/bootstrap/js/bootstrap-select.js",
                "~/Content/plugins/mCustomScrollbar/jquery.mCustomScrollbar.js",
                "~/Content/plugins/jquery/jquery.mask.min.js",
                "~/Content/plugins/Disly/DislyControls.js",
                "~/scripts/cms/disly_5.js"                
                ));
            
            bundles.Add(new ScriptBundle("~/bundles/popUp_js").Include(
                "~/Content/plugins/bootstrap/js/bootstrap.min.js",
                "~/Content/plugins/bootstrap/js/bootstrap-toggle.js",
                "~/Content/plugins/mCustomScrollbar/jquery.mCustomScrollbar.js",
                //"~/Content/plugins/Disly/DislyControls.js",
                "~/Scripts/cms/disly_5_popup.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Content/plugins/jquery/jquery.js",
                "~/Content/plugins/jquery/jquery.ui.js"));

            //js plugins: select2, icheck
            bundles.Add(new ScriptBundle("~/bundles/jq_plugins/js").Include(
               "~/Content/plugins/select2/select2.min.js",
               "~/Content/plugins/select2/i18n/ru.js",
               "~/Content/plugins/icheck/icheck.min.js",
               "~/Content/plugins/datatables/datatables.min.js",
                "~/Content/plugins/datatables/dataTables.bootstrap.min.js"
               ));
            bundles.Add(new StyleBundle("~/bundles/jq_plugins/css").Include(
              "~/Content/plugins/select2/css/select2.css",
              "~/Content/plugins/select2/css/select2_custom.css",
              "~/Content/plugins/icheck/skins/square/_all.css",
              "~/Content/plugins/datatables/datatables.min.css",
              "~/Content/plugins/datatables/dataTables.bootstrap.min.css"
              ));

           


            // --------- Стили ---------
            bundles.Add(new StyleBundle("~/bundles/css").Include(
                "~/Content/plugins/bootstrap/css/bootstrap.css",
                "~/Content/plugins/mCustomScrollbar/jquery.mCustomScrollbar.css",
                "~/Content/plugins/bootstrap/css/bootstrap-select.css", 
                "~/Content/plugins/Disly/DislyControls.css",
                "~/Content/css/styles.css"
                ));


            bundles.Add(new StyleBundle("~/bundles/popUp_css").Include(
                "~/Content/plugins/bootstrap/css/bootstrap.min.css",
                "~/Content/plugins/mCustomScrollbar/jquery.mCustomScrollbar.css",
                "~/Content/plugins/bootstrap/css/bootstrap-select.css", 
                "~/Content/plugins/Disly/DislyControls.css",
                "~/Content/css/styles_popUp.css"));

        }
    }
}
