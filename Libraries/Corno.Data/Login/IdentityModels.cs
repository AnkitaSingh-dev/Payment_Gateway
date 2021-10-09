using Corno.Globals;
using Corno.Logger;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Corno.Data.Login
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    //, ApplicationRole, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>
    {
        public ApplicationDbContext()
            : base(GlobalVariables.ConnectionString)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder); // This needs to go before the other rules!

        //    modelBuilder.Entity<ApplicationUser>().ToTable("AspNetUsers");
        //    modelBuilder.Entity<ApplicationRole>().ToTable("AspNetRoles");
        //    //modelBuilder.Entity<UserRole>().ToTable("UserRoles");
        //    //modelBuilder.Entity<UserLogin>().ToTable("UserLogins");
        //    //modelBuilder.Entity<UserClaim>().ToTable("UserClaims");
        //}
    }

    public class IdentityManager
    {
        #region -- Constructors --

        public IdentityManager()
        {
            var applicationDbContext = new ApplicationDbContext();
            _roleManager = new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(applicationDbContext));
            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(applicationDbContext));
        }

        #endregion

        #region -- Data Members --

        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        #endregion

        #region -- Methods (Roles) --

        public IEnumerable<ApplicationRole> GetRoles()
        {
            return _roleManager.Roles.ToList();
        }

        public ApplicationRole FindRoleById(string id)
        {
            return _roleManager.FindById(id);
        }

        public bool RoleExists(string name)
        {
            return _roleManager.RoleExists(name);
        }


        public bool CreateRole(ApplicationRole role)
        {
            IdentityResult idResult = null;
            try
            {
                idResult = _roleManager.Create(role);
                if (false == idResult.Succeeded)
                    throw new Exception(idResult.Errors.FirstOrDefault());
            }
            catch (Exception exception)
            {
                LogHandler.LogError(exception);
            }

            return idResult != null && idResult.Succeeded;
        }

        public bool EditRole(ApplicationRole role)
        {
            IdentityResult idResult = null;
            try
            {
                idResult = _roleManager.Update(role);
                if (false == idResult.Succeeded)
                    throw new Exception(idResult.Errors.FirstOrDefault());
            }
            catch (Exception exception)
            {
                LogHandler.LogError(exception);
            }

            return idResult != null && idResult.Succeeded;
        }

        public bool DeleteRole(ApplicationRole role)
        {
            IdentityResult idResult = null;
            try
            {
                idResult = _roleManager.Delete(role);
                if (false == idResult.Succeeded)
                    throw new Exception(idResult.Errors.FirstOrDefault());
            }
            catch (Exception exception)
            {
                LogHandler.LogError(exception);
            }

            return idResult != null && idResult.Succeeded;
        }

        #endregion

        #region -- Methods (Users) --

        public IEnumerable<ApplicationUser> GetUsers()
        {
            return _userManager.Users.ToList();
        }

        public ICollection<ApplicationUser> GetUsersInRole(string roleName)
        {
            var identityUserRoles = _roleManager.FindById(roleName).Users;
            ICollection<ApplicationUser> users = new List<ApplicationUser>();
            foreach (var indentityUserRole in identityUserRoles)
            {
                users.Add(_userManager.FindById(indentityUserRole.UserId));
            }

            return users;
        }

        public string GetUserName(string id)
        {
            var user = _userManager.FindById(id);
            if (null != user)
                return user.UserName;
            return string.Empty;
        }

        public ApplicationUser FindUserById(string id)
        {
            return _userManager.FindById(id);
        }

        public ApplicationUser FindUserByName(string name)
        {
            return _userManager.FindByName(name);
        }

        public bool IsUserInRole(string userId, string roleName)
        {
            return _userManager.IsInRole(userId, roleName);
        }

        public bool CreateUser(ApplicationUser user, string password)
        {
            IdentityResult idResult = null;
            try
            {
                idResult = _userManager.Create(user, password);
                if (false == idResult.Succeeded)
                    throw new Exception(idResult.Errors.FirstOrDefault());
            }
            catch (Exception exception)
            {
                LogHandler.LogError(exception);
            }

            return idResult != null && idResult.Succeeded;
        }

        public bool EditUser(ApplicationUser user)
        {
            IdentityResult idResult = null;
            try
            {
                idResult = _userManager.Update(user);
                if (false == idResult.Succeeded)
                    throw new Exception(idResult.Errors.FirstOrDefault());
            }
            catch (Exception exception)
            {
                LogHandler.LogError(exception);
            }

            return idResult != null && idResult.Succeeded;
        }

        public bool DeleteUser(ApplicationUser user)
        {
            IdentityResult idResult = null;
            try
            {
                idResult = _userManager.Delete(user);
                if (false == idResult.Succeeded)
                    throw new Exception(idResult.Errors.FirstOrDefault());
            }
            catch (Exception exception)
            {
                LogHandler.LogError(exception);
            }

            return idResult != null && idResult.Succeeded;
        }

        public bool VerifyUserNamePassword(string userName, string password)
        {
            var user = _userManager.Find(userName, password);
            return user != null;
        }

        public IList<string> GetUserRoles(string userId)
        {
            return _userManager.GetRoles(userId);
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
                _userManager.RemoveFromRole(userId, _roleManager.FindById(role.RoleId).Name);
            }
        }

        #endregion
    }
}