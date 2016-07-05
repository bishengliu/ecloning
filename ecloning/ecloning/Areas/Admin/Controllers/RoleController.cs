using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ecloning.Areas.Admin.Controllers
{
    public class RoleController : RootController
    {
        // GET: Admin/Role
        [Authorize(Roles = "appAdmin, InstAdmin")]
        public ActionResult Index()
        {
            return View();
        }
    }
}