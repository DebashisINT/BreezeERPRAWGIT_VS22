﻿using System.Web;
using System.Web.Optimization;

namespace ServiceManagement
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/assests/js/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/assests/js/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/assests/js/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/assests/bootstrap/js/bootstrap.js",
                      "~/assests/js/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/assests/bootstrap/css/bootstrap.css",
                      "~/assests/css/site.css"));
        }
    }
}
