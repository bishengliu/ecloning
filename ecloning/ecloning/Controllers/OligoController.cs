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
    public class OligoController : RootController
    {
        private ecloningEntities db = new ecloningEntities();

        // GET: Oligoe
        public ActionResult Index()
        {
            //get userId
            var userId = User.Identity.GetUserId();
            var userInfo = new UserInfo(userId);
            var groupInfo = new GroupInfo(userInfo.PersonId);
            //get the shared primer ids
            var sharedOligos = db.group_shared.Where(p => p.category == "oligo").Where(g => groupInfo.groupId.Contains(g.group_id));
            List<int> sharedIds = new List<int>();
            if (sharedOligos.Count() > 0)
            {
                sharedIds = sharedOligos.Select(r => r.resource_id).ToList();
            }
            //get all the peopleId in the group
            var groupPeopleIds = groupInfo.groupPeopleId;
            ViewBag.PersonId = userInfo.PersonId;
            ViewBag.shareIds = sharedIds;
            var oligos = db.oligoes.Include(p => p.person).Where(p => groupPeopleIds.Contains((int)p.people_id));
            ViewBag.Count = oligos.Count();

            return View(oligos.ToList());
        }

        // GET: Oligoe/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Oligoe/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,sequence,company,orderref,location,modification,people_id,des,dt")] mOligo mOligo)
        {
            if (ModelState.IsValid)
            {
                //user people id
                var userId = User.Identity.GetUserId();
                var userInfo = new UserInfo(userId);
                var people_id = userInfo.PersonId;

                var oligo = new oligo();
                oligo.name = mOligo.name;
                oligo.sequence = mOligo.sequence;
                oligo.modification = mOligo.modification;
                oligo.location = mOligo.location;
                oligo.company = mOligo.company;
                oligo.orderref = mOligo.orderref;
                oligo.company = mOligo.company;
                oligo.dt = DateTime.Now;
                oligo.people_id = people_id;


                db.oligoes.Add(oligo);
                db.SaveChanges();
                TempData["msg"] = "Oligo added!";
                return RedirectToAction("Index");
            }

            return View(mOligo);
        }

        // GET: Oligoe/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            oligo oligo = db.oligoes.Find(id);
            if (oligo == null)
            {
                return HttpNotFound();
            }
            //load data to dPrimer model
            var mOligo = new mOligo();
            mOligo.id = oligo.id;
            mOligo.name = oligo.name;
            mOligo.sequence = oligo.sequence;
            mOligo.location = oligo.location;
            mOligo.modification = oligo.modification;
            mOligo.orderref = oligo.orderref;
            mOligo.company = oligo.company;
            mOligo.des = oligo.des;
            return View(mOligo);
        }

        // POST: Oligoe/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,sequence,company,orderref,location,modification,people_id,des,dt")] mOligo mOligo)
        {
            if (ModelState.IsValid)
            {
                //find primer
                var oligo = db.primers.Find(mOligo.id);
                oligo.name = mOligo.name;
                oligo.sequence = mOligo.sequence;
                oligo.modification = mOligo.modification;
                oligo.location = mOligo.location;
                oligo.company = mOligo.company;
                oligo.orderref = mOligo.orderref;
                oligo.company = mOligo.company;
                oligo.dt = DateTime.Now;

                db.SaveChanges();
                TempData["msg"] = "Oligo Updated!";

                return RedirectToAction("Index");
            }
            return View(mOligo);
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
            var oligo = db.oligoes.Find(id);
            if (oligo == null)
            {
                return HttpNotFound();
            }

            //get the group info
            //get userId
            var userId = User.Identity.GetUserId();
            var userInfo = new UserInfo(userId);
            var groupInfo = new GroupInfo(userInfo.PersonId);
            //check whether it has already been shared
            var isShared = db.group_shared.Where(r => r.category == "oligo" && r.resource_id == id && r.group_id == groupInfo.groupId.FirstOrDefault());
            if (isShared.Count() > 0)
            {
                return RedirectToAction("Index");
            }

            //share the primer
            var share = new group_shared();
            share.category = "Oligo";
            share.group_id = groupInfo.groupId.FirstOrDefault();
            share.resource_id = (int)id;
            share.sratus = "submitted";
            db.group_shared.Add(share);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public ActionResult unShare(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //find the plasmid
            var oligo = db.group_shared.Where(s => s.category == "oligo" && s.resource_id == id);
            if (oligo.Count() == 0)
            {
                return HttpNotFound();
            }
            db.group_shared.Remove(oligo.FirstOrDefault());
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        // GET: Oligoe/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            oligo oligo = db.oligoes.Find(id);
            if (oligo == null)
            {
                return HttpNotFound();
            }
            return View(oligo);
        }

        // POST: Oligoe/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            oligo oligo = db.oligoes.Find(id);
            db.oligoes.Remove(oligo);
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
