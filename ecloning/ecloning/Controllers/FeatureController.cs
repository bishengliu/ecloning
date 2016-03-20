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

namespace ecloning.Controllers
{
    public class FeatureController : Controller
    {
        private ecloningEntities db = new ecloningEntities();

        // GET: Feature
        [Authorize]
        public ActionResult Index()
        {
            //get userId
            var userId = User.Identity.GetUserId();
            var userInfo = new UserInfo(userId);
            var groupInfo = new GroupInfo(userInfo.PersonId);
            var common_feature = db.common_feature.Include(c => c.plasmid_feature).Where(g=>groupInfo.groupId.Contains(g.group_id));
            ViewBag.Count = common_feature.Count();
            
            return View(common_feature.ToList());
        }


        // GET: Feature/Create
        [Authorize]
        public ActionResult Create()
        {
            //get userId
            var userId = User.Identity.GetUserId();
            var userInfo = new UserInfo(userId);
            var groupInfo = new GroupInfo(userInfo.PersonId);
            ViewBag.feature_id = new SelectList(db.plasmid_feature.Where(f=>f.id < 9), "id", "des"); //remove orf and exac features

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

        // POST: Feature/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,feature_id,label,sequence,group_id,des")] common_feature common_feature)
        {
            //get userId
            var userId = User.Identity.GetUserId();
            var userInfo = new UserInfo(userId);
            var groupInfo = new GroupInfo(userInfo.PersonId);
            ViewBag.feature_id = new SelectList(db.plasmid_feature.Where(f => f.id < 9), "id", "des", common_feature.feature_id);
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


            ////feature sequence must be unieque
            //var checkSeq = db.common_feature.Where(s => s.sequence == common_feature.sequence);
            //if (checkSeq.Count() > 0)
            //{
            //    TempData["msg"] = "Error: feature with the same sequence already exists!";
            //    return View(common_feature);
            //}

            if (ModelState.IsValid)
            {
                db.common_feature.Add(common_feature);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(common_feature);
        }

        // GET: Feature/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            //id is the table id of common features
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            common_feature common_feature = db.common_feature.Find(id);
            if (common_feature == null)
            {
                return HttpNotFound();
            }

            //get userId
            var userId = User.Identity.GetUserId();
            var userInfo = new UserInfo(userId);
            var groupInfo = new GroupInfo(userInfo.PersonId);
            ViewBag.feature_id = new SelectList(db.plasmid_feature.Where(f => f.id  < 9), "id", "des", common_feature.feature_id);

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

            return View(common_feature);
        }

        // POST: Feature/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,feature_id,label,sequence,group_id,des")] common_feature common_feature)
        {
            //get userId
            var userId = User.Identity.GetUserId();
            var userInfo = new UserInfo(userId);
            var groupInfo = new GroupInfo(userInfo.PersonId);
            ViewBag.feature_id = new SelectList(db.plasmid_feature.Where(f => f.id < 9), "id", "des", common_feature.feature_id);

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

            //var checkSeq = db.common_feature.Where(f=>f.id != common_feature.id && f.sequence == common_feature.sequence);
            //if (checkSeq.Count() > 0)
            //{
            //    TempData["msg"] = "Error: feature with the same sequence already exists!";
            //    return View(common_feature);
            //}

            if (ModelState.IsValid)
            {
                db.Entry(common_feature).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(common_feature);
        }

        // GET: Feature/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            common_feature common_feature = db.common_feature.Find(id);
            if (common_feature == null)
            {
                return HttpNotFound();
            }
            return View(common_feature);
        }

        // POST: Feature/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            common_feature common_feature = db.common_feature.Find(id);
            db.common_feature.Remove(common_feature);
            //detele all the reffered features in plasmid_map and plamsid_map backup tables
            var backups = db.plasmid_map_backup.Where(i => i.common_id == id);
            if (backups.Count() > 0)
            {
                foreach (var b in backups)
                {
                    db.plasmid_map_backup.Remove(b);
                }
            }
            //find plasmid_map
            var maps = db.plasmid_map.Where(i => i.common_id == id);
            if (maps.Count() > 0)
            {
                foreach (var m in maps)
                {
                    db.plasmid_map.Remove(m);
                }
            }
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
