using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using ecloning.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using SendGrid;
using System.Net.Mail;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ecloning.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        ecloningEntities db = new ecloningEntities();

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
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

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //var context = new ApplicationDbContext();
            ////get email confirmation 
            //var Confirmed = context.Users.Where(c => c.Email == model.Email).Select(c => c.EmailConfirmed).FirstOrDefault();
            //if (Confirmed)
            //{

                //check whether this user is active or not
                var person = db.people.Where(e => e.email == model.Email);
                //check whether this user is active
                if (person.Count() > 0 && person.FirstOrDefault().active==false)
                {
                    ModelState.AddModelError("", "Your account has been disabled!");
                    return View(model);
                }


                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, change to shouldLockout: true
                var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
                switch (result)
                {
                    case SignInStatus.Success:
                        //add user to role "Researcher"
                        var context = new ApplicationDbContext();
                        //get email confirmation 
                        var userStore = new UserStore<ApplicationUser>(context);
                        var userManager = new UserManager<ApplicationUser>(userStore);
                        ApplicationUser user = context.Users.Where(u => u.UserName.Equals(model.Email, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                        var Roles = userManager.GetRoles(user.Id);
                        ecloningEntities db = new ecloningEntities();

                        var appAdmin = new AppAdmin();
                        var instAdmin = new InstituteAdmin(eCloningSettings.AppEnv, eCloningSettings.AppHosting);

                        //auto create role AppAdmin
                        if (model.Email == appAdmin.email)
                        {
                            //create role if not exist
                            var appAdminRole = db.AspNetRoles.Where(r => r.Name == "AppAdmin").Select(r => r.Name).FirstOrDefault();
                            if (appAdminRole == null)
                            {
                                IdentityRole Role = new IdentityRole("AppAdmin");
                                context.Roles.Add(Role);
                                context.SaveChanges();
                            }

                            //add user to AppAdmin if not
                            if (!userManager.IsInRole(user.Id, "AppAdmin"))
                            {
                                userManager.AddToRole(user.Id, "AppAdmin");
                            }
                        }

                        //auto create role InstitueAdmin
                        if (model.Email == instAdmin.iEmail)
                        {
                            //create role if not exist
                            var instAdminRole = db.AspNetRoles.Where(r => r.Name == "InstAdmin").Select(r => r.Name).FirstOrDefault();
                            if (instAdminRole == null)
                            {
                                IdentityRole Role = new IdentityRole("InstAdmin");
                                context.Roles.Add(Role);
                                context.SaveChanges();
                            }

                            //add user to AppAdmin if not
                            if (!userManager.IsInRole(user.Id, "InstAdmin"))
                            {
                                userManager.AddToRole(user.Id, "InstAdmin");
                            }
                        }

                        //auto role researcher
                        if (model.Email != appAdmin.email && model.Email != instAdmin.iEmail)
                        {
                            //the user is researcher
                            //create role if not exist
                            var ResearcherRole = db.AspNetRoles.Where(r => r.Name == "Researcher").Select(r => r.Name).FirstOrDefault();
                            if (ResearcherRole == null)
                            {
                                IdentityRole Role = new IdentityRole("Researcher");
                                context.Roles.Add(Role);
                                context.SaveChanges();
                            }

                            //add user to Researcher if not
                            if (!userManager.IsInRole(user.Id, "Researcher"))
                            {
                                userManager.AddToRole(user.Id, "Researcher");
                            }
                        }

                    return RedirectToLocal(returnUrl);
                    case SignInStatus.LockedOut:
                        return View("Lockout");
                    case SignInStatus.RequiresVerification:
                        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError("", "Invalid login attempt.");
                        return View(model);
                }
            //}
            //else
            //{
            //    ModelState.AddModelError("", "You haven't completed the registration, please follow your email to activate your account.");
            //    return View(model);
            //}
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            //prepare department list
            ViewBag.Department = new SelectList(db.departments.OrderBy(n=>n.name).Select(d=> new { depart = d.name, name = d.name}), "depart", "name");
            //prepare group list
            var groups = db.groups.Include("department").OrderBy(n => n.name).Select(g => new {group = g.name, depart = g.department.name });
            ViewBag.JsonData = JsonConvert.SerializeObject(groups.ToList());
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            //prepare department list
            ViewBag.Department = new SelectList(db.departments.Select(d => new { depart = d.name, name = d.name }), "depart", "name");
            //prepare group list
            var groups = db.groups.Include("department").Select(g => new { group = g.name, depart = g.department.name });
            ViewBag.JsonData = JsonConvert.SerializeObject(groups.ToList());

            if (ModelState.IsValid)
            {
                //appAdmin
                var appAdmin = new AppAdmin();

                //institute admin
                var instAdmin = new InstituteAdmin(eCloningSettings.AppEnv, eCloningSettings.AppHosting);
                //db
                ecloningEntities db = new ecloningEntities();


                //check registration code

                //find depart and group info
                var depart = db.departments.Where(d => d.name == model.Department);
                if (depart.Count() == 0)
                {
                    TempData["msg"] = "Department not found!";
                    return View(model);
                }
                var group = db.groups.Where(g => g.depart_id == depart.FirstOrDefault().id && g.name == model.Group);
                if (group.Count() == 0)
                {
                    TempData["msg"] = "Group not found!";
                    return View(model);
                }

                //check registration code
                if (model.Email == appAdmin.email)
                {
                    if(model.code != appAdmin.code)
                    {
                        TempData["msg"] = "Invitation Code is wrong!";
                        return View(model);
                    }
                    else
                    {
                        //need to deal with appAdmin department and group
                        //need to add first
                        model.Department = "AppAdmin";
                        model.Group = "AppAdmin";
                    }

                }
                else if (model.Email == instAdmin.iEmail)
                {
                    if (model.code != instAdmin.iCode)
                    {
                        TempData["msg"] = "Invitation Code is wrong!";
                        return View(model);
                    }
                    else
                    {
                        //need to deal with institute admin department and group
                        //need to add first
                        model.Department = "Institute Admin";
                        model.Group = "Institute Admin";
                    }

                }
                else
                {
                    //check code in group table
                    //researchers
                    if (group.FirstOrDefault().code != model.code)
                    {
                        TempData["msg"] = "Invitation Code is wrong!";
                        return View(model);
                    }
                }


                //check license based on group
                if(model.Email != appAdmin.email && model.Email != instAdmin.iEmail)
                {
                    //get group and department again 
                    depart = db.departments.Where(d => d.name == model.Department);
                    group = db.groups.Where(g => g.depart_id == depart.FirstOrDefault().id && g.name == model.Group);

                    //check how many license left
                    var license = db.app_license.Where(l => l.depart_id == depart.FirstOrDefault().id && l.group_id == group.FirstOrDefault().id);
                    int licenseNum = -1;
                    bool? licenseExpired = false;
                    if (license.Count() > 0)
                    {
                        licenseNum = license.FirstOrDefault().group_num;
                        licenseExpired = license.FirstOrDefault().group_expired;
                    }
                    if(licenseExpired == true)
                    {
                        TempData["msg"] = "The license for "+model.Group+" has expired!";
                        return View(model);
                    }
                    //check how many people in the same group
                    var peopleInGroup = db.group_people.Where(g => g.group_id == group.FirstOrDefault().id).Select(p => p.people_id).ToList();
                    var activePeople = db.people.Where(p => peopleInGroup.Contains(p.id) && p.active == true).Select(a => a.active);

                    if(licenseNum >0 && activePeople.Count()>0 && activePeople.Count()> licenseNum)
                    {
                        TempData["msg"] = "registration failed. Maximum number of license for " + model.Group + "reached!";
                        return View(model);
                    }
                }
                

                //try to register
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);

                //role related
                ApplicationDbContext context = new ApplicationDbContext();
                var roleStore = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(roleStore);

                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);
 

                //register sucess
                if (result.Succeeded)
                {
                    //check whether person already exist in people table
                    var Person = db.people.Where(e => e.email == model.Email);
                    if (Person.Count() == 0)
                    {
                        //add to people table
                        var person = new person();

                        person.first_name = model.first_name;
                        person.last_name = model.last_name;
                        person.email = model.Email;
                        person.active = true;
                        db.people.Add(person);
                        db.SaveChanges();

                        //get persion id
                        int people_id = db.people.Where(e => e.email == model.Email).FirstOrDefault().id;


                        //add to group_people
                        var group_people = new group_people();
                        group_people.group_id = group.FirstOrDefault().id;
                        group_people.people_id = people_id;
                        db.group_people.Add(group_people);
                        db.SaveChanges();
                    }
                    

                    
                    //stopping auto sigin in
                    //await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                    //check whether it is appAdmin
                    if (model.Email == appAdmin.email)
                    {
                        //check whether appAmin role
                        var Admin = db.AspNetRoles.Where(r => r.Name == "appAdmin");
                        if (Admin.Count() == 0)
                        {
                            //create it
                            IdentityRole Role = new IdentityRole("appAdmin");
                            context.Roles.Add(Role);
                            context.SaveChanges();
                        }

                        //add to appAdmin role
                        if (!userManager.IsInRole(user.Id, "appAdmin"))
                        {
                            userManager.AddToRole(user.Id, "appAdmin");
                        }
                    }


                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    //if app host in azure, use sendgrid
                    //if(eCloningSettings.AppEnv() == "Cloud")
                    //{
                    //    var msg = new SendGridMessage();

                    //    msg.From = new MailAddress(appAdmin.email, appAdmin.appName);
                    //    msg.AddTo(model.Email);
                    //    msg.Subject = "Complete your registration";


                    //    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    //    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    //    //await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                    //    msg.Html = "Thank you for your registration, please click on the link to complete your registration: <a href=\"" + callbackUrl + "\">here</a>";

                    //    var username = eCloningSettings.SendgridLoginName();
                    //    var pswd = eCloningSettings.SendgridPsw();
                    //    var credentials = new NetworkCredential(username, pswd);
                    //    // Create an Web transport for sending email.
                    //    var transportWeb = new Web(credentials);

                    //    // Send the email.
                    //    // You can also use the **DeliverAsync** method, which returns an awaitable task.
                    //    await transportWeb.DeliverAsync(msg);
                    //}
                    //if(eCloningSettings.AppEnv() == "Cloud")
                    //{
                    //    //send email using local smtp
                    //    var smtp = new LocalSMTP();
                    //}

                    //return RedirectToAction("EmailSent", "Account");
                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        [AllowAnonymous]
        public ActionResult EmailSent()
        {
            return View();
        }
        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}