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
using Newtonsoft.Json;

namespace ecloning.Areas.Admin.Controllers
{
    public class RestrictionController : RootController
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
            ViewBag.staractitivity = new SelectList(db.dropdownitems.Where(c => c.category == "TF").OrderBy(g => g.id), "value", "text", false);
            ViewBag.dam = new SelectList(db.dropdownitems.Where(c => c.category == "TF").OrderBy(g => g.id), "value", "text", false);
            ViewBag.dcm = new SelectList(db.dropdownitems.Where(c => c.category == "TF").OrderBy(g => g.id), "value", "text", false);
            ViewBag.cpg = new SelectList(db.dropdownitems.Where(c => c.category == "TF").OrderBy(g => g.id), "value", "text", false);
            ViewBag.inactivation = new SelectList(db.dropdownitems.Where(c => c.category == "StarActivity").OrderBy(g => g.id), "value", "text", 0);

            //prepare lette code
            var codes = db.letter_code.OrderBy(c => c.name);
            ViewBag.Codes = codes;
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
            //prepare lette code
            var codes = db.letter_code.OrderBy(c => c.name);
            ViewBag.Codes = codes;

            //check the name
            var enzymes = db.restri_enzyme.Where(n => n.name == restriction.name);
            if (enzymes.Count() > 0)
            {
                TempData["error"] = "Enzyme \""+restriction.name+"\" already exists!";
                return View(restriction);
            }

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


        [Authorize]
        public ActionResult mCreate()
        {
            //prepare dropdown list
            ViewBag.staractitivity = new SelectList(db.dropdownitems.Where(c => c.category == "TF").OrderBy(g => g.id), "value", "text", false);
            ViewBag.dam = new SelectList(db.dropdownitems.Where(c => c.category == "TF").OrderBy(g => g.id), "value", "text", false);
            ViewBag.dcm = new SelectList(db.dropdownitems.Where(c => c.category == "TF").OrderBy(g => g.id), "value", "text", false);
            ViewBag.cpg = new SelectList(db.dropdownitems.Where(c => c.category == "TF").OrderBy(g => g.id), "value", "text", false);
            ViewBag.inactivation = new SelectList(db.dropdownitems.Where(c => c.category == "StarActivity").OrderBy(g => g.id), "value", "text", 0);

            //prepare lette code
            var codes = db.letter_code.OrderBy(c => c.name);
            ViewBag.Codes = codes;
            return View();
        }

        // POST: Admin/Restriction/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult mCreate([Bind(Include = "id,name,forward_seq,forward_cut,reverse_cut,forward_cut2,reverse_cut2,staractitivity,inactivation,dam,dcm,cpg")] Restriction restriction)
        {
            //prepare dropdown list
            ViewBag.staractitivity = new SelectList(db.dropdownitems.Where(c => c.category == "TF").OrderBy(g => g.id), "value", "text", restriction.staractitivity);
            ViewBag.dam = new SelectList(db.dropdownitems.Where(c => c.category == "TF").OrderBy(g => g.id), "value", "text", restriction.dam);
            ViewBag.dcm = new SelectList(db.dropdownitems.Where(c => c.category == "TF").OrderBy(g => g.id), "value", "text", restriction.dcm);
            ViewBag.cpg = new SelectList(db.dropdownitems.Where(c => c.category == "TF").OrderBy(g => g.id), "value", "text", restriction.cpg);
            ViewBag.inactivation = new SelectList(db.dropdownitems.Where(c => c.category == "StarActivity").OrderBy(g => g.id), "value", "text", restriction.inactivation);
            //prepare lette code
            var codes = db.letter_code.OrderBy(c => c.name);
            ViewBag.Codes = codes;

            //check the name
            var enzymes = db.restri_enzyme.Where(n => n.name == restriction.name);
            if (enzymes.Count() > 0)
            {
                TempData["error"] = "Enzyme \"" + restriction.name + "\" already exists!";
                return View(restriction);
            }
            //the most left cut must be smaller than the most right
            if (restriction.forward_cut > restriction.forward_cut2 || restriction.forward_cut > restriction.reverse_cut2)
            {
                TempData["error"] = "The forward cut position of the most left cut must be smaller than the cut positions of the most right cut!";
                return View(restriction);
            }
            if (restriction.reverse_cut > restriction.forward_cut2 || restriction.forward_cut > restriction.reverse_cut2)
            {
                TempData["error"] = "The complementary cut position of the most left cut must be smaller than the cut positions of the most right cut!";
                return View(restriction);
            }
            if (restriction.forward_cut2 == null || restriction.reverse_cut2 == null)
            {
                TempData["error"] = "Cut postions for the most right cut are required!";
                return View(restriction);
            }
            //non zero validation
            if (restriction.forward_cut == 0 || restriction.reverse_cut == 0 || restriction.forward_cut2 == 0 || restriction.reverse_cut2 == 0)
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
                restri_enzyme.forward_cut2 = restriction.forward_cut2;
                restri_enzyme.reverse_cut2 = restriction.reverse_cut2;
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

        [Authorize]
        public ActionResult preDelete()
        {
            //get all enzymes
            var enzymes = db.restri_enzyme.OrderBy(n => n.name).Select(e => new { id = e.id, name = e.name });
            ViewBag.Enzymes = JsonConvert.SerializeObject(enzymes.ToList());
            return View();
        }
        [Authorize]
        [HttpPost]
        public ActionResult preDelete(string enzyme)
        {
            //get all enzymes
            var enzymes = db.restri_enzyme.OrderBy(n => n.name).Select(e => new { id = e.id, name = e.name });
            ViewBag.Enzymes = JsonConvert.SerializeObject(enzymes.ToList());
            if (string.IsNullOrWhiteSpace(enzyme))
            {
                ViewBag.msg = "At least one enzyme is required!";
                return View();
            }

            return RedirectToAction("Delete", new { enzyme = enzyme });
        }

        // GET: Admin/Restriction/Delete/5
        [Authorize]
        public ActionResult Delete(string enzyme)
        {
            if (string.IsNullOrWhiteSpace(enzyme))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //generate enzyme id list
            List<int> enzymeId = new List<int>();
            string[] idArray = enzyme.Split(',');
            foreach (var i in idArray.Distinct())
            {
                enzymeId.Add(Int32.Parse(i.ToString()));
            }
            var restri_enzyme = db.restri_enzyme.Where(i=>enzymeId.Contains(i.id)).ToList();
            ViewBag.EnzymeId = enzymeId;
            return View(restri_enzyme);
        }

        // POST: Admin/Restriction/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int[] id)
        {
            foreach(int i in id)
            {
                restri_enzyme restri_enzyme = db.restri_enzyme.Find(i);
                db.restri_enzyme.Remove(restri_enzyme);
            }

            // need to deal with the remove plasmid map
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
