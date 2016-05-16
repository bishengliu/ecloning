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
using Microsoft.AspNet.Identity;
using System.Transactions;

namespace ecloning.Controllers
{
    public class MapController : RootController
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
            ViewBag.Plasmid = plasmid;

            //get userId
            var userId = User.Identity.GetUserId();
            var userInfo = new UserInfo(userId);
            var groupInfo = new GroupInfo(userInfo.PersonId);

            //autogenerate
            if (tag == "autogenerate")
            {
                //auto generate features
                var autoFeatures = new PlasmidFeature(plasmid.id, plasmid.sequence, groupInfo.groupId);
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
            var plasmid_map = db.plasmid_map.OrderBy(s=>s.start).Include(p => p.plasmid).Include(p => p.plasmid_feature).Where(p=>p.plasmid_id==id);

            //find all the backup features in backup tablwe
            var backup = db.plasmid_map_backup.Include(p => p.plasmid).Include(p => p.plasmid_feature).Where(p => p.plasmid_id == id);
            ViewBag.BackupCount = backup.Count();
            ViewBag.Tag = tag;
            //pass json
            var features = plasmid_map.Where(f=>f.feature_id !=4).OrderBy(s => s.start).Select(f => new { show_feature = f.show_feature, end = f.end, feature = f.common_feature != null? f.common_feature.label: f.feature, type_id = f.feature_id, start = f.start, cut =f.cut, clockwise = f.clockwise==1? true: false });
            ViewBag.Features = JsonConvert.SerializeObject(features.ToList());
            return View(plasmid_map.ToList());
        }

        [Authorize]
        [HttpGet]
        public ActionResult Enzyme(int? plasmid_id, string tag)
        {
            if (plasmid_id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            plasmid plasmid = db.plasmids.Find(plasmid_id);
            if (plasmid == null)
            {
                return HttpNotFound();
            }
            ViewBag.Tag = tag;
            ViewBag.PlasmidId = plasmid_id;
            ViewBag.Name = plasmid.name;
            ViewBag.Sequence = plasmid.sequence;
            ViewBag.SeqLength = plasmid.seq_length;
            //pass json
            //display all enzyme cuts of the current plasmid
            var plasmid_map = db.plasmid_map.OrderBy(s => s.start).Include(p => p.plasmid).Include(p => p.plasmid_feature).Where(p => p.plasmid_id == plasmid_id);
            var Enzymes = plasmid_map.Where(f=>f.feature_id == 4).OrderBy(s => s.start).Select(f => new { show_feature = f.show_feature, end = f.end, feature = f.common_feature != null ? f.common_feature.label : f.feature, type_id = f.feature_id, start = f.start, cut = f.cut, clockwise = f.clockwise == 1 ? true : false });
            ViewBag.Enzymes = JsonConvert.SerializeObject(Enzymes.ToList());

            //all the methylation
            var methylation = db.methylations.Where(m => m.plasmid_id == plasmid_id);
            var Methylation = methylation.OrderBy(n => n.name).Select(m => new { name = m.name, cut = m.cut, clockwise = m.clockwise, dam_cm = m.dam_complete, dam_ip = m.dam_impaired, dcm_cm = m.dcm_complete, dcm_ip = m.dcm_impaired});
            ViewBag.Methylation = JsonConvert.SerializeObject(Methylation.ToList());

            //all other features
            //pass json
            var features = plasmid_map.Where(f => f.feature_id != 4).OrderBy(s => s.start).Select(f => new { show_feature = f.show_feature, end = f.end, feature = f.common_feature != null ? f.common_feature.label : f.feature, type_id = f.feature_id, start = f.start, clockwise = f.clockwise == 1 ? true : false });
            ViewBag.Features = JsonConvert.SerializeObject(features.ToList());
            return View();
        }
        [Authorize]
        [HttpGet]
        public ActionResult Sequence(int? plasmid_id, string tag)
        {
            if (plasmid_id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            plasmid plasmid = db.plasmids.Find(plasmid_id);
            if (plasmid == null)
            {
                return HttpNotFound();
            }
            ViewBag.Tag = tag;
            ViewBag.Plasmid = plasmid;

            //get feartures
            //display all features of the current plasmid
            var plasmid_map = db.plasmid_map.OrderBy(s => s.start).Include(p => p.plasmid).Include(p => p.plasmid_feature).Where(p => p.plasmid_id == plasmid_id);
            //pass json for seq viewer
            var seqFeatures = plasmid_map.Where(f=>f.feature_id ==2 || f.feature_id == 5 || f.feature_id == 8).OrderBy(s => s.start).Select(f => new { end = f.end, feature = f.common_feature != null ? f.common_feature.label : f.feature, type_id = f.feature_id, start = f.start -1, cut = f.cut, underscore = false, color = "black", bgcolor= "#EEEFEE", tooltip= f.common_feature != null ? f.common_feature.label : f.feature });
            ViewBag.seqFeatures = JsonConvert.SerializeObject(seqFeatures.ToList());

            //pass json for feature viewers
            var fvFeatures = plasmid_map.OrderBy(f => f.feature_id).OrderBy(s => s.start).Select(f => new { x= f.feature_id==4? f.cut : f.start, y= f.feature_id == 4 ? f.cut : f.end, description  = f.common_feature != null ? f.common_feature.label : f.feature, id= f.common_feature != null ? f.common_feature.label : f.feature, type_id = f.feature_id, color = "black" });
            ViewBag.fvFeatures = JsonConvert.SerializeObject(fvFeatures.ToList());

            return View();
        }


        [Authorize]
        [HttpGet]
        public ActionResult EditSequence()
        {
            ViewBag.Seq = "GAGGCGGGTAGGGCGGGGATTGAGGCGGGTCAAGGCGGGTAAGAGGCGGGGTACCGACTGATTAAAAAAAATAAATACGTCTCCGGCTCCGGCGGAGACGGAGACTCGATAAGGTCTTCATCACTCCTCCGAAAAAACCTCCGGATCCGAAAACGTTTTTCGAGGGCCCTCGAACATATAGGTAAAAGCCTAGACTAGTCGTGCACAACTGTTAATTAGTAGCCGTATCATATAGCCGTATCATATTATGCTGTTCCACTCCTTGATTTGGTACCGGTTCAACTGGTCACGGCAAGGCCACGAGTGGCGCGCGCTGCAGCGGCCTCGCCAGCTCAAGACCTGGCTGGCCGAGCCCAAGAGGGCCCTGAAGCACCTCCTGCTGAAGCGGCCACACCAGGCCCTGCTGCACTGGGACAAGTAGTCGCGCCAGGTCCTGGTCCACCACGGCCTGTTGTGGGACCGGACCCACACCCACGCGCCGGACCTGCTCGACATGCGGCTCACCAGCCTCCAGCACAGGTGCTTGAAGGCCCTGCGGAGGCCCGGCCGGTACTGGCTCTAGCCGCTCGTCGGCACCCCCGCCCTCAAGCGGGACGCGCTGGGCCGGCCGTTGACGCAC";
            return View();
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
        [HttpGet]
        public ActionResult AddFeature(int? plasmid_id, string tag)
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

            ViewBag.PlasmidId = plasmid_id;
            ViewBag.Tag = tag;
            //get userId
            var userId = User.Identity.GetUserId();
            var userInfo = new UserInfo(userId);
            var groupInfo = new GroupInfo(userInfo.PersonId);
            ViewBag.feature_id = new SelectList(db.plasmid_feature.Where(f => f.id != 10), "id", "des");

            //prepare selectList
            List<SelectListItem> listItems = new List<SelectListItem>();
            foreach (var i in groupInfo.groupIdName)
            {
                listItems.Add(new SelectListItem
                {
                    Text = i.Value,
                    Value = i.Key.ToString()
                });
            }
            ViewBag.group_id = listItems;
            //pass json of label in current group
            var labels = db.common_feature.Where(g => groupInfo.groupId.Contains(g.group_id)).OrderBy(n => n.label).Select(f => new { id = f.id, label = f.label, group = f.group_id });
            ViewBag.JsonLabel = JsonConvert.SerializeObject(labels.ToList());
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddFeature(int? plasmid_id, string tag, [Bind(Include = "id,feature_id,label,sequence,group_id,des")] common_feature common_feature)
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

            ViewBag.PlasmidId = plasmid_id;
            ViewBag.Tag = tag;

            //get userId
            var userId = User.Identity.GetUserId();
            var userInfo = new UserInfo(userId);
            var groupInfo = new GroupInfo(userInfo.PersonId);
            ViewBag.feature_id = new SelectList(db.plasmid_feature.Where(f => f.id != 10 || f.id != 4), "id", "des", common_feature.feature_id);



            //prepare selectList
            List<SelectListItem> listItems = new List<SelectListItem>();
            foreach (var i in groupInfo.groupIdName)
            {
                listItems.Add(new SelectListItem
                {
                    Text = i.Value,
                    Value = i.Key.ToString()
                });
            }
            ViewBag.group_id = new SelectList(listItems, "Value", "Text", common_feature.group_id);
            //pass json of label in current group
            var labels = db.common_feature.Where(g => groupInfo.groupId.Contains(g.group_id)).OrderBy(n => n.label).Select(f => new { id = f.id, label = f.label, group = f.group_id });
            ViewBag.JsonLabel = JsonConvert.SerializeObject(labels.ToList());

            if (ModelState.IsValid)
            {
                db.common_feature.Add(common_feature);
                db.SaveChanges();
                if(tag == "noseq")
                {
                    return RedirectToAction("ChooseFeature", "Map", new { plasmid_id = plasmid_id});
                }
                else
                {
                    if (plasmid.sequence != null)
                    {
                        //backup plasmid map
                        var Backup = new BackupMap(plasmid.id);
                        //auto generate features
                        var autoFeatures = new PlasmidFeature(plasmid.id, plasmid.sequence, groupInfo.groupId);
                    }
                    return RedirectToAction("Index", "Map", new { id = plasmid_id, tag = "personDispaly" });
                }
            }

            return View(common_feature);
        }


        [Authorize]
        [HttpGet]
        public ActionResult Lock(int? plasmid_id)
        {
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
            //find all features in plasmid_map
            var plasmid_map = db.plasmid_map.Include(p => p.plasmid).Include(p => p.plasmid_feature).Where(p => p.plasmid_id == plasmid_id);
            if (plasmid_map.Count() > 0)
            {
                foreach(var item in plasmid_map.ToList())
                {
                    item.locked = 1;
                }
                db.SaveChanges();
            }
            return RedirectToAction("Index", "Map", new { id = plasmid_id, tag = "personDispaly" });
        }

        [Authorize]
        [HttpGet]
        public ActionResult unLock(int? plasmid_id)
        {
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
            //find all features in plasmid_map
            var plasmid_map = db.plasmid_map.Include(p => p.plasmid).Include(p => p.plasmid_feature).Where(p => p.plasmid_id == plasmid_id);
            if (plasmid_map.Count() > 0)
            {
                foreach (var item in plasmid_map.ToList())
                {
                    item.locked = 0;
                }
                db.SaveChanges();
            }
            return RedirectToAction("Index", "Map", new { id = plasmid_id, tag = "personDispaly" });
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
            foreach(var i in idArray.Distinct())
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
            var common_features = db.common_feature.OrderBy(n => n.label).Select(p => new { id = p.id, label = p.label, feature = p.feature_id });
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
            var common_features = db.common_feature.OrderBy(n => n.label).Select(p => new { id = p.id, label = p.label, feature = p.feature_id });
            ViewBag.JsonFeatures = JsonConvert.SerializeObject(common_features.ToList());

            if (ModelState.IsValid)
            {
                //find the exisiting plasmid map 
                var currentMap = db.plasmid_map.Where(p=>p.plasmid_id == plasmid_id);
                List<int?> CommonId = new List<int?>();
                if(currentMap.Count() > 0)
                {
                    CommonId = currentMap.Where(c => c.common_id != null).Select(c => c.common_id).ToList();
                }
                int tag = 0;
                foreach(var item in features)
                {
                    //check whether already in the plasmid_map
                    if (CommonId.Contains(item.common_id))
                    {
                        continue;
                    }
                    else
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
                }
                if(tag > 0)
                {
                    db.SaveChanges();
                }
                return RedirectToAction("Index", new { id = plasmid_id, tag= "personDispaly" });
            }
            var errors = ModelState.Values.SelectMany(v => v.Errors);
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
            var plasmid_map = db.plasmid_map.Where(p => p.plasmid_id == plasmid_id && p.feature_id != 4).OrderBy(c=>c.start);           
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


        //add feature in sequence modal
        [Authorize]
        [HttpGet]
        public ActionResult QuickFeature(int? plasmid_id, int? start, int? end, string tag, int clockwise)
        {
            if (plasmid_id == null|| start== null || end == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //get userId
            var userId = User.Identity.GetUserId();
            var userInfo = new UserInfo(userId);
            var groupInfo = new GroupInfo(userInfo.PersonId);
            ViewBag.feature_id = new SelectList(db.plasmid_feature.Where(f => f.id < 9 && f.id != 4), "id", "des"); //remove orf, enzyme, primer and exac features
            //get the plasmid seq
            //find the plasmid
            var plasmid = db.plasmids.Find(plasmid_id);
            var partialSeq = clockwise==1?plasmid.sequence.Substring((int)start - 1, (int)end - (int)start + 1):FindSeq.cDNA(plasmid.sequence.Substring((int)start - 1, (int)end - (int)start + 1));

            ViewBag.PlasmidId = (int)plasmid_id;
            ViewBag.Start = start;
            ViewBag.End = end;
            ViewBag.Tag = tag;
            ViewBag.Clockwise = clockwise;
            ViewBag.Seq = partialSeq;
            return View();
        }
        [Authorize]
        [HttpPost]
        public ActionResult QuickFeature([Bind(Include = "feature_id,label,sequence,plasmid_id,start,end,tag, clockwise")] FeatureModal feature)
        {
            //get userId
            var userId = User.Identity.GetUserId();
            var userInfo = new UserInfo(userId);
            var groupInfo = new GroupInfo(userInfo.PersonId);

            ViewBag.PlasmidId = (int)feature.plasmid_id;
            ViewBag.Start = feature.start;
            ViewBag.End = feature.end;
            ViewBag.Tag = feature.tag;
            ViewBag.Clockwise = feature.clockwise;
            ViewBag.Seq = feature.sequence;
            if (ModelState.IsValid)
            {
                //start transaction
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        if (feature.feature_id == 3)
                        {
                            //if feature id is 3, add primer to primer table
                            var primer = new primer();
                            primer.name = feature.label;
                            primer.sequence = feature.sequence;
                            primer.people_id = userInfo.PersonId;
                            primer.dt = DateTime.Now;
                            db.primers.Add(primer);
                        }
                        else
                        {
                            //add othe features to common_feature table
                            var cfeature = new common_feature();
                            cfeature.feature_id = feature.feature_id;
                            cfeature.label = feature.label;
                            cfeature.sequence = feature.sequence;
                            cfeature.group_id = groupInfo.groupId.FirstOrDefault();
                            cfeature.people_id = userInfo.PersonId;
                            db.common_feature.Add(cfeature);
                        }
                        //add feature to current plasmid
                        //add feature to plasmid map
                        var map = new plasmid_map();
                        map.plasmid_id = feature.plasmid_id;
                        map.show_feature = 1;
                        map.feature = feature.label;
                        map.feature_id = feature.feature_id;
                        map.start = feature.start;
                        map.end = feature.end;
                        map.clockwise = feature.clockwise;
                        db.plasmid_map.Add(map);

                        //save
                        db.SaveChanges();

                        //auto generate feature fir all plasmids
                        //get the object
                        dynamic fObject = null;
                        if (feature.feature_id == 3)
                        {
                            fObject = db.primers.Where(p => p.name == feature.label && p.people_id == userInfo.PersonId).FirstOrDefault();
                            if (fObject != null)
                            {
                                //find this feature in all plasmids
                               new FindFeature().Find(fObject, groupInfo.groupPeopleId, feature.plasmid_id, "primer");
                            }
                        }
                        else
                        {
                            fObject = db.common_feature.Where(f => f.label == feature.label && f.group_id == groupInfo.groupId.FirstOrDefault());
                            if (fObject != null)
                            {
                                //find this feature in all plasmids
                                new FindFeature().Find(fObject, groupInfo.groupPeopleId, feature.plasmid_id, "cfeature");
                            }
                        }
                        scope.Complete();
                        TempData["msg"] = "Feature added!";
                        return RedirectToAction("Sequence", new { plasmid_id = feature.plasmid_id, tag = feature.tag });
                    }
                    catch (Exception)
                    {
                        scope.Dispose();
                        TempData["msg"] = "Something went wrong, feature was not added, please try again later!";
                        return RedirectToAction("Sequence", new { plasmid_id = feature.plasmid_id, tag = feature.tag });
                    }
                }               
            }
            return View(feature);
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
            var plasmid_map = db.plasmid_map.Where(p => p.plasmid_id == plasmid_id && p.feature_id != 4).OrderBy(c => c.start);
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
            return RedirectToAction("Index", new { id = plasmid_id, tag = "personDispaly" });
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
