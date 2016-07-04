using ecloning.Areas.Admin.Models;
using ecloning.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
            
            var users = new UsersViewModal();
            users.Administrators = new List<Administrator>();
            users.Departments = new List<Department>();

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
                var iAdmin = db.people.Where(e => iEmails.Contains(e.email));
                if (iAdmin.Count() > 0)
                {
                    foreach(var ia in iAdmin.ToList())
                    {
                        var admin = new Administrator();
                        admin.Email = ia.email;
                        admin.Name = ia.first_name + " " + ia.last_name;
                        admin.peopleId = ia.id;
                        admin.Active = ia.active;
                        users.Administrators.Add(admin);
                    }
                }
               
            }

            //get the departments
            var departs = db.departments.Where(d => d.name != "AppAdmin" && d.name != "Institute Admin");
            if (departs.Count() >0){
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
                    users.Departments.Add(depart);
                }                
            }
            return View(users);
        }

        [Authorize(Roles = "appAdmin, InstAdmin")]
        [HttpPost]
        public ActionResult EditDepart(int? id, string dpName)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            //find the department
            var dpart = db.departments.Find(id);
            if (dpart == null)
            {
                return RedirectToAction("Index");
            }
            dpart.name = dpName;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "appAdmin, InstAdmin")]
        [HttpPost]
        public ActionResult AddDepart(string dpName)
        {
            var dpart = new department();
            dpart.name = dpName;
            db.departments.Add(dpart);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "appAdmin, InstAdmin")]
        [HttpGet]
        public ActionResult DeleteDepart(int? id)
        {
            //if the department
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            //find the dpart
            var dpart = db.departments.Find(id);
            if (dpart == null)
            {
                return RedirectToAction("Index");
            }
            //check whether there is group of the dpart
            var groups = db.groups.Where(d => d.depart_id == id);
            if (groups.Count() > 0)
            {
                return RedirectToAction("Index");
            }

            db.departments.Remove(dpart);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        [Authorize(Roles = "appAdmin, InstAdmin")]
        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> Invite(string email)
        {

            var group = db.groups.Where(g => g.name == "Institute Admin").FirstOrDefault();

            //send email
            //shared info
            var leaderEmail = User.Identity.GetUserName();
            var leaderName = db.people.Where(e => e.email == email).FirstOrDefault().first_name + " " + db.people.Where(e => e.email == email).FirstOrDefault().last_name;
            var html = "I invite you to register as an administrator on this website: <a href=\"" + eCloningSettings.AppURI + "\">" + eCloningSettings.AppURI + "</a>";
            html = html + "<br/><p>Please copy the following code to register</p>";
            html = html + "<p><strong>" + group.code + "</strong></p>";
            html = html + "<br/><br/><p>Regards, <br/>" + leaderName + "<br/>" + leaderEmail + "</p>";
            if (eCloningSettings.AppHosting == "Cloud")
            {
                //send email using send grid
                var msg = new SendGridMessage();
                msg.From = new MailAddress(leaderEmail, leaderName);
                msg.AddTo(email);
                msg.Subject = leaderName + " invites you to register on our website";

                msg.Html = html;
                var username = eCloningSettings.SendgridLoginName;
                var pswd = eCloningSettings.SendgridPsw;
                var credentials = new NetworkCredential(username, pswd);

                // Create an Web transport for sending email.
                var transportWeb = new Web(credentials);
                // Send the email.
                // You can also use the **DeliverAsync** method, which returns an awaitable task.
                await transportWeb.DeliverAsync(msg);
                TempData["msg"] = "Invitation Sent!";
            }
            if (eCloningSettings.AppHosting == "Local")
            {
                //send email using local smtp
                var message = new MailMessage();
                message.To.Add(email);  // replace with valid value 
                message.From = new MailAddress(leaderEmail);  // replace with valid value
                message.Subject = leaderName + " invites you to register on our website";
                message.Body = html;
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = LocalSMTP.Login,  // replace with valid value
                        Password = LocalSMTP.Password  // replace with valid value
                    };
                    smtp.Credentials = credential;
                    smtp.Host = LocalSMTP.Server;
                    smtp.Port = LocalSMTP.Port;
                    smtp.EnableSsl = LocalSMTP.EnableSsl;
                    await smtp.SendMailAsync(message);
                    TempData["msg"] = "Invitation Sent!";
                }
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "appAdmin, InstAdmin")]
        [HttpPost]
        public ActionResult AddGroup(int? id, string gpName, string gpEmail, string gpDes)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            //find the department
            var dpart = db.departments.Find(id);
            if (dpart == null)
            {
                return RedirectToAction("Index");
            }
            var group = new group();
            group.depart_id = (int)id;
            group.name = gpName;
            group.email = gpEmail;
            group.des = gpDes;
            //genCode
            var gencode = new genCode();
            group.code = gencode.HashStringCode(256);
            db.groups.Add(group);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}