using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ecloning.Models;

namespace ecloning.Controllers
{
    public class PlasmidController : RootController
    {
        private ecloningEntities db = new ecloningEntities();

        // GET: Plasmid
        public ActionResult Index()
        {
            var plasmids = db.plasmids.Include(p => p.person);
            return View(plasmids.ToList());
        }

        // GET: Plasmid/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            plasmid plasmid = db.plasmids.Find(id);
            if (plasmid == null)
            {
                return HttpNotFound();
            }
            return View(plasmid);
        }

        // GET: Plasmid/Create
        public ActionResult Create()
        {
            ViewBag.people_id = new SelectList(db.people, "id", "first_name");
            return View();
        }

        // POST: Plasmid/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,sequence,expression_system,expression_subsystem,promotor,polyA,resistance,reporter,selection,insert,usage,plasmid_type,ref_plasmid,img_fn,addgene,d,people_id,submitted_to_group,shared_with_group,shared_with_people,des")] plasmid plasmid)
        {
            if (ModelState.IsValid)
            {
                db.plasmids.Add(plasmid);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.people_id = new SelectList(db.people, "id", "first_name", plasmid.people_id);
            return View(plasmid);
        }

        // GET: Plasmid/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            plasmid plasmid = db.plasmids.Find(id);
            if (plasmid == null)
            {
                return HttpNotFound();
            }
            ViewBag.people_id = new SelectList(db.people, "id", "first_name", plasmid.people_id);
            return View(plasmid);
        }

        // POST: Plasmid/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,sequence,expression_system,expression_subsystem,promotor,polyA,resistance,reporter,selection,insert,usage,plasmid_type,ref_plasmid,img_fn,addgene,d,people_id,submitted_to_group,shared_with_group,shared_with_people,des")] plasmid plasmid)
        {
            if (ModelState.IsValid)
            {
                db.Entry(plasmid).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.people_id = new SelectList(db.people, "id", "first_name", plasmid.people_id);
            return View(plasmid);
        }

        // GET: Plasmid/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            plasmid plasmid = db.plasmids.Find(id);
            if (plasmid == null)
            {
                return HttpNotFound();
            }
            return View(plasmid);
        }

        // POST: Plasmid/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            plasmid plasmid = db.plasmids.Find(id);
            db.plasmids.Remove(plasmid);
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
