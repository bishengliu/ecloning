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
    public class BufferController : RootController
    {
        private ecloningEntities db = new ecloningEntities();

        // GET: Admin/Buffer
        [Authorize]
        public ActionResult Index()
        {
            var buffers = db.buffers.Include(b => b.company);
            return View(buffers.ToList());
        }

        [Authorize]
        public ActionResult ShowActivity(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            buffer buffer = db.buffers.Find(id);
            if (buffer == null)
            {
                return HttpNotFound();
            }
            buffer.show_activity = true;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult HideActivity(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            buffer buffer = db.buffers.Find(id);
            if (buffer == null)
            {
                return HttpNotFound();
            }
            buffer.show_activity = false;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        // GET: Admin/Buffer/Create
        [Authorize]
        public ActionResult Create()
        {            
            ViewBag.company_id = new SelectList(db.companies.OrderBy(n=>n.shortName), "id", "shortName");
            return View();
        }

        // POST: Admin/Buffer/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,des,composition,company_id")] buffer buffer)
        {
            if (ModelState.IsValid)
            {
                db.buffers.Add(buffer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.company_id = new SelectList(db.companies.OrderBy(n => n.shortName), "id", "shortName", buffer.company_id);
            return View(buffer);
        }

        // GET: Admin/Buffer/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            buffer buffer = db.buffers.Find(id);
            if (buffer == null)
            {
                return HttpNotFound();
            }
            ViewBag.company_id = new SelectList(db.companies.OrderBy(n => n.shortName), "id", "shortName", buffer.company_id);
            return View(buffer);
        }

        // POST: Admin/Buffer/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,des,composition,company_id")] buffer buffer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(buffer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.company_id = new SelectList(db.companies.OrderBy(n => n.shortName), "id", "shortName", buffer.company_id);
            return View(buffer);
        }

        // GET: Admin/Buffer/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            buffer buffer = db.buffers.Find(id);
            if (buffer == null)
            {
                return HttpNotFound();
            }
            return View(buffer);
        }

        // POST: Admin/Buffer/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            buffer buffer = db.buffers.Find(id);
            db.buffers.Remove(buffer);
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
