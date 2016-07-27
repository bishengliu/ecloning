using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using ecloning.Models;

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
            ViewBag.types = new SelectList(db.exp_type.OrderBy(g => g.name), "id", "name");
            return View();
        }
    }
}