using ecloning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ecloning.Areas.Admin.Controllers
{
    public class LadderController : RootController
    {
        private ecloningEntities db = new ecloningEntities();

        // GET: Admin/Ladder
        [Authorize]
        [HttpGet]
        public ActionResult Index(string type)
        {
            var ladder = db.ladders;
            if (type == "DNA")
            {
                return View(ladder.Where(t => t.ladder_type == "DNA").ToList());
            }
            else if (type == "RNA")
            {
                return View(ladder.Where(t => t.ladder_type == "RNA").ToList());
            }
            else
            {
                return View(ladder.Where(t => t.ladder_type == "Protein").ToList());
            }            
        }

        [Authorize]
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.ladder_type = new SelectList(db.dropdownitems.Where(c=>c.category=="ladder").OrderBy(n => n.id), "value", "text");
            ViewBag.company_id = new SelectList(db.companies.OrderBy(n => n.shortName), "id", "shortName");
            return View();
        }
    }
}