using ecloning.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ecloning.Controllers
{
    public class ProtocolController : RootController
    {
        private ecloningEntities db = new ecloningEntities();
        // GET: Protocol
        [Authorize]
        public ActionResult Index()
        {
            //get userId
            var userId = User.Identity.GetUserId();
            var userInfo = new UserInfo(userId);
            var groupInfo = new GroupInfo(userInfo.PersonId);
            //get all the peopleId in the group
            var groupPeopleIds = groupInfo.groupPeopleId;
            //show all the protocols in the group

            var protocols = db.protocols.Where(p => groupPeopleIds.Contains((int)p.people_id));
            return View(protocols.ToList());
        }

        [Authorize]
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

    }

}