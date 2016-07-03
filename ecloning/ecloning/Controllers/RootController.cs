﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ecloning.Controllers
{
    //instAdmin has no access
    [Authorize(Roles = "appAdmin, Researcher, GroupLeader, Assistant")]
    //[AuthorizeArea(AllowIpAddresses = new[] { "1.1.1.1", "1.2.3.4" })]
    public class RootController : Controller
    {

    }
}