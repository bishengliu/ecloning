﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ecloning.Areas.Admin.Controllers
{
    //researcher has no access
    [Authorize(Roles = "appAdmin, InstAdmin")]
    //[AuthorizeArea(AllowIpAddresses = new[] { "1.1.1.1", "1.2.3.4" })]
    public class RootController : Controller
    {

    }
}