#region IMPORT NAMESPACE
using System.Web;
using System.Web.Optimization;
#endregion
namespace Sample
{
    public class BundleConfig
    {
        #region METHODS
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
            "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryunobtrusive").Include(
                        "~/Scripts/jquery.unobtrusive*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/Plugin/bootbox.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jsTreeLessonTask").Include(
                      "~/Scripts/jstree.min.js",
                      "~/Scripts/Custom/Lesson.js",
                      "~/Scripts/Custom/Task.js"));

            bundles.Add(new ScriptBundle("~/bundles/jcrop").Include(
                      "~/Scripts/jquery.Jcrop.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryform").Include(
                      "~/Scripts/jquery.form.js"));

            bundles.Add(new ScriptBundle("~/bundles/avatar").Include(
                      "~/Scripts/site.avatar.js"));

            bundles.Add(new ScriptBundle("~/bundles/blockUI").Include(
                      "~/Scripts/jquery.blockUI.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jsTree").Include(
                      "~/Scripts/jstree.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/common").Include(
                     "~/Scripts/Common.js"));

            bundles.Add(new ScriptBundle("~/bundles/topic").Include(
                     "~/Scripts/Custom/Topic.js"));

            bundles.Add(new ScriptBundle("~/bundles/lesson").Include(
                   "~/Scripts/Custom/Lesson.js",
                   "~/Scripts/Custom/Task.js"));

            bundles.Add(new ScriptBundle("~/bundles/task").Include(
                   "~/Scripts/Custom/Task.js"));

            bundles.Add(new ScriptBundle("~/bundles/volunteerplayingchess").Include(
                    "~/Scripts/Custom/VolunteerPlayingChess.js"
                    ));

            bundles.Add(new ScriptBundle("~/bundles/studentplayingchess").Include(
                    "~/Scripts/Custom/StudentPlayingChess.js"
                    ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Scripts/Plugin/Chess/css/bundle").Include(
                      "~/Scripts/Plugin/Chess/css/chessboard-0.3.0.css"));

            bundles.Add(new StyleBundle("~/Dashboard/css").Include(
                      "~/Content/style_dashboard.css",
                      "~/Content/font-awesome.min.css",
                      "~/Content/responsive-dashboard.css"));

            bundles.Add(new StyleBundle("~/Search/css").Include(
            "~/Content/style_search.css",
            "~/Content/font-awesome.min.css",
            "~/Content/responsive_search.css"));

            bundles.Add(new StyleBundle("~/Content/jcrop").Include(
                      "~/Content/jquery.Jcrop.css"));

            bundles.Add(new StyleBundle("~/Content/avatar").Include(
                      "~/Content/site.avatar.css"));

            bundles.Add(new StyleBundle("~/Content/jsTree").Include(
                      "~/Content/themes/default/style.min.css"));

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            #if DEBUG
                        BundleTable.EnableOptimizations = false;
            #else
                        BundleTable.EnableOptimizations = true;
            #endif
        }
        #endregion
    }
}
