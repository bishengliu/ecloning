using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using ecloning.Models;
using System.Data.Entity;

namespace ecloning.Areas.Admin.Controllers
{
    public class ManageController : SubRootController
    {
        // GET: Admin/Manage
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ecloning.Controllers.ManageController.ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }


        [HttpGet]
        [Authorize]
        public ActionResult ViewAccount()
        {
            var email = @User.Identity.GetUserName();
            //string userId = User.Identity.GetUserId();
            ecloningEntities db = new ecloningEntities();

            var person = db.people.Where(e => e.email == email);
            if (person == null)
            {
                ViewBag.Count = 0;
                TempData["msg"] = "Account information is currently unvailable!";
                return View();
            }
            else
            {
                ViewBag.Count = 1;
                return View(person.FirstOrDefault());
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult EditAccount()
        {
            var email = @User.Identity.GetUserName();
            ecloningEntities db = new ecloningEntities();

            var person = db.people.Where(e => e.email == email);
            if (person == null)
            {
                ViewBag.Count = 0;
                TempData["msg"] = "Account information is currently unvailable!";
                return View();
            }
            else
            {
                ViewBag.Count = 1;
                return View(person.FirstOrDefault());
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult EditAccount([Bind(Include = "id,first_name,last_name")] person person)
        {
            ecloningEntities db = new ecloningEntities();
            ViewBag.Count = 1;

            if (ModelState.IsValid)
            {
                var email = @User.Identity.GetUserName();
                var Person = db.people.Where(e => e.email == email);
                Person.FirstOrDefault().first_name = person.first_name;
                Person.FirstOrDefault().last_name = person.last_name;
                db.SaveChanges();
                return RedirectToAction("ViewAccount");
            }
            return View(person);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }
    }
}