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
using Newtonsoft.Json;

namespace ecloning.Controllers
{
    public class pBundleController : Controller
    {
        private ecloningEntities db = new ecloningEntities();

        // GET: pBundle
        public ActionResult Index()
        {
            var plasmid_bundle = db.plasmid_bundle.Include(p => p.person);
            return View(plasmid_bundle.ToList());
        }

        // GET: pBundle/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            plasmid_bundle plasmid_bundle = db.plasmid_bundle.Find(id);
            if (plasmid_bundle == null)
            {
                return HttpNotFound();
            }
            return View(plasmid_bundle);
        }


        [Authorize]
        [HttpGet]
        public ActionResult Select()
        {
            //get userId
            var userId = User.Identity.GetUserId();
            var userInfo = new UserInfo(userId);
            var groupInfo = new GroupInfo(userInfo.PersonId);

            //get all the people in the group
            var peopleId = db.group_people.Where(p => groupInfo.groupId.Contains(p.group_id)).Select(p => p.people_id).ToList();

            //pass all the plasmids and setLength into json
            var plasmids = db.plasmids.Where(p => peopleId.Contains(p.people_id)).OrderBy(p => p.id).Select(p => new { id = p.id, length = p.seq_length, name = p.name });
            ViewBag.Plasmids = JsonConvert.SerializeObject(plasmids.ToList());
            var plasmidId = plasmids.Select(p => p.id).ToList();
            //pass all features into json
            var features = db.plasmid_map.Include(p => p.plasmid).Where(f => f.feature_id != 4).OrderBy(p => p.plasmid_id).OrderBy(s => s.start).Where(p => plasmidId.Contains(p.plasmid_id)).Select(f => new { pId = f.plasmid.id, pName = f.plasmid.name, pSeqCount = f.plasmid.seq_length, show_feature = f.show_feature, end = f.end, feature = f.common_feature != null ? f.common_feature.label : f.feature, type_id = f.feature_id, start = f.start, cut = f.cut, clockwise = f.clockwise == 1 ? true : false });
            ViewBag.Features = JsonConvert.SerializeObject(features.ToList());
            return View();
        }

        // GET: pBundle/Create
        public ActionResult Create()
        {
            ViewBag.people_id = new SelectList(db.people, "id", "first_name");
            return View();
        }

        // POST: pBundle/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,des,member_type,member_id,member_role,ref_bundle,img_fn,dt,people_id")] plasmid_bundle plasmid_bundle)
        {
            if (ModelState.IsValid)
            {
                db.plasmid_bundle.Add(plasmid_bundle);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.people_id = new SelectList(db.people, "id", "first_name", plasmid_bundle.people_id);
            return View(plasmid_bundle);
        }

        // GET: pBundle/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            plasmid_bundle plasmid_bundle = db.plasmid_bundle.Find(id);
            if (plasmid_bundle == null)
            {
                return HttpNotFound();
            }
            ViewBag.people_id = new SelectList(db.people, "id", "first_name", plasmid_bundle.people_id);
            return View(plasmid_bundle);
        }

        // POST: pBundle/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,des,member_type,member_id,member_role,ref_bundle,img_fn,dt,people_id")] plasmid_bundle plasmid_bundle)
        {
            if (ModelState.IsValid)
            {
                db.Entry(plasmid_bundle).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.people_id = new SelectList(db.people, "id", "first_name", plasmid_bundle.people_id);
            return View(plasmid_bundle);
        }

        // GET: pBundle/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            plasmid_bundle plasmid_bundle = db.plasmid_bundle.Find(id);
            if (plasmid_bundle == null)
            {
                return HttpNotFound();
            }
            return View(plasmid_bundle);
        }

        // POST: pBundle/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            plasmid_bundle plasmid_bundle = db.plasmid_bundle.Find(id);
            db.plasmid_bundle.Remove(plasmid_bundle);
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
