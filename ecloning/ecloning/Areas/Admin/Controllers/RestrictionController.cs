using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ecloning.Models;
using ecloning.Areas.Admin.Models;
using System.Text;

namespace ecloning.Areas.Admin.Controllers
{
    public class RestrictionController : Controller
    {
        private ecloningEntities db = new ecloningEntities();

        // GET: Admin/Restriction
        [Authorize]
        public ActionResult Index()
        {
            return View(db.restri_enzyme.OrderBy(n=>n.name).ToList());
        }

        // GET: Admin/Restriction/Create
        [Authorize]
        public ActionResult Create()
        {
            //prepare dropdown list
            ViewBag.staractitivity = new SelectList(db.dropdownitems.Where(c => c.category == "TF").OrderBy(g => g.id), "value", "text");
            ViewBag.dam = new SelectList(db.dropdownitems.Where(c => c.category == "TF").OrderBy(g => g.id), "value", "text");
            ViewBag.dcm = new SelectList(db.dropdownitems.Where(c => c.category == "TF").OrderBy(g => g.id), "value", "text");
            ViewBag.cpg = new SelectList(db.dropdownitems.Where(c => c.category == "TF").OrderBy(g => g.id), "value", "text");
            ViewBag.inactivation = new SelectList(db.dropdownitems.Where(c => c.category == "StarActivity").OrderBy(g => g.id), "value", "text", 0);
            return View();
        }

        // POST: Admin/Restriction/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,forward_seq,forward_cut,reverse_cut,staractitivity,inactivation,dam,dcm,cpg")] Restriction restriction)
        {
            //prepare dropdown list
            ViewBag.staractitivity = new SelectList(db.dropdownitems.Where(c => c.category == "TF").OrderBy(g => g.id), "value", "text", restriction.staractitivity);
            ViewBag.dam = new SelectList(db.dropdownitems.Where(c => c.category == "TF").OrderBy(g => g.id), "value", "text", restriction.dam);
            ViewBag.dcm = new SelectList(db.dropdownitems.Where(c => c.category == "TF").OrderBy(g => g.id), "value", "text", restriction.dcm);
            ViewBag.cpg = new SelectList(db.dropdownitems.Where(c => c.category == "TF").OrderBy(g => g.id), "value", "text", restriction.cpg);
            ViewBag.inactivation = new SelectList(db.dropdownitems.Where(c => c.category == "StarActivity").OrderBy(g => g.id), "value", "text", restriction.inactivation);

            //non zero validation
            if(restriction.forward_cut == 0 || restriction.reverse_cut == 0)
            {
                TempData["error"] = "Cut postion cann't be zero!";
                return View(restriction);
            }

            if (ModelState.IsValid)
            {
                //add to restri_enzyme
                var restri_enzyme = new restri_enzyme();
                restri_enzyme.name = restriction.name;
                restri_enzyme.forward_seq = restriction.forward_seq;
                restri_enzyme.forward_cut = restriction.forward_cut;
                restri_enzyme.reverse_cut = restriction.reverse_cut;
                restri_enzyme.staractitivity = restriction.staractitivity;
                restri_enzyme.dam = restriction.dam;
                restri_enzyme.dcm = restriction.dcm;
                restri_enzyme.cpg = restriction.cpg;
                restri_enzyme.inactivation = restriction.inactivation;
                db.restri_enzyme.Add(restri_enzyme);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(restriction);
        }

        // GET: Admin/Restriction/Edit/5
        [Authorize]
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
        [Authorize]
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
        [Authorize]
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
        [Authorize]
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
