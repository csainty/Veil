﻿using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Veil.Mvc5.TestSite
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new VeilViewEngine());
        }
    }
}
