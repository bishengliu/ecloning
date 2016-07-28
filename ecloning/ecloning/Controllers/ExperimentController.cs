using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using ecloning.Models;
using System.Net;
using Newtonsoft.Json;

namespace ecloning.Controllers
{
    public class ExperimentController : Controller
    {
        private ecloningEntities db = new ecloningEntities();
        // GET: Experiment
        [Authorize]
        [HttpGet]
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var userInfo = new UserInfo(userId);
            var groupInfo = new GroupInfo(userInfo.PersonId);
            ViewBag.PersonId = userInfo.PersonId;
            //show shared experiment
            var sharedExps= db.group_shared.Where(p => p.category == "experiment").Where(g => groupInfo.groupId.Contains(g.group_id)).OrderByDescending(r=>r.resource_id);
            List<int> sharedIds = new List<int>();
            if (sharedExps.Count() > 0)
            {
                sharedIds = sharedExps.Select(r => r.resource_id).ToList();
            }
            ViewBag.SharedId = sharedIds;
            //show my experiment
            var myExpIds = new List<int>();
            var myExps = db.experiments.Where(e => e.people_id == userInfo.PersonId).Where(e => !sharedIds.Contains(e.id)).OrderByDescending(e => e.id);
            if (myExps.Count() > 0)
            {
                myExpIds = myExps.Select(e => e.id).ToList();
            }
            //combined id
            List<int> combinedIds = myExpIds.Concat(sharedIds).ToList();            
            //get all exps
            var exps = db.experiments.Where(e => combinedIds.Contains(e.id)).ToList();
            return View(exps);
        }


        [Authorize]
        [HttpGet]
        public ActionResult Create()
        {
            //generate list
            //ViewBag.types = new SelectList(db.exp_type.OrderBy(g => g.name), "id", "name");
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,des")] ExperimentViewModal experiment)
        {
            //get userId
            var userId = User.Identity.GetUserId();
            var userInfo = new UserInfo(userId);
            if (ModelState.IsValid)
            {
                var exp = new experiment();
                exp.name = experiment.name;
                exp.des = experiment.des;
                exp.people_id = userInfo.PersonId;
                exp.dt = DateTime.Now;
                db.experiments.Add(exp);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(experiment);
        }


        [Authorize]
        [HttpGet]
        public ActionResult AddStep(int? id)
        {
            //id is the experiment id
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            experiment exp = db.experiments.Find(id);
            if (exp == null)
            {
                return HttpNotFound();
            }
            ViewBag.expId = id;

            var userId = User.Identity.GetUserId();
            var userInfo = new UserInfo(userId);
            var groupInfo = new GroupInfo(userInfo.PersonId);

            //get all the peopleId in the group
            var groupPeopleIds = groupInfo.groupPeopleId;

            //generate exp type list
            List<ExpType> types = new List<ExpType>();
            var type1 = new ExpType();
            type1.id = 1;
            type1.Name = "Restriction Enzyme Digestion";
            types.Add(type1);
            var type2 = new ExpType();
            type2.id = 2;
            type2.Name = "Plasmid Transformation";
            types.Add(type2);
            var type3 = new ExpType();
            type3.id = 3;
            type3.Name = "Plasmid Miniprep";
            types.Add(type3);
            var type4 = new ExpType();
            type4.id = 4;
            type4.Name = "Fragment Gel Extraction";
            types.Add(type4);
            var type5 = new ExpType();
            type5.id = 5;
            type5.Name = "PCR";
            types.Add(type5);
            var type6 = new ExpType();
            type6.id = 6;
            type6.Name = "Ligation";
            types.Add(type6);
            var type7 = new ExpType();
            type7.id = 7;
            type7.Name = "Pick Colonies";
            types.Add(type7);
            var type8 = new ExpType();
            type8.id = 8;
            type8.Name = "Plasmid Maxiprep";
            types.Add(type8);
            
            var listItems = types.OrderBy(g => g.Name);
            ViewBag.types = new SelectList(listItems, "id", "name");

            //pass plasmid json data
            //get shared plasmids
            var sharedPlasmidId = db.group_shared.Where(c => c.category == "plasmid").Where(g => groupInfo.groupId.Contains(g.group_id)).Select(p => p.resource_id).ToList();
            //my plasmidId
            var myPlasmidId = db.plasmids.Where(p => p.people_id == userInfo.PersonId && !sharedPlasmidId.Contains(p.id)).Select(p => p.id).ToList();
            //get combinedId
            var comPlasmidId = sharedPlasmidId.Concat(myPlasmidId).ToList();
            //pass protocol json data
            var plasmids = db.plasmids.Where(p => comPlasmidId.Contains(p.id)).Select(p => new
            {
                id = p.id,
                name = p.name
            });
            ViewBag.Plasmids = JsonConvert.SerializeObject(plasmids.ToList());
            //pass protocol id
            var protocols = db.protocols.Where(p => groupPeopleIds.Contains((int)p.people_id)).Select(p => new
            {
                id = p.id,
                name = p.name,
                version = p.version
            });
            ViewBag.Protocols = JsonConvert.SerializeObject(protocols.ToList());
            return View();
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