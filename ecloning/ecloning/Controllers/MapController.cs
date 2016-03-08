using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ecloning.Models;
using Newtonsoft.Json;

namespace ecloning.Controllers
{
    public class MapController : Controller
    {
        private ecloningEntities db = new ecloningEntities();

        // GET: Map
        public ActionResult Index(int? id)
        {
            //id is plasmid table id and plasmid id
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            plasmid plasmid = db.plasmids.Find(id);
            if(plasmid == null)
            {
                return HttpNotFound();
            }
            ViewBag.Name = plasmid.name;
            ViewBag.Id = id;
            ViewBag.Sequence = plasmid.sequence;


            


            //display all features of the current plasmid
            var plasmid_map = db.plasmid_map.Include(p => p.plasmid).Include(p => p.plasmid_feature).Where(p=>p.plasmid_id==id);


            //pass json

            var features = plasmid_map.Select(f => new { show_feature = f.show_feature, end = f.end, feature = f.label, type_id = f.feature_id, start = f.start, cut =f.cut, clockwise = f.clockwise==1? true: false });
            ViewBag.Features = JsonConvert.SerializeObject(features.ToList());



            return View(plasmid_map.ToList());
        }


        // GET: Map/Create
        public ActionResult Create()
        {
            ViewBag.plasmid_id = new SelectList(db.plasmids, "id", "name");
            ViewBag.feature_id = new SelectList(db.plasmid_feature, "id", "feature");
            return View();
        }

        // POST: Map/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,plasmid_id,show_feature,feature,feature_id,start,end,cut,label,clockwise,des")] plasmid_map plasmid_map)
        {
            if (ModelState.IsValid)
            {
                db.plasmid_map.Add(plasmid_map);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.plasmid_id = new SelectList(db.plasmids, "id", "name", plasmid_map.plasmid_id);
            ViewBag.feature_id = new SelectList(db.plasmid_feature, "id", "feature", plasmid_map.feature_id);
            return View(plasmid_map);
        }

        // GET: Map/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            plasmid_map plasmid_map = db.plasmid_map.Find(id);
            if (plasmid_map == null)
            {
                return HttpNotFound();
            }
            ViewBag.plasmid_id = new SelectList(db.plasmids, "id", "name", plasmid_map.plasmid_id);
            ViewBag.feature_id = new SelectList(db.plasmid_feature, "id", "feature", plasmid_map.feature_id);
            return View(plasmid_map);
        }

        // POST: Map/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,plasmid_id,show_feature,feature,feature_id,start,end,cut,label,clockwise,des")] plasmid_map plasmid_map)
        {
            if (ModelState.IsValid)
            {
                db.Entry(plasmid_map).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.plasmid_id = new SelectList(db.plasmids, "id", "name", plasmid_map.plasmid_id);
            ViewBag.feature_id = new SelectList(db.plasmid_feature, "id", "feature", plasmid_map.feature_id);
            return View(plasmid_map);
        }

        // GET: Map/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            plasmid_map plasmid_map = db.plasmid_map.Find(id);
            if (plasmid_map == null)
            {
                return HttpNotFound();
            }
            return View(plasmid_map);
        }

        // POST: Map/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            plasmid_map plasmid_map = db.plasmid_map.Find(id);
            db.plasmid_map.Remove(plasmid_map);
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
