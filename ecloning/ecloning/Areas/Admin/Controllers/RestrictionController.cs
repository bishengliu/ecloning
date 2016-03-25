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
    public class RestrictionController : Controller
    {
        private ecloningEntities db = new ecloningEntities();

        // GET: Admin/Restriction
        public ActionResult Index()
        {
            return View(db.restri_enzyme.ToList());
        }

        // GET: Admin/Restriction/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            restri_enzyme restri_enzyme = db.restri_enzyme.Find(id);
            if (restri_enzyme == null)
            {
                return HttpNotFound();
            }
            return View(restri_enzyme);
        }

        // GET: Admin/Restriction/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Restriction/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,forward_seq,forward_cut,reverse_cut,staractitivity,inactivation,dam,dcm,cpg,methylation")] restri_enzyme restri_enzyme)
        {
            if (ModelState.IsValid)
            {
                db.restri_enzyme.Add(restri_enzyme);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(restri_enzyme);
        }

        // GET: Admin/Restriction/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            restri_enzyme restri_enzyme = db.restri_enzyme.Find(id);
            if (restri_enzyme == null)
            {
                return HttpNotFound();
            }
            return View(restri_enzyme);
        }

        // POST: Admin/Restriction/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,forward_seq,forward_cut,reverse_cut,staractitivity,inactivation,dam,dcm,cpg,methylation")] restri_enzyme restri_enzyme)
        {
            if (ModelState.IsValid)
            {
                db.Entry(restri_enzyme).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(restri_enzyme);
        }

        // GET: Admin/Restriction/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            restri_enzyme restri_enzyme = db.restri_enzyme.Find(id);
            if (restri_enzyme == null)
            {
                return HttpNotFound();
            }
            return View(restri_enzyme);
        }

        // POST: Admin/Restriction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            restri_enzyme restri_enzyme = db.restri_enzyme.Find(id);
            db.restri_enzyme.Remove(restri_enzyme);
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
