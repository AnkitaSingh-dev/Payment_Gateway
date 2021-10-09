using System.Web.Optimization;

namespace Corno.Raychem.CustomerPortal
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                //-- jQuery -
                //"~/Scripts/jquery-{version}.js"
                "~/Scripts/jquery.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                //-- jQuery Valiate --
                "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                //-- Modernizer --
                "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/plugins").Include(

                // -- Nice Scroll --
                "~/Scripts/plugins/nicescroll/jquery.nicescroll.min.js",

                // -- jQuery UI --
                "~/Scripts/plugins/jquery-ui/jquery.ui.core.min.js",
                "~/Scripts/plugins/jquery-ui/jquery.ui.widget.min.js",
                "~/Scripts/plugins/jquery-ui/jquery.ui.mouse.min.js",
                "~/Scripts/plugins/jquery-ui/jquery.ui.draggable.min.js",
                "~/Scripts/plugins/jquery-ui/jquery.ui.resizable.min.js",
                "~/Scripts/plugins/jquery-ui/jquery.ui.sortable.min.js",
                "~/Scripts/plugins/touch-punch/jquery.touch-punch.min.js",
                "~/Scripts/jquery.js",
                "~/Scripts/jquery-ui.js",

                // -- slimScroll --
                "~/Scripts/plugins/slimscroll/jquery.slimscroll.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                // -- Bootstrap --
                "~/Scripts/bootstrap.min.js",

                // -- Bootbox --
                "~/Scripts/plugins/bootbox/bootbox.min.js",

                // -- dataTables --
                "~/Scripts/plugins/datatable/jquery.dataTables.min.js",
                "~/Scripts/plugins/datatable/TableTools.min.js",
                "~/Scripts/plugins/datatable/ColReorderWithResize.js",
                "~/Scripts/plugins/datatable/ColVis.min.js",
                "~/Scripts/plugins/datatable/jquery.dataTables.columnFilter.js",
                "~/Scripts/plugins/datatable/jquery.dataTables.grouping.js",
                "~/Scripts/plugins/vmap/jquery.vmap.min.js",
                "~/Scripts/plugins/vmap/jquery.vmap.world.js",
                "~/Scripts/plugins/vmap/jquery.vmap.sampledata.js",
                "~/Scripts/plugins/select2/select2.min.js",

                // -- form --
                "~/Scripts/plugins/form/jquery.form.min.js",

                // -- Validation --
                "~/Scripts/plugins/validation/jquery.validate.min.js",
                "~/Scripts/plugins/validation/additional-methods.min.js",

                // -- Chosen --
                "~/Scripts/plugins/chosen/chosen.jquery.min.js",
                "~/Scripts/plugins/icheck/jquery.icheck.min.js",

                // Timepicker
                "~/Scripts/plugins/timepicker/bootstrap-timepicker.min.js",

                // -- Theme framework --
                "~/Scripts/eakroko.min.js",

                // -- Theme scripts --
                "~/Scripts/application.min.js",

                // -- Just for demonstration --
                "~/Scripts/demonstration.min.js",
                "~/Scripts/respond.js",
                "~/ReportViewer/js/telerikReportViewer-9.1.15.624.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(

                // -- Bootstrap --
                "~/Content/bootstrap.min.css",

                // -- Bootstrap responsive --
                "~/Content/bootstrap-responsive.min.css",

                // -- jQuery UI --                
                "~/Content/plugins/jquery-ui/smoothness/jquery-ui.css",
                "~/Content/plugins/jquery-ui/smoothness/jquery.ui.theme.css",

                // -- dataTables --
                "~/Content/plugins/datatable/TableTools.css",
                "~/Content/plugins/plugins/pageguide/pageguide.css",
                "~/Content/plugins/fullcalendar/fullcalendar.css",
                "~/Content/plugins/fullcalendar/fullcalendar.print.css",

                // -- Chosen --
                "~/Content/plugins/chosen/chosen.css",
                "~/Content/plugins/select2/select2.css",
                "~/Content/plugins/icheck/all.css",

                // TimePicker
                "~/Content/plugins/timepicker/bootstrap-timepicker.min.css",

                // -- Theme CSS --
                "~/Content/style.css",

                // -- Color CSS --
                "~/Content/themes.css",
                //"~/Content/Kendo/kendo.common.min.css",
                //"~/Content/Kendo/kendo.default.min.css",
                "~/ReportViewer/styles/telerikReportViewer-9.1.15.624.css"

                ));

            // Kendo UI
            //bundles.Add(new ScriptBundle("~/bundles/kendo").Include(
            //    "~/Scripts/kendo/kendo.all.min.js",
            //    "~/Scripts/Kendo/cultures/kendo.culture.en-US.min.js",
            //    "~/Scripts/kendo/kendo.aspnetmvc.min.js"));
            //bundles.Add(new StyleBundle("~/Content/kendo/css").Include(
            //"~/Content/kendo/kendo.common.min.css",
            //"~/Content/kendo/kendo.default.min.css"));
            bundles.IgnoreList.Clear();
        }
    }
}