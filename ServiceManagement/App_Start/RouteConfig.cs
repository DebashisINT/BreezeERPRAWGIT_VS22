﻿/***********************************************************************************************************************************
 * Rev 1.0      Sanchita    16/10/2024      0027747: Need to Implement existing SMS sending to Normal link instead of Bitly for GTPL
 * *********************************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ServiceManagement
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Rev 1.0
            routes.MapRoute(
                name: "CustomRoute",
                url: "Short/{id}",
                defaults: new { controller = "Short", action = "Index", id = UrlParameter.Optional }
            );
            // End of Rev 1.0

            routes.MapRoute(
             name: "Default1",
             url: "{controller}/{action}/{Company_Code}",
             defaults: new { controller = "Login", action = "login", Company_Code = UrlParameter.Optional }
             );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}

