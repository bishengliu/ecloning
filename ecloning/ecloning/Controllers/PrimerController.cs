using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ecloning.Models;
using Microsoft.AspNet.Identity;

namespace ecloning.Controllers
{
    public class PrimerController : RootController
    {
        private ecloningEntities db = new ecloningEntities();

        // GET: Primer
        public ActionResult Index()
        {
            //get userId
            var userId = User.Identity.GetUserId();
            var userInfo = new UserInfo(userId);
            var groupInfo = new GroupInfo(userInfo.PersonId);
            //get the shared primer ids
            var sharedPrimers = db.group_shared.Where(p => p.category == "Primer").Where(g => groupInfo.groupId.Contains(g.group_id));
            List<int> sharedIds = new List<int>();
            if (sharedPrimers.Count() > 0)
            {
                sharedIds = sharedPrimers.Select(r => r.resource_id).ToList();
            }
            //get all the peopleId in the group
            var groupPeopleIds = groupInfo.groupPeopleId;
            ViewBag.PersonId = userInfo.PersonId;
            ViewBag.shareIds = sharedIds;
            var primers = db.primers.Include(p => p.person).Where(p=>groupPeopleIds.Contains((int)p.people_id));
            ViewBag.Count = primers.Count();
            return View(primers.ToList());
        }
       
        // GET: Primer/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Primer/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "name,sequence,company,orderref,location,usage,purity,modification,des")] dPrimer dprimer)
        {
            if (ModelState.IsValid)
            {
                //user people id
                var userId = User.Identity.GetUserId();
                var userInfo = new UserInfo(userId);
                var people_id = userInfo.PersonId;

                var primer = new primer();
                primer.name = dprimer.name;
                primer.sequence = dprimer.sequence;
                primer.usage = dprimer.usage;
                primer.purity = dprimer.purity;
                primer.modification = dprimer.modification;
                primer.location = dprimer.location;
                primer.company = dprimer.company;
                primer.orderref = dprimer.orderref;
                primer.company = dprimer.company;
                primer.dt = DateTime.Now;
                primer.people_id = people_id;

                db.primers.Add(primer);
                db.SaveChanges();
                TempData["msg"] = "Primer added!";
                return RedirectToAction("Index");
            }
            return View(dprimer);
        }

        // GET: Primer/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            primer primer = db.primers.Find(id);
            if (primer == null)
            {
                return HttpNotFound();
            }
            //load data to dPrimer model
            var dprimer = new dPrimer();
            dprimer.id = primer.id;
            dprimer.name = primer.name;
            dprimer.sequence = primer.sequence;
            dprimer.location = primer.location;
            dprimer.modification = primer.modification;
            dprimer.orderref = primer.orderref;
            dprimer.purity = primer.purity;
            dprimer.company = primer.company;
            dprimer.des = primer.des;
            dprimer.usage = primer.usage;
            return View(dprimer);
        }

        // POST: Primer/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,sequence,company,orderref,location,usage,purity,modification,des")] dPrimer dprimer)
        {
            if (ModelState.IsValid)
            {
                //find primer
                var primer = db.primers.Find(dprimer.id);
                primer.name = dprimer.name;
                primer.sequence = dprimer.sequence;
                primer.usage = dprimer.usage;
                primer.purity = dprimer.purity;
                primer.modification = dprimer.modification;
                primer.location = dprimer.location;
                primer.company = dprimer.company;
                primer.orderref = dprimer.orderref;
                primer.company = dprimer.company;
                primer.dt = DateTime.Now;

                db.SaveChanges();
                TempData["msg"] = "Primer Updated!";
                return RedirectToAction("Index");
            }
            return View(dprimer);
        }

        [Authorize]
        [HttpGet]
        public ActionResult Share(int? id)
        {
            //check the existence of bundle id
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var primer = db.primers.Find(id);
            if (primer == null)
            {
                return HttpNotFound();
            }

            //get the group info
            //get userId
            var userId = User.Identity.GetUserId();
            var userInfo = new UserInfo(userId);
            var groupInfo = new GroupInfo(userInfo.PersonId);
            //check whether it has already been shared
            var isShared = db.group_shared.Where(r => r.category == "Primer" && r.resource_id == id && r.group_id == groupInfo.groupId.FirstOrDefault());
            if (isShared.Count() > 0)
            {
                return RedirectToAction("Index");
            }

            //share the primer
            var share = new group_shared();
            share.category = "Primer";
            share.group_id = groupInfo.groupId.FirstOrDefault();
            share.resource_id = (int)id;
            share.sratus = "submitted";
            db.group_shared.Add(share);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Primer/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            primer primer = db.primers.Find(id);
            if (primer == null)
            {
                return HttpNotFound();
            }
            return View(primer);
        }

        // POST: Primer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            primer primer = db.primers.Find(id);
            db.primers.Remove(primer);
            //detele all the reffered features in plasmid_map and plamsid_map backup tables
            var userId = User.Identity.GetUserId();
            var userInfo = new UserInfo(userId);
            var groupInfo = new GroupInfo(userInfo.PersonId);
            //get all the plasmid in my group
            var plasmidIds = db.plasmids.Where(p => groupInfo.groupPeopleId.Contains(p.people_id)).Select(i => i.id);
            //delete all the primer feature in my group
            var backups = db.plasmid_map_backup.Where(b=>b.feature_id==3&&b.feature == primer.name &&plasmidIds.Contains(b.plasmid_id));
            if (backups.Count() > 0)
            {
                foreach (var b in backups)
                {
                    db.plasmid_map_backup.Remove(b);
                }
            }
            //find plasmid_map
            //delete all the primer feature in my group
            var maps = db.plasmid_map.Where(b => b.feature_id == 3 && b.feature == primer.name && plasmidIds.Contains(b.plasmid_id));
            if (maps.Count() > 0)
            {
                foreach (var m in maps)
                {
                    db.plasmid_map.Remove(m);
                }
            }

            db.SaveChanges();
            return RedirectToAction("Index");
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
