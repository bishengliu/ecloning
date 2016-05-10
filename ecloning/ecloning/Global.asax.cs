using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ecloning
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }


        public void application_error(Object sender, EventArgs e)
        {
            ILog log = LogManager.GetLogger(typeof(MvcApplication).FullName);
            Exception error = Server.GetLastError();
            log.Error(error.Message);
            log.Error(error.StackTrace);
        }

    }
}
