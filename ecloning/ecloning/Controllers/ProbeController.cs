using ecloning.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Net;

namespace ecloning.Controllers
{
    public class ProbeController : RootController
    {
        private ecloningEntities db = new ecloningEntities();
        // GET: Probe
        public ActionResult Index()
        {
            //get userId
            var userId = User.Identity.GetUserId();
            var userInfo = new UserInfo(userId);
            var groupInfo = new GroupInfo(userInfo.PersonId);

            //get the shared primer ids
            var sharedProbes = db.group_shared.Where(p => p.category == "probe").Where(g => groupInfo.groupId.Contains(g.group_id));
            List<int> sharedIds = new List<int>();
            if (sharedProbes.Count() > 0)
            {
                sharedIds = sharedProbes.Select(r => r.resource_id).ToList();
            }
            //get all the peopleId in the group
            var groupPeopleIds = groupInfo.groupPeopleId;
            ViewBag.PersonId = userInfo.PersonId;
            ViewBag.shareIds = sharedIds;
            var probes = db.probes.Where(p => groupPeopleIds.Contains((int)p.people_id));
            ViewBag.Count = probes.Count();
            return View(probes.ToList());
        }

        [Authorize]
        [HttpGet]
        public ActionResult Create()
        {
            //pass json data of all primers
            var primers = db.primers.Select(p => new { id = p.id, name = p.name, seq = p.sequence });
            ViewBag.JsonData = JsonConvert.SerializeObject(primers.ToList());
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,sequence,forward_primer,reverse_primer,usage,location,des")] ProbeViewModel probe)
        {
            //user people id
            var userId = User.Identity.GetUserId();
            var userInfo = new UserInfo(userId);
            var people_id = userInfo.PersonId;

            //pass json data of all primers
            var primers = db.primers.Select(p => new { id = p.id, name = p.name, seq = p.sequence });
            ViewBag.JsonData = JsonConvert.SerializeObject(primers.ToList());
            if (ModelState.IsValid)
            {
                //check name unique
                var oldProbe = db.probes.Where(p => p.name == probe.name);
                if (oldProbe.Count() > 0)
                {
                    TempData["msg"] = "Probe name already exists, please choose another name!";
                    return View(probe);
                }

                //save the data
                var pb = new probe();
                pb.name = probe.name;
                pb.location = probe.location;
                pb.sequence = probe.sequence;
                pb.forward_primer = probe.forward_primer;
                pb.reverse_primer = probe.reverse_primer;
                pb.usage = probe.usage;
                pb.des = probe.des;
                pb.dt = DateTime.Now;
                pb.people_id = people_id;
                db.probes.Add(pb);
                db.SaveChanges();
                return RedirectToAction("Index", "Probe");
            }
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var probe = db.probes.Find(id);
            if (probe == null)
            {
                return HttpNotFound();
            }
            //pass json data of all primers
            var primers = db.primers.Select(p => new { id = p.id, name = p.name, seq = p.sequence });
            ViewBag.JsonData = JsonConvert.SerializeObject(primers.ToList());

            //load data into view model
            var pb = new ProbeViewModel();
            pb.id = probe.id;
            pb.name = probe.name;
            pb.sequence = probe.sequence;
            pb.forward_primer = (int)probe.forward_primer;
            pb.reverse_primer = (int)probe.reverse_primer;
            pb.location = probe.location;
            pb.usage = probe.usage;
            pb.des = probe.des;
            return View(pb);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,sequence,forward_primer,reverse_primer,usage,location,des")] ProbeViewModel probe)
        {
            //user people id
            var userId = User.Identity.GetUserId();
            var userInfo = new UserInfo(userId);
            var people_id = userInfo.PersonId;

            //pass json data of all primers
            var primers = db.primers.Select(p => new { id = p.id, name = p.name, seq = p.sequence });
            ViewBag.JsonData = JsonConvert.SerializeObject(primers.ToList());
            if (ModelState.IsValid)
            {
                //check name unique
                var oldProbe = db.probes.Where(p => p.name == probe.name && p.id != probe.id);
                if (oldProbe.Count() > 0)
                {
                    TempData["msg"] = "Probe name already exists, please choose another name!";
                    return View(probe);
                }

                //save the data
                var pb = db.probes.Find(probe.id);
                pb.name = probe.name;
                pb.location = probe.location;
                pb.sequence = probe.sequence;
                pb.forward_primer = probe.forward_primer;
                pb.reverse_primer = probe.reverse_primer;
                pb.usage = probe.usage;
                pb.des = probe.des;
                pb.dt = DateTime.Now;
                pb.people_id = people_id;
                db.SaveChanges();
                return RedirectToAction("Index", "Probe");
            }
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult Share(int? id)
        {        
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var probe = db.probes.Find(id);
            if (probe == null)
            {
                return HttpNotFound();
            }

            //get the group info
            //get userId
            var userId = User.Identity.GetUserId();
            var userInfo = new UserInfo(userId);
            var groupInfo = new GroupInfo(userInfo.PersonId);
            //check whether it has already been shared
            var isShared = db.group_shared.Where(r => r.category == "probe" && r.resource_id == id && r.group_id == groupInfo.groupId.FirstOrDefault());
            if (isShared.Count() > 0)
            {
                return RedirectToAction("Index");
            }

            //share the primer
            var share = new group_shared();
            share.category = "probe";
            share.group_id = groupInfo.groupId.FirstOrDefault();
            share.resource_id = (int)id;
            share.sratus = "submitted";
            db.group_shared.Add(share);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public ActionResult unShare(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //find the probe
            var probe = db.group_shared.Where(s => s.category == "probe" && s.resource_id == id);
            if (probe.Count() == 0)
            {
                return HttpNotFound();
            }
            db.group_shared.Remove(probe.FirstOrDefault());
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            probe probe = db.probes.Find(id);
            if (probe == null)
            {
                return HttpNotFound();
            }
            return View(probe);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            probe probe = db.probes.Find(id);
            db.probes.Remove(probe);
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