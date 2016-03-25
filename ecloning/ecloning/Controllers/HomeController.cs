using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ecloning.Controllers
{
    public class HomeController : RootController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}