﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace RssReader.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                     name: "ActionApi",
                     routeTemplate: "api/{controller}/{action}/{id}",
                     defaults: new { id = RouteParameter.Optional }
                 );

            config.EnableQuerySupport();

            // To disable tracing in your application, please comment out or remove the following line of code
            // For more information, refer to: http://www.asp.net/web-api
            config.EnableSystemDiagnosticsTracing();
        }
    }
}
