using ecloning.Areas.Admin.Models;
using ecloning.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ecloning.Areas.Admin.Controllers
{
    public class UsersController : Controller
    {
        private ecloningEntities db = new ecloningEntities();
        private ApplicationDbContext context = new ApplicationDbContext();
        // GET: Admin/Users
        [Authorize(Roles = "appAdmin, InstAdmin")]
        public ActionResult Index()
        {
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);
            
            var Users = new UsersViewModal();
            //get the InstAdmin
            //get the user emails that has role of "InstAdmin"
            //get group id
            List<string> iEmails = new List<string>();
            int gId = -1;
            var iGroup = db.groups.Where(g => g.name == "Institute Admin");
            if (iGroup.Count() > 0)
            {
                gId = iGroup.FirstOrDefault().id;
            }
            if(gId != -1)
            {
                //get the users emails in this group
                var aUsers = db.group_people.Where(g => g.group_id == gId).Select(p => p.people_id).ToList();
                if (aUsers.Count() > 0)
                {
                    iEmails = db.people.Where(u => aUsers.Contains(u.id)).Select(e=>e.email).ToList();
                }
            }
            if (iEmails.Count() > 0)
            {
                var admins = new List<Administrator>();
                var iAdmin = db.people.Where(e => iEmails.Contains(e.email));
                if (iAdmin.Count() > 0)
                {
                    foreach(var ia in iAdmin.ToList())
                    {
                        var admin = new Administrator();
                        admin.Email = ia.email;
                        admin.Name = ia.first_name + " " + ia.last_name;
                        admin.peopleId = ia.id;
                        admins.Add(admin);
                    }
                }
                Users.Administrators = admins;
            }

            //get the departments
            var departs = db.departments.Where(d => d.name != "AppAdmin" && d.name != "Institute Admin");
            if (departs.Count() >0){
                var departments = new List<Department>();
                foreach(var dp in departs)
                {
                    var depart = new Department();
                    depart.Id = dp.id;
                    depart.Name = dp.name;
                    depart.Des = dp.des;

                    //get the groups
                    var gps = new List<Group>();
                    var groups = db.groups.Where(g => g.depart_id == dp.id);
                    if (groups.Count() > 0)
                    {
                        foreach(var gp in groups)
                        {
                            var g = new Group();
                            g.groupId = gp.id;
                            g.departId = dp.id;
                            g.Name = gp.name;
                            g.Email = gp.email;
                            g.Des = gp.des;
                            //get the people
                            var people = new List<People>();
                            //get the people id
                            var peopleId = db.group_people.Where(p => p.group_id == gp.id).Select(p => p.people_id).ToList();
                            if (peopleId.Count() > 0)
                            {
                                foreach(var pId in peopleId)
                                {
                                    var pn = new People();
                                    //find the person
                                    var person = db.people.Find(pId);
                                    pn.departId = dp.id;
                                    pn.groupId = gp.id;
                                    pn.peopleId = pId;
                                    pn.Email = person.email;
                                    pn.Name = person.first_name + " " + person.last_name;
                                    pn.Active = person.active;
                                    people.Add(pn);
                                }
                            }
                            //add people
                            g.People = people;
                            gps.Add(g);
                        }
                    }
                    //add groups
                    depart.Groups = gps;
                    departments.Add(depart);
                }
                Users.Departments = departments;
            }
            
            return View(Users);
        }
    }
}