using AutoMapper;
using Corno.Globals;
using Corno.Raychem.CustomerPortal.Areas.Admin.Models;
using Corno.Raychem.CustomerPortal.Controllers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Corno.Services.Bootstrapper;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;
using Corno.Services.Email;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services;

namespace Corno.Raychem.CustomerPortal.Areas.Admin.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        public AccountController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
            var dataProtectionProvider = Startup.DataProtectionProvider;
            UserManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("EmailConfirmation"));

           // var provider = new DpapiDataProtectionProvider(Assembly.GetExecutingAssembly().GetName().Name);
          //  UserManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(
           //                                     provider.Create("EmailConfirmation"));
            UserManager.EmailService = new EmailService();
        }
        public AccountController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;

            var dataProtectionProvider = Startup.DataProtectionProvider;
            UserManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("EmailConfirmation"));

            //var provider = new DpapiDataProtectionProvider(Assembly.GetExecutingAssembly().GetName().Name);
            //UserManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(provider.Create("EmailConfirmation"));
            UserManager.EmailService = new EmailService();
        }

        #region -- Data Members --
        public UserManager<ApplicationUser> UserManager { get; private set; }
        #endregion

        #region -- Methods --
        private async Task<string> SendEmailConfirmationTokenAsync(string userId, string subject)
        {
            var code = await UserManager.GenerateEmailConfirmationTokenAsync(userId);

            if (Request.Url == null) return null;

            var callbackUrl = Url.Action("ConfirmEmail", "Account",
                new { area = "Admin", controller = "Account", userId, code },  Request.Url.Scheme);
            await UserManager.SendEmailAsync(userId, subject,
                "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

            return callbackUrl;
        }
        #endregion

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await UserManager.FindAsync(model.UserName, model.Password);
                    if (user == null)
                        throw new Exception("Username/Password is invalid.");
                    if(user.LockoutEnabled)
                        throw new Exception("User is locked out");

                    // Require the user to have a confirmed email before they can log on.
                    if (!await UserManager.IsEmailConfirmedAsync(user.Id))
                    {
                        //await SendEmailConfirmationTokenAsync(user.Id, "Confirm your account");
                        throw new Exception("Check your email and confirm your account, you must be confirmed before you can log in");
                        //ViewBag.errorMessage = "You must have a confirmed email to log on.";
                        //return View("Error");
                    }

                    var db = new ApplicationDbContext();
                    var tmpUser = db.Users.First(u => u.UserName == model.UserName);
                    if (null != tmpUser)
                    {
                        //if (null == tmpUser.CompanyID || tmpUser.CompanyID <= 0)
                        //    throw new Exception("Branch is not assigned for this user");        

                        GlobalVariables.UserId = tmpUser.Id;
                        //GlobalVariables.CompanyID = tmpUser.CompanyID;
                    }

                    await SignInAsync(user, model.RememberMe);
                    return RedirectToLocal(returnUrl);
                }
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: /Account/ConfirmEmail 
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
                return View("Error");

            var user = UserManager.FindById(userId);
            if (user == null)
                return RedirectToAction("Confirm", "Account", new { Email = "" });

            var confirmed = await UserManager.IsEmailConfirmedAsync(userId);
            if (confirmed)
                return RedirectToAction("Index", "Home", new { ConfirmedEmail = user.Email });

            var result = await UserManager.ConfirmEmailAsync(userId, code);
            if (!result.Succeeded)
                return View(result.Succeeded ? "ConfirmEmail" : "Error");

            return View();
            //return RedirectToAction("Confirm", "Account", new { area = "Admin", controller = "Account", user.Email });
        }
        //public async Task<ActionResult> ConfirmEmail(string token, string email)
        //{
        //    var user = UserManager.FindById(token);
        //    if (user == null)
        //        return RedirectToAction("Confirm", "Account", new {Email = ""});

        //    if (user.Email != email)
        //        return RedirectToAction("Confirm", "Account", new {user.Email});

        //    user.EmailConfirmed = true;
        //    await UserManager.UpdateAsync(user);
        //    await SignInAsync(user, isPersistent: false);
        //    return RedirectToAction("Index", "Home", new { ConfirmedEmail = user.Email });
        //}

        [AllowAnonymous]
        public ActionResult Confirm(string email)
        {
            ViewBag.Email = email;
            return View();
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            var viewModel = new RegisterViewModel();
            return View(viewModel);
        }

        //
        // GET: /Account/RegisterDirect
        [AllowAnonymous]
        public ActionResult RegisterDirect()
        {
            var viewModel = new RegisterViewModel
            {
                FirstName = "Moblie",
                LastName = "Moblie"
            };
            return View(viewModel);
        }

        //POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = new ApplicationUser()
                    {
                        UserName = model.UserName,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        PhoneNumber = model.PhoneNumber,
                        Email = model.Email,
                        AccountNo = model.AccountNo
                    };
                    var identityManager = (IdentityManager) Bootstrapper.GetService(typeof(IdentityManager));
                    var result = identityManager.CreateUser(user, model.Password);
                    if (result)
                    {
                        //await SignInAsync(user, isPersistent: false);

                        var token = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        if (Request.Url == null) return View("Info");

                        var callbackUrl = Url.Action("ConfirmEmail", "Account",
                            new { userId = user.Id, token }, protocol: Request.Url.Scheme);
                        await UserManager.SendEmailAsync(user.Id,
                            "Confirm your account", "Please confirm your account by clicking <a href=\""
                                                    + callbackUrl + "\">here</a>");

                        return View("Info");
                        //return RedirectToAction("Index", "Home");
                    }
                    AddErrors(new IdentityResult("Unable to create user"));
                }
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
            }

            return View(model);
        }

        //POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterDirect(RegisterViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = new ApplicationUser
                    {
                        UserName = model.UserName,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        PhoneNumber = model.PhoneNumber,
                        Email = model.Email,
                        AccountNo = model.AccountNo
                    };
                    var identityManager = (IdentityManager) Bootstrapper.GetService(typeof(IdentityManager));
                    var result = identityManager.CreateUser(user, model.Password);
                    if (result)
                    {
                        await SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Index", "Home");
                    }
                    AddErrors(new IdentityResult("Unable to create user"));
                }
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
            }

            // If we got this far, something failed, redisplay form
            //IMasterService masterService = (IMasterService)Bootstrapper.GetService(typeof(MasterService));
            //model.Companies = masterService.CompanyRepository.Get().ToList();
            return View(model);
        }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            ManageMessageId? message = result.Succeeded ? ManageMessageId.RemoveLoginSuccess : ManageMessageId.Error;
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            try
            {
                var hasPassword = HasPassword();
                ViewBag.HasLocalPassword = hasPassword;
                ViewBag.ReturnUrl = Url.Action("Manage");
                if (hasPassword)
                {
                    if (!ModelState.IsValid) return View(model);
                    var result =
                        await
                            UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
                                model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    AddErrors(result);
                }
                else
                {
                    // User does not have a password so remove any validation errors caused by a missing OldPassword field
                    var state = ModelState["OldPassword"];
                    state?.Errors.Clear();

                    if (!ModelState.IsValid) return View(model);

                    var result =
                        await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    AddErrors(result);
                }
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
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
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            try
            {
                var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (loginInfo == null)
                    return RedirectToAction("Login");

                // Sign in the user with this external login provider if the user already has a login
                var user = await UserManager.FindAsync(loginInfo.Login);
                if (user != null)
                {
                    await SignInAsync(user, isPersistent: false);
                    return RedirectToLocal(returnUrl);
                }
                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;

                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = loginInfo.DefaultUserName });
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
            }

            return RedirectToAction("Login");
        }

        //
        // POST: /Account/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        //
        // GET: /Account/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("Manage") : RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Manage");
                }

                if (ModelState.IsValid)
                {
                    // Get the information about the user from the external login provider
                    var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                    if (info == null)
                    {
                        return View("ExternalLoginFailure");
                    }
                    var user = new ApplicationUser() { UserName = model.UserName };
                    var result = await UserManager.CreateAsync(user);
                    if (result.Succeeded)
                    {
                        result = await UserManager.AddLoginAsync(user.Id, info.Login);
                        if (result.Succeeded)
                        {
                            await SignInAsync(user, isPersistent: false);

                            return RedirectToLocal(returnUrl);
                        }
                    }
                    AddErrors(result);
                }

                ViewBag.ReturnUrl = returnUrl;
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
            }

            return View(model);
        }

        //
        // POST: /Account/LogOff
        //[HttpPost, ActionName("LogOff")]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Login", "Account");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var db = new ApplicationDbContext();
            var users = db.Users;
            var model = new List<UserViewModel>();
            foreach (var user in users)
            {
                var u = new UserViewModel(user);
                u.LockoutEnabled = user.LockoutEnabled;
                model.Add(u);
            }
            model = model.OrderBy(x => x.CreatedOn).ToList();
            return View(model);
        }


        [Authorize(Roles = "Admin")]
        public ActionResult Edit(EditUserViewModel model, string id, ManageMessageId? message = null)
        {
            try
            {
                if (model == null) throw new ArgumentNullException(nameof(model));

                var db = new ApplicationDbContext();
                var user = db.Users.First(u => u.Id == id);
                model = new EditUserViewModel(user);
                model = Mapper.Map<EditUserViewModel>(model);

                //IMasterService masterService = (IMasterService)Bootstrapper.GetService(typeof(MasterService));
                //if (null != masterService)
                //    model.Companies = masterService.CompanyRepository.Get().ToList();
                ViewBag.MessageId = message;
                //ViewBag.IsLocked = user.LockoutEnabled;
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditUserViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var db = new ApplicationDbContext();
                    var user = db.Users.First(u => u.UserName == model.UserName);
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.PhoneNumber = model.PhoneNumber;
                    user.Email = model.Email;
                    user.AccountNo = model.AccountNo;
                    db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string id = null)
        {
            try
            {
                var db = new ApplicationDbContext();
                var user = db.Users.First(u => u.Id == id);
                var model = new EditUserViewModel(user);
                if (user == null)
                    return HttpNotFound();
                return View(model);
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
            }

            return View();
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(string id)
        {
            try
            {
                var db = new ApplicationDbContext();
                var user = db.Users.First(u => u.Id == id);
                db.Users.Remove(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
            }

            return View();
        }


        //[Authorize(Roles = "Admin")]
        public ActionResult UserRoles(string id)
        {
            try
            {
                var db = new ApplicationDbContext();
                var user = db.Users.First(u => u.Id == id);
                var model = new SelectUserRolesViewModel(user);
                return View(model);
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
            }

            return View();
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult UserRoles(SelectUserRolesViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var identityManager = (IdentityManager) Bootstrapper.GetService(typeof(IdentityManager));
                    var db = new ApplicationDbContext();
                    var user = db.Users.First(u => u.Id == model.Id);
                    identityManager.ClearUserRoles(user.Id);
                    foreach (var role in model.Roles)
                    {
                        if (role.Selected)
                        {
                            identityManager.AddUserToRole(user.Id, role.RoleName);
                        }
                    }
                    return RedirectToAction("index");
                }
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
            }

            return View(model);
        }


        [Authorize(Roles = "Admin")]
        public ActionResult RoleCreate()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RoleCreate(string roleName)
        {
            Roles.CreateRole(Request.Form["RoleName"]);
            // ViewBag.ResultMessage = "Role created successfully !";

            return RedirectToAction("RoleIndex", "Account");
        }


        [Authorize(Roles = "Admin")]
        public ActionResult RoleIndex()
        {
            var roles = Roles.GetAllRoles();
            return View(roles);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult RoleDelete(string roleName)
        {

            Roles.DeleteRole(roleName);
            // ViewBag.ResultMessage = "Role deleted succesfully !";


            return RedirectToAction("RoleIndex", "Account");
        }

        /// <summary>
        /// Create a new role to the user
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public ActionResult RoleAddToUser()
        {
            var list = new SelectList(Roles.GetAllRoles());
            ViewBag.Roles = list;

            return View();
        }

        /// <summary>
        /// Add role to the user
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RoleAddToUser(string roleName, string userName)
        {
            try
            {
                if (Roles.IsUserInRole(userName, roleName))
                {
                    ViewBag.ResultMessage = "This user already has the role specified !";
                }
                else
                {
                    Roles.AddUserToRole(userName, roleName);
                    ViewBag.ResultMessage = "Username added to the role succesfully !";
                }

                var list = new SelectList(Roles.GetAllRoles());
                ViewBag.Roles = list;
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
            }

            return View();
        }

        /// <summary>
        /// Get all the roles for a particular user
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetRoles(string userName)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(userName))
                {
                    ViewBag.RolesForThisUser = Roles.GetRolesForUser(userName);
                    var list = new SelectList(Roles.GetAllRoles());
                    ViewBag.Roles = list;
                }
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
            }

            return View("RoleAddToUser");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRoleForUser(string userName, string roleName)
        {
            try
            {
                if (Roles.IsUserInRole(userName, roleName))
                {
                    Roles.RemoveUserFromRole(userName, roleName);
                    ViewBag.ResultMessage = "Role removed from this user successfully !";
                }
                else
                {
                    ViewBag.ResultMessage = "This user doesn't belong to selected role.";
                }
                ViewBag.RolesForThisUser = Roles.GetRolesForUser(userName);
                var list = new SelectList(Roles.GetAllRoles());
                ViewBag.Roles = list;
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
            }

            return View("RoleAddToUser");
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());

            return user?.PasswordHash != null;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri, string userId = null)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            private string LoginProvider { get; }
            private string RedirectUri { get; }
            private string UserId { get; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
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
                var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                if (Request.Url != null)
                {
                    var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code }, Request.Url.Scheme);
                    await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                }
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
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
            if (code == null)
                AddErrors(IdentityResult.Failed("Error occured while processing your request."));
            return View();
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
                //return RedirectToAction("ResetPasswordConfirmation", "Account");
                AddErrors(IdentityResult.Failed("Invalid Email Id : " + model.Email));
                return View();
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

        //public ActionResult ResetPassword()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid) return View(model);

        //        var context = new ApplicationDbContext();
        //        var store = new UserStore<ApplicationUser>(context);
        //        var userManager = new UserManager<ApplicationUser>(store);

        //        var userId = User.Identity.GetUserId();//"<YourLogicAssignsRequestedUserId>";
        //        var cUser = await store.FindByIdAsync(userId);
        //        //var passhash = await store.GetPasswordHashAsync(cUser);
        //        //var oldpassword = model.Password.ToString();
        //        //String hashedoldPassword = UserManager.PasswordHasher.HashPassword(oldpassword);
        //        var newPassword = model.NewPassword; //"<PasswordAsTypedByUser>";
        //        var hashedNewPassword = userManager.PasswordHasher.HashPassword(newPassword);
        //        //if (passhash == hashedoldPassword)
        //        //    {
        //        await store.SetPasswordHashAsync(cUser, hashedNewPassword);
        //        await store.UpdateAsync(cUser);
        //        ViewBag.successMessage = "Password reset successfully.";
        //    }
        //    catch (Exception exception)
        //    {
        //        HandleControllerException(exception);
        //    }

        //    return View(model);
        //}

        public ActionResult UserProfile()
        {
            try
            {
                //string a;
                //a = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
                var currentUser = UserManager.FindById(User.Identity.GetUserId());

                var viewModel = new UserProfileViewModel
                {
                    FirstName = currentUser.FirstName,
                    LastName = currentUser.LastName,
                    Email = currentUser.Email
                };
                return View(viewModel);
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
            }

            return View();
        }

        public ActionResult LockUnlockUser(string id = null)
        {
            UserViewModel userModel = null;
            try
            {               
                var db = new ApplicationDbContext();
                var user = db.Users.First(u => u.Id == id);
                if (user == null)
                    throw new Exception("User doesnt exist");
                var identityManager = (IdentityManager)Bootstrapper.GetService(typeof(IdentityManager));
                user.LockoutEnabled = user.LockoutEnabled ? false : true;
                user.Attempt = user.LockoutEnabled ? 4 : 0;
                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                userModel = new UserViewModel(user);
                //identityManager.UpdateUser(user);                
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
            }
            return RedirectToAction("Index",userModel);
        }

        #endregion
    }
}