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
        [Authorize]
        // GET: Map
        public ActionResult Index(int? id, string tag)
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
            ViewBag.SeqLength = plasmid.seq_length;

            //autogenerate
            if (tag == "autogenerate")
            {
                //auto generate features
                var autoFeatures = new PlasmidFeature(plasmid.id, plasmid.sequence);
            }

            //backup
            if (tag == "backup")
            {
                //backup plasmid map
                var Backup = new BackupMap(plasmid.id);
            }

            //restore
            if (tag == "restore")
            {
                //restore plasmid map
                var Restore = new RestoreMap(plasmid.id);
            }

            //display all features of the current plasmid
            var plasmid_map = db.plasmid_map.Include(p => p.plasmid).Include(p => p.plasmid_feature).Where(p=>p.plasmid_id==id);

            //find all the backup features in backup tablwe
            var backup = db.plasmid_map_backup.Include(p => p.plasmid).Include(p => p.plasmid_feature).Where(p => p.plasmid_id == id);
            ViewBag.BackupCount = backup.Count();
            ViewBag.Tag = tag;
            //pass json
            var features = plasmid_map.Select(f => new { show_feature = f.show_feature, end = f.end, feature = f.common_feature.label, type_id = f.feature_id, start = f.start, cut =f.cut, clockwise = f.clockwise==1? true: false });
            ViewBag.Features = JsonConvert.SerializeObject(features.ToList());
            return View(plasmid_map.ToList());
        }


        [Authorize]
        [HttpGet]
        public ActionResult ChooseFeature(int? plasmid_id)
        {
            //id is the plasmid id
            if (plasmid_id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            plasmid plasmid = db.plasmids.Find(plasmid_id);
            if (plasmid == null)
            {
                return HttpNotFound();
            }
            ViewBag.Name = plasmid.name;
            ViewBag.SeqLength = plasmid.seq_length;
            ViewBag.PlasmidId = plasmid_id;
            //find all plasmid
            var common_features = db.common_feature.OrderBy(n => n.label).Select(p => new { id = p.id, label = p.label, feature = p.plasmid_feature.des });
            ViewBag.JsonFeatures = JsonConvert.SerializeObject(common_features.ToList());
            ViewBag.msg = "";
            return View();
        }


        [Authorize]
        [HttpPost]
        public ActionResult ChooseFeature(int? plasmid_id, string feature)
        {
            //id is the plasmid id
            if (plasmid_id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            plasmid plasmid = db.plasmids.Find(plasmid_id);
            if (plasmid == null)
            {
                return HttpNotFound();
            }
            ViewBag.Name = plasmid.name;
            ViewBag.SeqLength = plasmid.seq_length;
            ViewBag.PlasmidId = plasmid_id;
            //find all plasmid
            var common_features = db.common_feature.OrderBy(n => n.label).Select(p => new { id = p.id, label = p.label, feature = p.plasmid_feature.des });
            ViewBag.JsonFeatures = JsonConvert.SerializeObject(common_features.ToList());

            if (string.IsNullOrWhiteSpace(feature))
            {
                ViewBag.msg = "At least one feature is required!";
                return View();
            }
            TempData["feature"] = feature;
            return RedirectToAction("Create", new { plasmid_id = plasmid_id });
        }



        [Authorize]
        // GET: Map/Create
        //for no seq
        public ActionResult Create(int? plasmid_id)
        {
            //id is the plasmid id
            if (plasmid_id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            plasmid plasmid = db.plasmids.Find(plasmid_id);
            if (plasmid == null)
            {
                return HttpNotFound();
            }
            ViewBag.Name = plasmid.name;
            ViewBag.SeqLength = plasmid.seq_length;
            ViewBag.PlasmidId = plasmid_id;

            if(TempData["feature"] == null)
            {
                TempData["error"] = "Something went woring, please try again!";
                return RedirectToAction("ChooseFeature", new { id = plasmid_id });
            }
            //prepare common_id
            List<int> CommonId = new List<int>();
            var featureId = (string)TempData["feature"];
            string[] idArray = featureId.Split(',');
            foreach(var i in idArray)
            {
                CommonId.Add(Int32.Parse(i.ToString()));
            }
            ViewBag.TableLength = CommonId.Count();

            for (int i = 0; i < CommonId.Count(); i++)
            {
                var defaultValue = CommonId[i];
                ViewData["[" + i + "].common_id"] = new SelectList(db.common_feature.Where(f => f.id == defaultValue), "id", "label", defaultValue);
                ViewData["[" + i + "].show_feature"] = new SelectList(db.dropdownitems.Where(c => c.category == "YN01").OrderBy(g => g.text), "value", "text", 1);
                ViewData["[" + i + "].clockwise"] = new SelectList(db.dropdownitems.Where(c => c.category == "YN01").OrderBy(g => g.text), "value", "text", 1);
            }

            //find all plasmid
            var common_features = db.common_feature.OrderBy(n => n.label).Select(p => new { id = p.id, label = p.label, feature = p.plasmid_feature.des });
            ViewBag.JsonFeatures = JsonConvert.SerializeObject(common_features.ToList());

            return View();
        }

        // POST: Map/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int plasmid_id, [Bind(Include = "show_feature,start,end,cut,common_id,clockwise")] IList<FeatureViewModel> features)
        {

            plasmid plasmid = db.plasmids.Find(plasmid_id);
            if (plasmid == null)
            {
                return HttpNotFound();
            }
            ViewBag.Name = plasmid.name;
            ViewBag.SeqLength = plasmid.seq_length;
            ViewBag.PlasmidId = plasmid_id;
            ViewBag.TableLength = features.Count();
            for(int i=0; i< features.Count(); i++)
            {
                var defaultValue = features[i].common_id;
                ViewData["[" + i + "].common_id"] = new SelectList(db.common_feature.Where(f => f.id == defaultValue), "id", "label", defaultValue);
                ViewData["[" + i + "].show_feature"] = new SelectList(db.dropdownitems.Where(c => c.category == "YN01").OrderBy(g => g.text), "value", "text", features[i].show_feature);
                ViewData["[" + i + "].clockwise"] = new SelectList(db.dropdownitems.Where(c => c.category == "YN01").OrderBy(g => g.text), "value", "text", features[i].clockwise);
            }
            //find all plasmid
            var common_features = db.common_feature.OrderBy(n => n.label).Select(p => new { id = p.id, label = p.label, feature = p.plasmid_feature.des });
            ViewBag.JsonFeatures = JsonConvert.SerializeObject(common_features.ToList());

            if (ModelState.IsValid)
            {
                int tag = 0;
                foreach(var item in features)
                {

                    //find the feature_id
                    var featureCommon = db.common_feature.Find(item.common_id);
                    if(featureCommon != null)
                    {
                        var map = new plasmid_map();
                        map.plasmid_id = plasmid_id;
                        map.show_feature = item.show_feature;
                        map.feature_id = featureCommon.feature_id;
                        map.start = item.start;
                        map.end = item.end;
                        map.cut = item.cut;
                        map.common_id = item.common_id;
                        map.clockwise = item.clockwise;
                        map.feature = "N.A.";
                        db.plasmid_map.Add(map);
                        tag += 1; 
                    }
                    else
                    {
                        continue;
                    }
                }
                if(tag > 0)
                {
                    db.SaveChanges();
                }
                return RedirectToAction("Index", new { id = plasmid_id, tag= "personDispaly" });
            }

            return View(features);
        }

        // GET: Map/Edit/5
        public ActionResult Edit(int? plasmid_id)
        {
            //only for plasmid sequence is provided
            //allow edit only the "show feature"

            //id is the plasmid id
            if (plasmid_id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            plasmid plasmid = db.plasmids.Find(plasmid_id);
            if (plasmid == null)
            {
                return HttpNotFound();
            }
            ViewBag.Name = plasmid.name;
            ViewBag.SeqLength = plasmid.seq_length;
            ViewBag.PlasmidId = plasmid_id;

            //get all the feature of the current plasmid
            var plasmid_map = db.plasmid_map.Where(p => p.plasmid_id == plasmid_id).OrderBy(i=>i.id);           
            ViewBag.Count = plasmid_map.Count();
            return View(plasmid_map.ToList());
        }

        // POST: Map/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int[] id, int[] show_feature, int plasmid_id)
        {
            for (int i=0; i<id.Count(); i++)
            {
                var plasmid_map = db.plasmid_map.Find(id[i]);
                plasmid_map.show_feature = show_feature[i];
                db.SaveChanges();
            }
            return RedirectToAction("Index", new { id = plasmid_id });
        }

        [Authorize]
        [HttpGet]
        public ActionResult Change(int? plasmid_id)
        {
            //only for plasmid sequence is NOT provided
            //allow edit everyware except Feature and Label
            //id is plasmid table id and plasmid id
            if (plasmid_id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            plasmid plasmid = db.plasmids.Find(plasmid_id);
            if (plasmid == null)
            {
                return HttpNotFound();
            }
            ViewBag.Name = plasmid.name;
            ViewBag.Id = plasmid_id;
            ViewBag.Sequence = plasmid.sequence;
            ViewBag.SeqLength = plasmid.seq_length;


            //display all features of the current plasmid
            var plasmid_map = db.plasmid_map.Include(p => p.plasmid).Include(p => p.plasmid_feature).Where(p => p.plasmid_id == plasmid_id);
            return View(plasmid_map.ToList());
        }

        [Authorize]
        [HttpPost]
        public ActionResult Change(int? plasmid_id, string target, int pk, int value)
        {
            //only for plasmid sequence is NOT provided
            //allow edit everyware except Feature and Label
            //id is plasmid table id and plasmid id
            if (plasmid_id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            plasmid plasmid = db.plasmids.Find(plasmid_id);
            if (plasmid == null)
            {
                return HttpNotFound();
            }
            //save the changes to plasmid_map table
            //find the feature
            var plasmid_map = db.plasmid_map.Find(pk);
            if(target == "start")
            {
                plasmid_map.start = value;
            }
            else if(target == "end")
            {
                plasmid_map.end = value;
            }
            else if(target == "cut")
            {
                plasmid_map.cut = value;
            }
            else if(target == "clockwise")
            {
                plasmid_map.clockwise = value;
            }
            else
            {
                //show feature
                plasmid_map.show_feature = value;
            }

            db.SaveChanges();
            return RedirectToAction("Change", new { plasmid_id = plasmid_id });
        }


        [Authorize]
        // GET: Map/Delete/5
        public ActionResult Delete(int? plasmid_id)
        {
            //id is the plasmid id
            if (plasmid_id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            plasmid plasmid = db.plasmids.Find(plasmid_id);
            if (plasmid == null)
            {
                return HttpNotFound();
            }
            ViewBag.Name = plasmid.name;
            ViewBag.SeqLength = plasmid.seq_length;
            ViewBag.PlasmidId = plasmid_id;
            //get all the feature of the current plasmid
            var plasmid_map = db.plasmid_map.Where(p => p.plasmid_id == plasmid_id);
            ViewBag.Count = plasmid_map.Count();
            return View(plasmid_map.ToList());
        }
        [Authorize]
        // POST: Map/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int[] id, int plasmid_id)
        {
            if (id.Count() == 0)
            {
                TempData["msg"] = "Please select the features you want to remove!";
                return View();
            }
            foreach(int i in id)
            {
                plasmid_map plasmid_map = db.plasmid_map.Find(i);
                db.plasmid_map.Remove(plasmid_map);
            }
            db.SaveChanges();
            return RedirectToAction("Index", new { plasmid_id = plasmid_id});
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
