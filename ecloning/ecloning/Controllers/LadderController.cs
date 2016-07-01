using ecloning.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ecloning.Controllers
{
    public class LadderController : Controller
    {
        private ecloningEntities db = new ecloningEntities();
        // GET: Ladder
        public ActionResult Index(string type)
        {
            var ladder = db.ladders.Where(l => l.ladder_type == type);
            //get json data
            var ladderId = ladder.Select(i => i.id);
            var ladderSize = db.ladder_size.Where(l => ladderId.Contains(l.ladder_id)).OrderBy(l => l.ladder_id).OrderBy(r => r.Rf).Select(l => new {
                id = l.ladder_id,
                size = l.size,
                mass = l.mass,
                Rf = l.Rf
            });
            ViewBag.ladderSize = JsonConvert.SerializeObject(ladderSize.ToList());
            ViewBag.Type = type;
            return View(ladder.ToList());
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}