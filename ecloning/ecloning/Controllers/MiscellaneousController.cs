using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ecloning.Controllers
{
    public class MiscellaneousController : RootController
    {
        public ActionResult ORFFinder()
        {
            return View();
        }
        public ActionResult FormatSeq()
        {
            return View();
        }
        public ActionResult ReVerseSeq()
        {
            return View();
        }
    }
}