using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ecloning.Areas.Admin.Controllers
{
    public class DashboardController : RootController
    {
        // GET: Admin/Dashboard
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
    }
}