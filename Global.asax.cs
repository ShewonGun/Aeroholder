using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AeroHolder_new
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            // Register MVC areas
            AreaRegistration.RegisterAllAreas();
            
            // Register MVC routes
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}