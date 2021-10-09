using Corno.Data.Base;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Corno.Data.Login
{
    //[NotMapped]
    public class ApplicationUser : IdentityUser, ICornoModel
    {
        #region -- Constructors --

        #endregion

        #region -- Methods --

        public void Reset()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        #endregion

        #region -- Properties --
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        //[Required]
        //[EmailAddress]
        //public string Email { get; set; }
        //public bool EmailConfirmed { get; set; }

        #endregion
    }
}