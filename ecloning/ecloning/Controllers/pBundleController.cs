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
using System.IO;

//for transaction
using System.Transactions;


namespace ecloning.Controllers
{
    public class pBundleController : Controller
    {
        private ecloningEntities db = new ecloningEntities();

        // GET: pBundle
        public ActionResult Index()
        {
            //get userId
            var userId = User.Identity.GetUserId();
            var userInfo = new UserInfo(userId);
            var groupInfo = new GroupInfo(userInfo.PersonId);
            //only get my bundle and group bundle
            //get the group shared plasmid bundle id          
            var grouppBundleIds = db.group_shared.Where(g => groupInfo.groupId.Contains(g.group_id)).Where(c => c.category == "pBundle").Select(r => r.resource_id).ToList();


            //only show my bundles that are not shared with any group  
            IQueryable<plasmid_bundle> pBundles = null;
            if (grouppBundleIds.Count() > 0)
            {
                pBundles = db.plasmid_bundle.Where(p => p.people_id == userInfo.PersonId).Where(b => !grouppBundleIds.Contains(b.bundle_id));
            }
            else
            {
                pBundles = db.plasmid_bundle.Where(p => p.people_id == userInfo.PersonId);
            }
            var pBundleIds = pBundles.Select(i => i.bundle_id).ToList();

            //get combined bundle ids
            var combinedIds = grouppBundleIds.Concat(pBundleIds).Distinct().ToList();

            ViewBag.BundleIds = combinedIds;
            //get top level bundles
            var rootBundles = db.plasmid_bundle.Where(i => combinedIds.Contains(i.bundle_id));
            //get the features into json
            //get all the plasmidId
            var plasmidIds = rootBundles.Select(p => p.member_id).Distinct().ToList();
            //pass all features into json
            var features = db.plasmid_map.Include(p => p.plasmid).Where(p=>plasmidIds.Contains(p.plasmid_id)).Where(f => f.feature_id != 4).OrderBy(p => p.plasmid_id).OrderBy(s => s.start).Select(f => new { pId = f.plasmid.id, pName = f.plasmid.name, pSeqCount = f.plasmid.seq_length, show_feature = f.show_feature, end = f.end, feature = f.common_feature != null ? f.common_feature.label : f.feature, type_id = f.feature_id, start = f.start, cut = f.cut, clockwise = f.clockwise == 1 ? true : false });
            ViewBag.Features = JsonConvert.SerializeObject(features.ToList());
            ViewBag.plasmidIds = JsonConvert.SerializeObject(plasmidIds.ToList());
            ViewBag.AllBundleIds = JsonConvert.SerializeObject(combinedIds);
            return View(rootBundles.ToList());
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
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Select(string plasmid)
        {
            if (String.IsNullOrWhiteSpace(plasmid))
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
            
            return RedirectToAction("Create", new { idString = plasmid});
        }
        // GET: pBundle/Create
        public ActionResult Create(string idString)
        {
            if (String.IsNullOrWhiteSpace(idString))
            {
                TempData["msg"] = "Something went wrong, please try again later!";
                return RedirectToAction("Select");
            }

            //process the plasmid ids and parse ids to list<int>
            List<int> pId = new List<int>();
            string[] Ids = idString.Split(',');
            foreach (var id in Ids)
            {
                pId.Add(Int32.Parse(id));
            }

            //get the features for the selected plasmids
            var plasmid_map = db.plasmid_map.OrderBy(s => s.start).Include(p => p.plasmid).Include(p => p.plasmid_feature).Where(p => pId.Contains(p.plasmid_id)).Where(f=>f.feature_id != 4);

            //pass all features into json
            var features = plasmid_map.OrderBy(p => p.plasmid_id).OrderBy(s => s.start).Select(f => new { pId = f.plasmid.id, pName = f.plasmid.name, pSeqCount = f.plasmid.seq_length, show_feature = f.show_feature, end = f.end, feature = f.common_feature != null ? f.common_feature.label : f.feature, type_id = f.feature_id, start = f.start, cut = f.cut, clockwise = f.clockwise == 1 ? true : false });
            ViewBag.Features = JsonConvert.SerializeObject(features.ToList());

            //get userId
            var userId = User.Identity.GetUserId();
            var userInfo = new UserInfo(userId);
            //get all the people in the group
            var groupInfo = new GroupInfo(userInfo.PersonId);
            var peopleIds = db.group_people.Where(g => groupInfo.groupId.Contains(g.group_id)).Select(p=>p.people_id).ToList();
            //pass the bundle name
            var bundles = db.plasmid_bundle.Where(p => peopleIds.Contains(p.people_id)).Select(n=> n.name).Distinct().ToList();
            ViewBag.Bundles = JsonConvert.SerializeObject(bundles);


            ViewBag.IdString = idString;
            ViewBag.Count = pId.Count();
            ViewBag.pId = pId;
            return View();
        }

        // POST: pBundle/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string idString, string Upload, [Bind(Include = "Name,Des,Upload,Plasmids")] pBundle pBundle)
        {
            //prepare some data
            //process the plasmid ids and parse ids to list<int>
            List<int> pId = new List<int>();
            string[] Ids = idString.Split(',');
            foreach (var id in Ids)
            {
                pId.Add(Int32.Parse(id));
            }

            //get the features for the selected plasmids
            var plasmid_map = db.plasmid_map.OrderBy(s => s.start).Include(p => p.plasmid).Include(p => p.plasmid_feature).Where(p => pId.Contains(p.plasmid_id)).Where(f => f.feature_id != 4);

            //pass all features into json
            var features = plasmid_map.OrderBy(p => p.plasmid_id).OrderBy(s => s.start).Select(f => new { pId = f.plasmid.id, pName = f.plasmid.name, pSeqCount = f.plasmid.seq_length, show_feature = f.show_feature, end = f.end, feature = f.common_feature != null ? f.common_feature.label : f.feature, type_id = f.feature_id, start = f.start, cut = f.cut, clockwise = f.clockwise == 1 ? true : false });
            ViewBag.Features = JsonConvert.SerializeObject(features.ToList());

            ViewBag.IdString = idString;
            ViewBag.Count = pId.Count();
            ViewBag.pId = pId;
            //==========================================

            //get userId
            var userId = User.Identity.GetUserId();
            var userInfo = new UserInfo(userId);
            //get all the people in the group
            var groupInfo = new GroupInfo(userInfo.PersonId);
            var peopleIds = db.group_people.Where(g => groupInfo.groupId.Contains(g.group_id)).Select(p => p.people_id).ToList();
            //pass the bundle name
            var allBundles = db.plasmid_bundle.Where(p => peopleIds.Contains(p.people_id)).Select(n => n.name ).Distinct().ToList();
            ViewBag.Bundles = JsonConvert.SerializeObject(allBundles);

            if (String.IsNullOrWhiteSpace(idString))
            {
                TempData["msg"] = "Something went wrong, please try again later!";
                return RedirectToAction("Select");
            }

            if (ModelState.IsValid)
            {
                if (!String.IsNullOrWhiteSpace(Upload))
                {
                    //deal with upload
                    var timeStamp = DateTime.Now.Millisecond.ToString();
                    string fileName = null;

                    //upload  file
                    HttpPostedFileBase file = null;
                    file = Request.Files["file_fn"];

                    if (eCloningSettings.AppHosting == "Cloud")
                    {
                        //upload to azure
                        if (file != null && file.FileName != null && file.ContentLength > 0)
                        {
                            try
                            {
                                fileName = timeStamp + Path.GetFileName(file.FileName);
                                AzureBlob azureBlob = new AzureBlob();
                                azureBlob.directoryName = eCloningSettings.bundleDir;
                                azureBlob.AzureBlobUpload(fileName, file);
                            }
                            catch (Exception)
                            {
                                ModelState.AddModelError("", "File upload failed!");
                                return View(pBundle);
                            }
                        }
                    }
                    else
                    {
                        //upload to local plasmid folder
                        var bundlePath = eCloningSettings.filePath + eCloningSettings.bundleDir;
                        if (file != null && file.FileName != null && file.ContentLength > 0)
                        {
                            try
                            {
                                fileName = timeStamp + Path.GetFileName(file.FileName);
                                var path = Path.Combine(Server.MapPath(bundlePath), fileName);
                                file.SaveAs(path);
                            }
                            catch (Exception)
                            {
                                ModelState.AddModelError("", "File upload failed!");
                                return View(pBundle);
                            }
                        }
                    }
                    Upload = fileName;
                }
                //start transaction
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        //check the max bundle id
                        int maxId = 0;
                        var bundles = db.plasmid_bundle;
                        if (bundles.Count() > 0)
                        {
                            maxId = bundles.OrderByDescending(i => i.bundle_id).FirstOrDefault().bundle_id;
                        }
                        //add pBundle into database
                        foreach (var item in pBundle.Plasmids)
                        {
                            var bundle = new plasmid_bundle();
                            bundle.bundle_id = maxId + 1;
                            bundle.name = pBundle.Name;
                            bundle.des = pBundle.Des;
                            bundle.img_fn = Upload;
                            bundle.dt = DateTime.Now;
                            bundle.people_id = userInfo.PersonId;
                            bundle.member_type = "plasmid";
                            bundle.member_id = item.plasmidId;
                            bundle.member_role = item.plasmidRole;
                            db.plasmid_bundle.Add(bundle);
                        }
                        db.SaveChanges();
                        // If all execution successful
                        scope.Complete();
                    }
                    catch (Exception)
                    {
                        // If any exception is caught, roll back the entire transaction and end the scope.
                        scope.Dispose();
                    }
                    
                }

                return RedirectToAction("Index");
            }


            return View(pBundle);
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
