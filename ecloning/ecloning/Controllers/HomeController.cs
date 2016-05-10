using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

//[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Web.config", Watch = true)]
namespace ecloning.Controllers
{

    public class HomeController : RootController
    {
        //private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(HomeController));
        public ActionResult Index()
        {
            //log.Info("Action Index has been fired.");
            return View();
        }
    }
}