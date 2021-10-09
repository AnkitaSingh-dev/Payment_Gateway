using System;
using Corno.Globals.Constants;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using Corno.Services.Email;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;

namespace Corno.Raychem.CustomerPortal.Areas.Admin.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public int? AccountNo { get; set; }

        public double? Wallet { get; set; }
        public string UserType { get; set; }
        public string Otp { get; set; }
        public string DeviceId { get; set; }
        public string AgencyName { get; set; }
        public bool IsKYCSubmit { get; set; }
        public bool IsPanSubmit { get; set; }
        public bool IsAdhaarSubmit { get; set; }
        public bool IsSocietyMember { get; set; }
        public int SocietyId { get; set; }
        public long MemberId { get; set; }
        public string CustId { get; set; }
        public string DOB { get; set; }
        public string FCMToken { get; set; }
        public string OVD { get; set; }
        public string OVDType { get; set; }
        public int Attempt { get; set; }
        public string KYCStatus { get; set; }
        public DateTime CreatedOn { get; set; }
        public double? CardAmount { get; set; }

        //[Required]
        //[EmailAddress]
        //public string Email { get; set; }
        //public bool EmailConfirmed { get; set; }
    }


    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base(FieldConstants.CornoContext, throwIfV1Schema: false)
        {

        }
    }

    public class IdentityManager
    {
        #region -- Constructors --

        public IdentityManager()
        {
            var applicationDbContext = new ApplicationDbContext();
            _roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(applicationDbContext));
            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(applicationDbContext));

            var dataProtectionProvider = Startup.DataProtectionProvider;
            _userManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("EmailConfirmation"));

            // var provider = new DpapiDataProtectionProvider(Assembly.GetExecutingAssembly().GetName().Name);
            //  _userManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(provider.Create("EmailConfirmation"));
            _userManager.EmailService = new EmailService();
        }

        #endregion

        #region -- Data Members --

        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                var context = HttpContext.Current;

                return context.GetOwinContext().Authentication;
            }
        }

        #endregion

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            try
            {
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                var identity = await _userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                AuthenticationManager.SignIn(new AuthenticationProperties {IsPersistent = isPersistent}, identity);
            }
            catch
            {
                // Ignore
            }
        }

        public async Task<bool> Login(UserViewModel userViewModel)
        {
            var user =  _userManager.Find(userViewModel.UserName, userViewModel.Password);
            if (user == null) return false;
            await SignInAsync(user, false);
            return true;
        }

        public async Task<bool> Login(string userName, string password)
        {
            var user = _userManager.Find(userName, password);
            if (user == null) return false;
            await SignInAsync(user, false);
            return true;
        }

        public async Task<bool> ChangePassword(UserViewModel viewModel)
        {
            var user = _userManager.FindByName(viewModel.UserName);
            if (user == null) return false;
            var code = await _userManager.GeneratePasswordResetTokenAsync(user.Id);
            var result = _userManager.ResetPasswordAsync(user.Id, code, viewModel.Password);
            return result.Result.Succeeded;
        }

        public void SignOut()
        {
            AuthenticationManager.SignOut();
        }

        public bool RoleExists(string name)
        {
            return _roleManager.RoleExists(name);
        }

        public bool CreateRole(string name)
        {
            var idResult = _roleManager.Create(new IdentityRole(name));
            return idResult.Succeeded;
        }

        public bool EditRole(AspNetRole role)
        {
            var roleToUpdate = _roleManager.FindById(role.Id);
            var idResult = _roleManager.Update(roleToUpdate);
            return idResult.Succeeded;
        }

        public bool DeleteRole(AspNetRole role)
        {
            var roleToDelete = _roleManager.FindById(role.Id);
            var idResult = _roleManager.Delete(roleToDelete);
            return idResult.Succeeded;
        }


        public bool CreateUser(ApplicationUser user, string password)
        {
            var idResult = _userManager.Create(user, password);
            if (!idResult.Succeeded)
                throw new Exception(idResult.Errors.FirstOrDefault());
            return idResult.Succeeded;
        }

        public bool UserExists(string userName)
        {
            var user = _userManager.FindByName(userName);
            return null != user;
        }

        public bool UserExistsByPhoneNumber(string phoneNumber)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.PhoneNumber == phoneNumber);
            return null != user;
        }

        public ApplicationUser GetUserByDeviceId(string deviceId)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.DeviceId == deviceId);
            return user;
        }

        public ApplicationUser GetUser(string userId)
        {
            return _userManager.FindById(userId);
        }

        public ApplicationUser GetUserFromUserName(string userName)
        {
            return _userManager.FindByName(userName);
        }

        public int? GetAccountNo(string userId)
        {
            var user = _userManager.FindById(userId);
            return user?.AccountNo;
        }

        public string GetPhoneNumber(string userId)
        {
            return _userManager.GetPhoneNumber(userId);
        }
        
        public double? GetWallet(string userId)
        {
            var user = _userManager.FindById(userId);
            return user?.Wallet;
        }

        public void UpdateWallet(string userId, double? wallet)
        {
            var user = _userManager.FindById(userId);
            user.Wallet = wallet;
            _userManager.Update(user);
        }

        public void UpdateCard(string userId, double? wallet)
        {
            var user = _userManager.FindById(userId);
            user.CardAmount = wallet;
            _userManager.Update(user);
        }

        public bool UpdateUser(ApplicationUser user)
        {
            Logger.LogHandler.LogInfo("Update start", Logger.LogHandler.LogType.Notify);
            _userManager.Update(user);
            Logger.LogHandler.LogInfo("Update end", Logger.LogHandler.LogType.Notify);
            return true;
        }

        public bool AddUserToRole(string userId, string roleName)
        {
            var idResult = _userManager.AddToRole(userId, roleName);
            return idResult.Succeeded;
        }

        public void ClearUserRoles(string userId)
        {
            var user = _userManager.FindById(userId);
            var currentRoles = new List<IdentityUserRole>();
            currentRoles.AddRange(user.Roles);
            foreach (var role in currentRoles)
            {
            }
        }

        public List<ApplicationUser> GetUsersFCMToken()
        {
            return _userManager.Users.Where(x => !string.IsNullOrEmpty(x.FCMToken)).ToList();
        }
        
    }
}