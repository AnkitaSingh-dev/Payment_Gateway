using Corno.Data.Base;
using Corno.Globals.Constants;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Corno.Raychem.CustomerPortal.Areas.Admin.Models
{
    public class VersionModel
    {
        public int version { get; set; }
        public int status { get; set; }
    }
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }
    }

    public class UserViewModel
    {
        public UserViewModel() { }

        public UserViewModel(ApplicationUser user)
        {
            Id = user.Id;
            UserName = user.UserName;
            FirstName = user.FirstName;
            LastName = user.LastName;
            PhoneNumber = user.PhoneNumber;
            Email = user.Email;
            UserType = user.UserType;
            Wallet = user.Wallet;
            DeviceId = user.DeviceId;
            AgencyName = user.UserType == UserTypeConstants.Customer ? string.Empty : user.AgencyName;
            IsKYCSubmit = user.IsKYCSubmit;
            IsEmailConfirmed = user.EmailConfirmed;
            IsSocietyMember = user.IsSocietyMember;
            SocietyId = user.SocietyId;
            MemberId = user.MemberId;
            CustId = user.CustId;
            DOB = user.DOB;
            FCMToken = user.FCMToken;
            isFingerPrint = false;
            OVD = user.OVD;
            OVDType = user.OVDType;
            KYCStatus = user.KYCStatus;
            CreatedOn = user.CreatedOn;
            CardAmount = user.CardAmount;
        }

        public string Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string NewPassword { get; set; }
        public string UserType { get; set; }
        public double? Wallet { get; set; }
        public string Otp { get; set; }
        public string DeviceId { get; set; }
        public string AgencyName { get; set; }
        public bool IsKYCSubmit { get; set; }
        public bool IsPanSubmit { get; set; }
        public bool IsAdhaarSubmit { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public bool IsSocietyMember { get; set; }
        public int SocietyId { get; set; }
        public long MemberId { get; set; }
        public string CustId { get; set; }
        public bool LockoutEnabled { get; set; }
        public string DOB { get; set; }
        public string FCMToken { get; set; }
        public bool isFingerPrint { get; set; }
        public string OVD { get; set; }
        public string OVDType { get; set; }
        public string KYCStatus { get; set; }
        public DateTime CreatedOn { get; set; }
        public double? CardAmount { get; set; }
    }

    public class ManageUserViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage =
            "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }


    public class LoginViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }


    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "User name")]
        //[RegularExpression(@"^[a-zA-Z]+[ a-zA-Z0-9-_]*$", ErrorMessage = "Please enter correct User Name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage =
            "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage =
            "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        // New Fields added to extend Application User class:

        [Required(ErrorMessage = "First Name is required")]
        [StringLength(100, MinimumLength = 3)]
        //[RegularExpression(@"^[a-zA-Z]+[ a-zA-Z0-9-_]*$", ErrorMessage = "Please enter correct First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(100, MinimumLength = 3)]
        //[RegularExpression(@"^[a-zA-Z]+[ a-zA-Z0-9-_]*$", ErrorMessage = "Please enter correct Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage ="User Type is required")]
        public string UserType { get; set; }
        //[Required(ErrorMessage = "Please enter your email address")]
        //[DataType(DataType.EmailAddress)]
        //[Display(Name = "Email address")]
        //[MaxLength(50)]
        //[RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessage = "Please enter correct email")]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int? AccountNo { get; set; }

        // Return a pre-poulated instance of AppliationUser:
        public ApplicationUser GetUser()
        {
            var user = new ApplicationUser()
            {
                UserName = UserName,
                FirstName = FirstName,
                LastName = LastName,
                PhoneNumber = PhoneNumber,
                Email = Email,
            };
            return user;
        }
        //public virtual ICollection<Company> Companies { get; set; }
    }


    public class EditUserViewModel
    {
        public EditUserViewModel() { }

        // Allow Initialization with an instance of ApplicationUser:
        public EditUserViewModel(ApplicationUser user)
        {
            Id = user.Id;
            UserName = user.UserName;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            PhoneNumber = user.PhoneNumber;
            AccountNo = user.AccountNo;
        }

        public string Id { get; set; }
        //[Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        //[Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        //[Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        //[Required]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int? AccountNo { get; set; }
    }


    public class SelectUserRolesViewModel
    {
        public SelectUserRolesViewModel()
        {
            Roles = new List<SelectRoleEditorViewModel>();
        }


        // Enable initialization with an instance of ApplicationUser:
        public SelectUserRolesViewModel(ApplicationUser user)
            : this()
        {
            UserName = user.UserName;
            FirstName = user.FirstName;
            LastName = user.LastName;

            var db = new ApplicationDbContext();

            // Add all available roles to the list of EditorViewModels:
            var allRoles = db.Roles;
            foreach (var role in allRoles)
            {
                // An EditorViewModel will be used by Editor Template:
                var rvm = new SelectRoleEditorViewModel(role);
                Roles.Add(rvm);
            }

            // Set the Selected property to true for those roles for 
            // which the current user is a member:
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(new ApplicationDbContext()));
            foreach (var userRole in user.Roles)
            {
                var checkUserRole =
                    Roles.Find(r => r.RoleName == roleManager.FindById(userRole.RoleId).Name);
                checkUserRole.Selected = true;
            }
        }

        public string Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<SelectRoleEditorViewModel> Roles { get; set; }
    }

    // Used to display a single role with a checkbox, within a list structure:
    public class SelectRoleEditorViewModel
    {
        public SelectRoleEditorViewModel() { }
        public SelectRoleEditorViewModel(IdentityRole role)
        {
            RoleName = role.Name;
        }

        public bool Selected { get; set; }

        [Required]
        public string RoleName { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    //public class ResetPasswordViewModel
    //{

    //    //[Display(Name = "User name")]
    //    //public string UserName { get; set; }

    //    //[Required]
    //    [StringLength(100, ErrorMessage =
    //        "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    //    [DataType(DataType.Password)]
    //    [Display(Name = "Old Password")]
    //    public string Password { get; set; }

    //    [Required]
    //    [StringLength(100, ErrorMessage =
    //        "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    //    [DataType(DataType.Password)]
    //    [Display(Name = "New Password")]
    //    public string NewPassword { get; set; }

    //    [DataType(DataType.Password)]
    //    [Display(Name = "Confirm password")]
    //    [Compare("NewPassword", ErrorMessage =
    //        "The password and confirmation password do not match.")]
    //    public string ConfirmPassword { get; set; }

    //    // New Fields added to extend Application User class:

    //    //[Required]
    //    //[Display(Name = "First Name")]
    //    //public string FirstName { get; set; }

    //    //[Required]
    //    //[Display(Name = "Last Name")]
    //    //public string LastName { get; set; }

    //    //[Required]
    //    //public string Email { get; set; }
    //}

    public class UserProfileViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public int? AccountNo { get; set; }

        // Return a pre-poulated instance of AppliationUser:
        public ApplicationUser GetUser()
        {
            var user = new ApplicationUser()
            {
                FirstName = FirstName,
                LastName = LastName,
                PhoneNumber = PhoneNumber,
                Email = Email
            };
            return user;
        }
    }
    public class KYCModel
    {
        public string UserName { get; set; }

        public string DeviceId { get; set; }

        public string AdharCardImageFront { get; set; }
        public string AdharCardImageBack { get; set; }
        public string AdharCardNumber { get; set; }

        public string PanNumber { get; set; }
        public string PanImage { get; set; }

        public string AccountNumber { get; set; }
        public string IFSC { get; set; }

    }
    public class UserKYCModel : BaseModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string DeviceId { get; set; }

        public string AdharCardImageFront { get; set; }
        public string AdharCardImageBack { get; set; }

        public string PanNumber { get; set; }
        public string PanImage { get; set; }

        public string AccountNumber { get; set; }
        public string IFSC { get; set; }

    }

}
