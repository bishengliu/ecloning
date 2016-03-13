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
            ViewBag.feature_id = new SelectList(db.plasmid_feature.Where(f=>f.id !=10), "id", "des");

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
            ViewBag.feature_id = new SelectList(db.plasmid_feature.Where(f => f.id != 10), "id", "des", common_feature.feature_id);
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
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            common_feature common_feature = db.common_feature.Find(id);
            if (common_feature == null)
            {
                return HttpNotFound();
            }
            ViewBag.feature_id = new SelectList(db.plasmid_feature, "id", "feature", common_feature.feature_id);
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
            if (ModelState.IsValid)
            {
                db.Entry(common_feature).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.feature_id = new SelectList(db.plasmid_feature, "id", "feature", common_feature.feature_id);
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
