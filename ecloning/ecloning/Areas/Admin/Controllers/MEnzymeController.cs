using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ecloning.Models;

namespace ecloning.Areas.Admin.Controllers
{
    public class MEnzymeController : Controller
    {
        private ecloningEntities db = new ecloningEntities();

        // GET: Admin/MEnzyme
        public ActionResult Index()
        {
            return View(db.modifying_enzyme.ToList());
        }

        // GET: Admin/MEnzyme/Create
        public ActionResult Create()
        {
            ViewBag.category = new SelectList(db.dropdownitems.Where(c => c.category == "MEnzyme").OrderBy(g => g.text), "text", "value");
            return View();
        }

        // POST: Admin/MEnzyme/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,category,application")] modifying_enzyme modifying_enzyme)
        {
            ViewBag.category = new SelectList(db.dropdownitems.Where(c => c.category == "MEnzyme").OrderBy(g => g.text), "text", "value", modifying_enzyme.category);
            if (ModelState.IsValid)
            {
                db.modifying_enzyme.Add(modifying_enzyme);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(modifying_enzyme);
        }

        // GET: Admin/MEnzyme/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            modifying_enzyme modifying_enzyme = db.modifying_enzyme.Find(id);
            ViewBag.category = new SelectList(db.dropdownitems.Where(c => c.category == "MEnzyme").OrderBy(g => g.text), "text", "value", modifying_enzyme.category);
            if (modifying_enzyme == null)
            {
                return HttpNotFound();
            }
            return View(modifying_enzyme);
        }

        // POST: Admin/MEnzyme/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,category,application")] modifying_enzyme modifying_enzyme)
        {
            ViewBag.category = new SelectList(db.dropdownitems.Where(c => c.category == "MEnzyme").OrderBy(g => g.text), "text", "value", modifying_enzyme.category);
            if (ModelState.IsValid)
            {
                db.Entry(modifying_enzyme).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(modifying_enzyme);
        }

        // GET: Admin/MEnzyme/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            modifying_enzyme modifying_enzyme = db.modifying_enzyme.Find(id);
            if (modifying_enzyme == null)
            {
                return HttpNotFound();
            }
            return View(modifying_enzyme);
        }

        // POST: Admin/MEnzyme/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            modifying_enzyme modifying_enzyme = db.modifying_enzyme.Find(id);
            db.modifying_enzyme.Remove(modifying_enzyme);
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
