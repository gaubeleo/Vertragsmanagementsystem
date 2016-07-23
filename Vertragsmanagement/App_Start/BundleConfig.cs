using System.Web.Optimization;

namespace Vertragsmanagement
{
    /// <summary>
    /// BundleConfig
    /// </summary>
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        /// <summary>
        /// Central configuration of all availible Bundle packets for using in the WebApp.
        /// </summary>
        /// <param name="bundles"></param>
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/jQueryValidateFixes.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js",
                      "~/DataTables/datatables.js",
                      "~/Scripts/Modals.js"));

            bundles.Add(new StyleBundle("~/Content/Contract").Include(
                      "~/Content/jquery.dataTables.yadcf.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/DataTables/datatables.css"));

            bundles.Add(new ScriptBundle("~/bundles/Contract").Include(
                      "~/Scripts/moment*",
                      "~/Scripts/de.js",
                      "~/Scripts/bootstrap-datetimepicker*",
                      "~/Scripts/jquery.dataTables.yadcf.js",
                      "~/Scripts/Contract.js"));

            bundles.Add(new ScriptBundle("~/bundles/ContractCreate").Include(
                      "~/Scripts/ContractCreate.js"));

            bundles.Add(new ScriptBundle("~/bundles/ContractEdit").Include(
                      "~/Scripts/ContractEdit.js"));

            bundles.Add(new ScriptBundle("~/bundles/ContractCreateEdit").Include(
                      "~/Scripts/moment*",
                      "~/Scripts/de.js",
                      "~/Scripts/bootstrap-datetimepicker*",
                      "~/Scripts/jquery.dataTables.yadcf.js"));

            bundles.Add(new ScriptBundle("~/bundles/CoreData").Include(
                     "~/Scripts/CoreData.js"));

            bundles.Add(new ScriptBundle("~/bundles/Home").Include(
                     "~/Scripts/Home.js"));

            bundles.Add(new ScriptBundle("~/bundles/Report").Include(
                    "~/Scripts/Report.js"));
            
            //Enable optimization when debugging is disabled
            #if DEBUG
                    BundleTable.EnableOptimizations = false;
            #else
                    BundleTable.EnableOptimizations = true;
            #endif
        }
    }
}
