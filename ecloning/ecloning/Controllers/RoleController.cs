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
using System.Security.Cryptography;
using System.Text;
using SendGrid;
using System.Net.Mail;

namespace ecloning.Controllers
{
    public class RoleController : RootController
    {
        ApplicationDbContext context = new ApplicationDbContext();
        private ecloningEntities db = new ecloningEntities();

        // GET: Role
        [Authorize(Roles = "appAdmin, GroupLeader")]
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

                //check whether is the assistant
                role.isAssistant = Array.IndexOf(Roles.ToArray(), "Assistant") >=0 ? true : false;
                role.isActive = person.active;
                role.Roles = Roles;
                RoleModel.Add(role);
            }
            //current userId
            ViewBag.userId = userId;
            return View(RoleModel.ToList());
        }

        [Authorize(Roles = "appAdmin, GroupLeader")]
        [HttpGet]
        public ActionResult AddRole(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //get the email of the current user
            var userInfo = new UserInfo(userId);
            var groupInfo = new GroupInfo(userInfo.PersonId);
            var groupId = groupInfo.groupId[0];
            var people = db.group_people.Where(p => p.group_id == groupId).Select(p => p.people_id);
            var Email = db.people.Where(p => people.Contains(p.id)).Select(e => e.email).ToList();
            ViewBag.Email = new SelectList(db.people.Where(p => people.Contains(p.id)).Select(e=> new { email = e.email, name = e.first_name +" " +e.last_name}), "email", "name");

            //get the user role list
            //var Roles = context.Roles.Where(e => e.Name != "appAdmin" && e.Name != "InstAdmin" && e.Name != "Researcher").OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            //ViewBag.Roles = Roles;
            return View();
        }


        [Authorize(Roles = "appAdmin, GroupLeader")]
        [HttpGet]
        public ActionResult Assistant(string userId, string action)
        {
            try
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);
                if(action == "add")
                {
                    if(!userManager.IsInRole(userId, "Assistant"))
                    {
                        userManager.AddToRole(userId, "Assistant");
                    }
                }
                if (action == "remove")
                {
                    //remove
                    if (userManager.IsInRole(userId, "Assistant"))
                    {
                        userManager.RemoveFromRoles(userId, "Assistant");
                    }
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "appAdmin, GroupLeader")]
        [HttpGet]
        public ActionResult Lock(string email, string action)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    return RedirectToAction("Index");
                }
                //get the person
                var person = db.people.Where(p => p.email == email).FirstOrDefault();
                if (action == "lock")
                {
                    if(person.active == true)
                    {
                        person.active = false;
                        db.SaveChanges();
                    }
                }
                if (action == "unlock")
                {
                    if (person.active != null && person.active == false)
                    {
                        person.active = true;
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "appAdmin, GroupLeader")]
        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> Invite(string email)
        {
            var userId = User.Identity.GetUserId();
            var userInfo = new UserInfo(userId);
            var groupInfo = new GroupInfo(userInfo.PersonId);
            var groupId = groupInfo.groupId[0];

            //generate the invitation code in the group
            byte[] bytes = new byte[256];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            HashAlgorithm algorithm = SHA256.Create();
            var Hash = algorithm.ComputeHash(bytes);

            StringBuilder sb = new StringBuilder();
            foreach (byte b in Hash) { sb.Append(b.ToString("X2")); }
            var code = sb.ToString();
            var group = db.groups.Find(groupId);
            group.code = code;
            db.SaveChanges();
            //send email
            //shared info
            var leaderEmail = User.Identity.GetUserName();
            var leaderName = db.people.Where(e => e.email == email).FirstOrDefault().first_name + " " + db.people.Where(e => e.email == email).FirstOrDefault().last_name;
            var html = "I invite you to register on this website: <a href=\"" + eCloningSettings.AppURI + "\">" + eCloningSettings.AppURI + "</a>";
            html = html + "<br/><p>Please copy the following code to register</p>";
            html = html + "<p><strong>" + code + "</strong></p>";
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