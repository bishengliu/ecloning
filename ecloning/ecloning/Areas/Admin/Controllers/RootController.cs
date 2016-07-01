using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ecloning.Areas.Admin.Controllers
{
    //only researcher and researcher have access
    [Authorize(Roles = "appAdmin, InstAdmin, groupLeader, Assistant")]
    //[AuthorizeArea(AllowIpAddresses = new[] { "1.1.1.1", "1.2.3.4" })]
    public class RootController : Controller
    {

    }
}