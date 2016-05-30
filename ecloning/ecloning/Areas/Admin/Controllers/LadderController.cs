using ecloning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
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
        public ActionResult Index()
        {
            var ladder = db.ladders;
            return View(ladder.ToList());                   
        }
        public ActionResult Details(string type)
        {
            var ladder = db.ladders.Where(l=>l.ladder_type==type);
            ViewBag.Type = type;
            return View(ladder.ToList());
        }
        [Authorize]
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.ladder_type = new SelectList(db.dropdownitems.Where(c=>c.category=="ladder").OrderBy(n => n.id), "value", "text");
            ViewBag.company_id = new SelectList(db.companies.OrderBy(n => n.shortName), "id", "shortName");
            ViewBag.Count = 1;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Create([Bind(Include = "ladder_type,name,company_id,orderref,ladderSize")] mladder mladder)
        {
            ViewBag.ladder_type = new SelectList(db.dropdownitems.Where(c => c.category == "ladder").OrderBy(n => n.id), "value", "text");
            ViewBag.company_id = new SelectList(db.companies.OrderBy(n => n.shortName), "id", "shortName");
            ViewBag.Count = mladder.ladderSize.Count();
            if (ModelState.IsValid)
            {
                //start transction
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {                       
                        var ladder = new ladder();
                        ladder.ladder_type = mladder.ladder_type;
                        ladder.name = mladder.name;
                        ladder.company_id = mladder.company_id;
                        ladder.orderref = mladder.orderref;
                        //get the min and max fragment
                        ladder.min_bp_kDa = mladder.ladderSize.Select(s => s.size).Min();
                        ladder.max_bp_kda = mladder.ladderSize.Select(s => s.size).Max();
                        db.ladders.Add(ladder);
                        db.SaveChanges();

                        //find the newly added ladder
                        var nladder = db.ladders.Where(l => l.name == mladder.name && l.company_id == mladder.company_id).FirstOrDefault();

                        //add ladder_size
                        foreach(var item in mladder.ladderSize)
                        {
                            var ladderSize = new ladder_size();
                            ladderSize.ladder_id = nladder.id;
                            ladderSize.size = item.size;
                            ladderSize.mass = item.mass;
                            ladderSize.Rf = item.Rf;
                            db.ladder_size.Add(ladderSize);
                            db.SaveChanges();
                        }
                        scope.Complete();
                        TempData["msg"] = "Ladder added!";
                        return RedirectToAction("Index");
                    }
                    catch (Exception e)
                    {
                        var msg = e;
                        // If any exception is caught, roll back the entire transaction and end the scope.
                        scope.Dispose();
                        TempData["msg"] = "Something went wrong, ladder not added!";
                        return RedirectToAction("Index");

                    }
                }
            }
            return View(mladder);
        }
    }
}