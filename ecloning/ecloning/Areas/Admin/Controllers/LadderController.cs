using ecloning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace ecloning.Areas.Admin.Controllers
{
    public class LadderController : SubRootController
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
            //get json data
            var ladderId = ladder.Select(i => i.id);
            var ladderSize = db.ladder_size.Where(l => ladderId.Contains(l.ladder_id)).OrderBy(l=>l.ladder_id).OrderBy(r=>r.Rf).Select(l => new {
                id = l.ladder_id,
                size = l.size,
                mass = l.mass,
                Rf = l.Rf
            });
            ViewBag.ladderSize = JsonConvert.SerializeObject(ladderSize.ToList());
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
        public ActionResult Create([Bind(Include = "id,ladder_type,name,company_id,orderref,ladderSize")] mladder mladder)
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
                        return RedirectToAction("Details", new { type = mladder.ladder_type });
                    }
                    catch (Exception e)
                    {
                        var msg = e;
                        // If any exception is caught, roll back the entire transaction and end the scope.
                        scope.Dispose();
                        TempData["msg"] = "Something went wrong, ladder not added!";
                        return RedirectToAction("Details", new { type = mladder.ladder_type });

                    }
                }
            }
            return View(mladder);
        }


        [Authorize]
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ladder ladder = db.ladders.Find(id);
            if (ladder == null)
            {
                return HttpNotFound();
            }
            
            //load data into mladder
            var mladder = new mladder();
            mladder.id = ladder.id;
            mladder.company_id = ladder.company_id;
            mladder.ladder_type = ladder.ladder_type;
            mladder.name = ladder.name;
            mladder.orderref = ladder.orderref;

            //find ladder fragment
            var fragments = new List<ladderSize>();
            var ladderSize = db.ladder_size.Where(l => l.ladder_id == id).OrderBy(s=>s.size);
            foreach(var item in ladderSize.ToList())
            {
                var fragment = new ladderSize();
                fragment.size = item.size;
                fragment.mass = item.mass;
                fragment.Rf = item.Rf;
                fragments.Add(fragment);
            }
            mladder.ladderSize = fragments;
            ViewBag.ladder_type = new SelectList(db.dropdownitems.Where(c => c.category == "ladder").OrderBy(n => n.id), "value", "text", mladder.ladder_type);
            ViewBag.company_id = new SelectList(db.companies.OrderBy(n => n.shortName), "id", "shortName", mladder.company_id);
            ViewBag.Count = ladderSize.Count();
            return View(mladder);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Edit([Bind(Include = "id,ladder_type,name,company_id,orderref,ladderSize")] mladder mladder)
        {
            ViewBag.ladder_type = new SelectList(db.dropdownitems.Where(c => c.category == "ladder").OrderBy(n => n.id), "value", "text", mladder.ladder_type);
            ViewBag.company_id = new SelectList(db.companies.OrderBy(n => n.shortName), "id", "shortName", mladder.company_id);
            ViewBag.Count = mladder.ladderSize.Count();
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        var ladder = db.ladders.Find(mladder.id);
                        ladder.ladder_type = mladder.ladder_type;
                        ladder.name = mladder.name;
                        ladder.company_id = mladder.company_id;
                        ladder.orderref = mladder.orderref;
                        //get the min and max fragment
                        ladder.min_bp_kDa = mladder.ladderSize.Select(s => s.size).Min();
                        ladder.max_bp_kda = mladder.ladderSize.Select(s => s.size).Max();

                        //remove all the existing fragments
                        var fragments = db.ladder_size.Where(l => l.ladder_id == mladder.id);
                        if (fragments.Count() > 0)
                        {
                            foreach (var item in fragments.ToList())
                            {
                                db.ladder_size.Remove(item);
                            }
                        }
                        //add new fragments
                        foreach (var item in mladder.ladderSize)
                        {
                            var ladderSize = new ladder_size();
                            ladderSize.ladder_id = mladder.id;
                            ladderSize.size = item.size;
                            ladderSize.mass = item.mass;
                            ladderSize.Rf = item.Rf;
                            db.ladder_size.Add(ladderSize);
                        }
                        db.SaveChanges();
                        scope.Complete();
                        TempData["msg"] = "ladder updated!";
                        return RedirectToAction("Details", new { type = mladder.ladder_type });
                    }
                    catch(Exception e)
                    {
                        var msg = e;
                        // If any exception is caught, roll back the entire transaction and end the scope.
                        scope.Dispose();
                        TempData["msg"] = "Something went wrong, ladder not added!";
                        return View(mladder);
                    }
                }               
            }
            return View(mladder);
        }

        [Authorize]
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ladder ladder = db.ladders.Find(id);
            if (ladder == null)
            {
                return HttpNotFound();
            }
            var ladderSize = db.ladder_size.Where(l =>l.ladder_id== ladder.id).OrderBy(l => l.ladder_id).OrderBy(r => r.Rf).Select(l => new {
                id = l.ladder_id,
                size = l.size,
                mass = l.mass,
                Rf = l.Rf
            });
            ViewBag.ladderSize = JsonConvert.SerializeObject(ladderSize.ToList());
            return View(ladder);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ladder ladder = db.ladders.Find(id);
            db.ladders.Remove(ladder);
            //remove fragements
            var fragments = db.ladder_size.Where(l => l.ladder_id == ladder.id);
            if (fragments.Count() > 0)
            {
                foreach(var item in fragments.ToList())
                {
                    db.ladder_size.Remove(item);
                }
            }
            db.SaveChanges();
            return RedirectToAction("Details", new { type = ladder.ladder_type });
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