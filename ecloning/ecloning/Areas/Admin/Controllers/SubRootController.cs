using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ecloning.Areas.Admin.Controllers
{
    [Authorize(Roles = "GroupLeader, Assistant, appAdmin, InstAdmin")]
    public class SubRootController : Controller
    {
       
    }
}