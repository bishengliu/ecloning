using ecloning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Net;
using System.Data.Entity;

namespace ecloning.Controllers
{
    public class RoleController : RootController
    {
        ApplicationDbContext context = new ApplicationDbContext();
        private ecloningEntities db = new ecloningEntities();

        // GET: Role
        [Authorize]
        public ActionResult Index()
        {
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);
            //get the users in the current group
            //get userId
            var userId = User.Identity.GetUserId();
            var userInfo = new UserInfo(userId);
            var groupInfo = new GroupInfo(userInfo.PersonId);
            var groupId = groupInfo.groupId[0];
            var people = db.group_people.Where(p => p.group_id == groupId).Select(p => p.people_id);

            //list for passing to the view
            List<RoleViewModel> RoleModel = new List<RoleViewModel>();

            foreach (var peopleId in people.ToList())
            {
                var role = new RoleViewModel();
                //get email and name
                var person = db.people.Find(peopleId);
                role.userName = person.first_name + " " + person.last_name;
                role.Email = person.email;

                //get the roles of the current person
                //get the current user id string
                var Id = context.Users.Where(u => u.Email == person.email).FirstOrDefault().Id;
                role.userId = Id;

                List<string> Roles = userManager.GetRoles(Id).ToList();
                role.Roles = Roles;
                RoleModel.Add(role);
            }

            return View(RoleModel.ToList());
        }

        [Authorize]
        [HttpGet]
        public ActionResult AddRole(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //get the user role list
            var Roles = context.Roles.Where(e => e.Name != "appAdmin" && e.Name != "InstAdmin").OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            ViewBag.Roles = Roles;
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AddRole(string userId, string RoleName)
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult DeleteRole(string userId)
        {
            return View();
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRole(string userId, string RoleName)
        {
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