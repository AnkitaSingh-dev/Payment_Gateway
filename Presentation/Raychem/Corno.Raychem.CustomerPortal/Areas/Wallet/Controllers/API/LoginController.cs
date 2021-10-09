using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;
using Corno.Globals.Constants;
using Corno.Logger;
using Corno.Raychem.CustomerPortal.Areas.Admin.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces;
using Corno.Services.Email.Interfaces;
using Corno.Services.Bootstrapper;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using System.Drawing;
using Corno.Services.Encryption.Interfaces;
using Corno.Services.Encryption;
using System.Net.Http.Headers;
using System.Web.Script.Serialization;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Constants;
using Newtonsoft.Json;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Controllers
{
    public class LoginController : ApiController
    {
        #region -- Constructors --

        public LoginController(IdentityManager identityManager, IEmailService emailService,
            IWalletBaseService walletBaseService, IWalletService walletService)
        {
            _identityManager = identityManager;
            _emailService = emailService;
            _walletBaseService = walletBaseService;
            _walletService = walletService;
        }
        #endregion

        #region -- Data Members --

        private readonly IdentityManager _identityManager;
        private readonly IEmailService _emailService;
        private readonly IWalletBaseService _walletBaseService;
        private readonly IWalletService _walletService;
        #endregion

        #region -- Methods --

        public void GenerateAndSendOtp(ApplicationUser user, string message)
        {
            // Generate Otp
            var otp = GenerateOtp();

            // Send otp by sms
            message = message.Replace("@otp", otp);
            TboBaseController.SendOtp(user.UserName, message, otp, null);

            // Store Otp in user.
            user.Otp = otp;
            _identityManager.UpdateUser(user);
        }

        public string GenerateOtp()
        {
            // Generate 6 digit otp
            var generator = new Random();
            return generator.Next(0, 1000000).ToString("D6");
        }
        #endregion

        [HttpGet]
        public HttpResponseMessage GetUserTypes()
        {
            var userTypes = new[] { "Customer", "Merchant" };

            return Request.CreateResponse(HttpStatusCode.OK, userTypes);
        }

        [HttpGet]
        public HttpResponseMessage GetVersionNumber()
        {
            return Request.CreateResponse(new VersionModel { status = 200, version = Convert.ToInt32(WebConfigurationManager.AppSettings["Version"]) });
            //return Request.CreateResponse(HttpStatusCode.OK,version);
        }

        public HttpResponseMessage ResendOtp(UserViewModel viewModel)
        {
            var result = "OTP resend Successful";
            var bStatus = true;
            var otp = string.Empty;
            try
            {
                if (null == viewModel)
                    throw new Exception("Bad Request");

                var user = _walletBaseService.GetUser(viewModel.UserName, _identityManager); //_identityManager.GetUserFromUserName(viewModel.UserName));
                var message = "Your OTP  is @otp. Please, don't disclose it to anybody.";
                if (null == user)
                {
                    viewModel.UserName = _walletBaseService.Decrypt(viewModel.UserName, FieldConstants.LoginIv);

                    // Generate Otp
                    otp = GenerateOtp();

                    // Send otp by sms
                    message = message.Replace("@otp", otp);
                    TboBaseController.SendOtp(viewModel.UserName, message, otp, null);
                    result = "User does not exists!";
                    //throw new Exception("User does not exists!");
                }
                else
                {
                    // Generate & Send OTP
                    //const string message = "Your OTP  is @otp. Please, don't disclose it to anybody.";
                    GenerateAndSendOtp(user, message);
                    otp = user.Otp;
                }
            }
            catch (Exception exception)
            {
                result = LogHandler.LogError(exception).Message;
                bStatus = false;
            }

            var returnViewModel = new ResponseModel
            {
                Status = bStatus,
                Result = result,
                WalletBalance = 0
                //   Otp = otp
            };
            return Request.CreateResponse(HttpStatusCode.OK, returnViewModel);
        }

        public HttpResponseMessage OtpConfirm(UserViewModel viewModel)
        {
            var status = "Otp confirmed.";
            var bStatus = true;
            try
            {
                if (null == viewModel)
                    throw new Exception("Bad Request");

                var user = _walletBaseService.GetUser(viewModel.UserName, _identityManager); //_identityManager.GetUserFromUserName(viewModel.UserName);
                if (null == user)
                {
                    viewModel.UserName = _walletBaseService.Decrypt(viewModel.UserName, FieldConstants.LoginIv);
                    status = viewModel.UserName + "'s Account does not exist !";
                }
                else
                {
                    viewModel.Otp = _walletBaseService.Decrypt(viewModel.Otp, FieldConstants.Iv);
                    //_walletBaseService.ValidateDeviceId(new RequestModel { DeviceId = viewModel.DeviceId}, user);
                    if (user.Otp != viewModel.Otp)
                        throw new Exception("The OTP you received, does not match with the system");

                    user.PhoneNumberConfirmed = true;
                    _identityManager.UpdateUser(user);
                }
            }
            catch (Exception exception)
            {
                status = LogHandler.LogError(exception).Message;
                bStatus = false;
            }

            var returnViewModel = new ResponseModel
            {
                Status = bStatus,
                Result = status,
                WalletBalance = 0,
                IsAuthenticated = bStatus
            };
            return Request.CreateResponse(HttpStatusCode.OK, returnViewModel);
        }

        public HttpResponseMessage Register(UserViewModel viewModel)
        {
            var status = "Registration Successful & Otp sent to your mobile number";
            try
            {
                if (null == viewModel)
                    throw new Exception("Bad Request");

                var existingUser = _walletBaseService.GetUser(viewModel.UserName, _identityManager); //_identityManager.GetUserFromUserName(user.UserName);
                if (null != existingUser)
                    throw new Exception("This user already exists!");

                var deviceId = _walletBaseService.Decrypt(viewModel.DeviceId, FieldConstants.Iv);
                existingUser = _identityManager.GetUserByDeviceId(deviceId);
                if (null != existingUser && false == existingUser.LockoutEnabled)
                    throw new Exception("The user already exists and is active for this device id ");

                // Generate & Send OTP
                var message = "%3C%23%3E Your Jugaad verification code is @otp. Message ID: " + ServiceConstants.SMSProd ;

                // Generate Otp
                var otp = GenerateOtp();

                var userName = _walletBaseService.Decrypt(viewModel.UserName, FieldConstants.LoginIv);
                var password = _walletBaseService.Decrypt(viewModel.Password, FieldConstants.LoginIv);

                if (!Regex.Match(viewModel.FirstName, @"^[_A-z0-9]*((-|\s)*[_A-z0-9])*$").Success || !Regex.Match(viewModel.LastName, @"^[_A-z0-9]*((-|\s)*[_A-z0-9])*$").Success)
                    throw new Exception("Name cannot contain special characters.");

                if (viewModel.IsSocietyMember)
                    if (CheckIfValidSocietyMember(viewModel.SocietyId, viewModel.MemberId).ToUpper() != StatusConstants.Success.ToUpper())
                        throw new Exception("This is not a valid Member Id for society, Please contact society support for further assistance");

                // Send otp by sms
                message = message.Replace("@otp", otp);
                TboBaseController.SendSms(userName, message, null);

                var user = new ApplicationUser
                {
                    UserName = userName,
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    PhoneNumber = viewModel.PhoneNumber,
                    PhoneNumberConfirmed = false,
                    Email = viewModel.Email,
                    EmailConfirmed = false,
                    UserType = "Customer",
                    DeviceId = viewModel.DeviceId,
                    AgencyName = viewModel.UserType == UserTypeConstants.Merchant ? viewModel.AgencyName: string.Empty,
                    IsKYCSubmit = false,
                    IsPanSubmit = false,
                    IsAdhaarSubmit = false,
                    IsSocietyMember = viewModel.IsSocietyMember,
                    SocietyId = viewModel.SocietyId,
                    MemberId = viewModel.MemberId,
                    OVD = viewModel.OVD,
                    OVDType = viewModel.OVDType,
                    CreatedOn = DateTime.Now
                };
                var result = _identityManager.CreateUser(user, password);
                if (result)
                {
                    // Store Otp in user.
                    var registeredUser = _walletBaseService.GetUser(viewModel.UserName, _identityManager); //_identityManager.GetUserFromUserName(user.UserName);
                    registeredUser.Otp = otp;
                    _identityManager.UpdateUser(registeredUser);

                    var returnViewModelSuccess = new ResponseModel
                    {
                        Status = true,
                        Result = status,
                        WalletBalance = 0,
                        UserInfo = new UserViewModel
                        {
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            PhoneNumber = user.PhoneNumber,
                            Email = user.Email,
                            UserType = user.UserType,
                            DeviceId = user.DeviceId
                        }
                    };

                    return Request.CreateResponse(HttpStatusCode.OK, returnViewModelSuccess);
                }

                status = "Unable to create user.";
            }
            catch (Exception exception)
            {
                status = LogHandler.LogError(exception).Message;
            }

            var returnViewModel = new ResponseModel
            {
                Status = false,
                Result = status,
                WalletBalance = 0,
                IsAuthenticated = false
            };
            return Request.CreateResponse(HttpStatusCode.OK, returnViewModel);
        }

        public HttpResponseMessage RegisterConfirm(UserViewModel viewModel)
        {
            var status = "User registration confirmed.";
            var bStatus = true;
            try
            {
                if (null == viewModel)
                    throw new Exception("Bad Request");

                var user = _walletBaseService.GetUser(viewModel.UserName, _identityManager); //_identityManager.GetUserFromUserName(viewModel.UserName);
                if (null == user)
                    throw new Exception(viewModel.UserName + "'s Account does not exist!");

                viewModel.Otp = _walletBaseService.Decrypt(viewModel.Otp, FieldConstants.Iv);
                if (user.Otp != viewModel.Otp)
                    throw new Exception("The OTP you received, does not match with the system.");

                user.PhoneNumberConfirmed = true;
                _identityManager.UpdateUser(user);

                // Send Email & SMS
                var smsBody = "Dear Customer, Thank you for registering to our application. Welcome to 4everPay community. Please verify your Email by clicking on the link sent to your registered email-Id.( " + user.Email +")";
                TboBaseController.SendSms(user.UserName, smsBody, "Registration Confirmation");

                var emailBody = string.Format("Dear Customer, <br><br><br>You received this message because you registered to our application. <br>Welcome to 4everPay community.<br><br> Please click on the link below to confirm the email id : <br><br> <a href = \"" + FieldConstants.LiveEmailUrl + "\"> Confirm My Email Id </a><br><br>Thank You,<br>Jugaad Team", System.Web.HttpUtility.UrlEncode(viewModel.UserName)); //43.252.195.170
                _emailService.SendEmailAsync(user.Email, "Registration Confirmation", emailBody);
            }
            catch (Exception exception)
            {
                status = LogHandler.LogError(exception).Message;
                bStatus = false;
            }

            var returnViewModel = new ResponseModel
            {
                Status = bStatus,
                Result = status,
                WalletBalance = 0,
                IsAuthenticated = bStatus
            };
            return Request.CreateResponse(HttpStatusCode.OK, returnViewModel);
        }

        public async Task<HttpResponseMessage> Login(UserViewModel viewModel)
        {
            var status = "Login Successful.";
            var result = false;

            try
            {

                if (null == viewModel)
                    throw new Exception("Bad Request");

                var user = _walletBaseService.GetUser(viewModel.UserName, _identityManager); //_identityManager.GetUserFromUserName(viewModel.UserName);
                if (null == user)
                    throw new Exception("Account does not exist!");
                //if (user.DeviceId == null || user.DeviceId != viewModel.DeviceId)
                //    throw new Exception("You are trying to login with another device other than registered device.");

                if (!user.PhoneNumberConfirmed)
                    throw new Exception(
                        "Phone number is not confirmed is during Registration. Please, confirm it before login");

                if (user.LockoutEnabled)
                    throw new Exception("User is locked");

                if (!viewModel.isFingerPrint)
                {
                    var userName = _walletBaseService.Decrypt(viewModel.UserName, FieldConstants.LoginIv);
                    var password = _walletBaseService.Decrypt(viewModel.Password, FieldConstants.LoginIv);
                    result = await _identityManager.Login(userName, password);
                }

                result = viewModel.isFingerPrint ? true : result;

                if (result)
                {
                     user.Attempt = 0;
                     _identityManager.UpdateUser(user);

                    var returnViewModelSuccess = new ResponseModel
                    {
                        Status = true,
                        Result = status,
                        WalletBalance = user.Wallet ?? 0,
                        UserInfo = new UserViewModel
                        {
                            UserName = user.UserName,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            PhoneNumber = user.PhoneNumber,
                            Email = user.Email,
                            UserType = user.UserType,
                            DeviceId = user.DeviceId,
                            IsSocietyMember = user.IsSocietyMember,
                            SocietyId = user.SocietyId,
                            MemberId = user.MemberId,
                        },
                        IsAuthenticated = true,
                    };

                    return Request.CreateResponse(HttpStatusCode.OK, returnViewModelSuccess);
                }
                else
                {
                    user.Attempt = user.Attempt + 1;
                    _identityManager.UpdateUser(user);

                    if (user.Attempt == 4)
                    {
                        user.LockoutEnabled = true;
                        _identityManager.UpdateUser(user);
                        TboBaseController.SendSms(user.UserName, "Your account has been blocked temporarily due to 3 invalid login attempts. Please call us on 022-28272727 or email at support@4everpayment.com to unblock your account. JUGAD team", string.Empty);
                        _emailService.SendEmail("support@4everpayment.com", "User Account Locked" + user.UserName, "Wrong password entered for mobile number "+ user.UserName,null,null);
                        throw new Exception("Account locked, Please try after 6hrs or contact our support team");
                    }      
                }

                var attempt = 4 - user.Attempt;
                status = "Invalid User Name or Password."+ attempt +" attempts left.";
            }
            catch (Exception exception)
            {
                status = LogHandler.LogError(exception).Message;
            }

            var returnViewModel = new ResponseModel
            {
                Status = false,
                Result = status,
                WalletBalance = 0,
                IsAuthenticated = false
            };
            return Request.CreateResponse(HttpStatusCode.OK, returnViewModel);
        }

        public async Task<HttpResponseMessage> ChangePassword(UserViewModel viewModel)
        {
            var status = "Change Password Successful.";
            try
            {
                if (null == viewModel)
                    throw new Exception("Bad Request");

                var user = _walletBaseService.GetUser(viewModel.UserName, _identityManager); //_identityManager.GetUserFromUserName(viewModel.UserName);
                if (null == user)
                    throw new Exception(viewModel.UserName + "'s Account does not exist!");

                viewModel.Otp = _walletBaseService.Decrypt(viewModel.Otp, FieldConstants.Iv);
                if (user.Otp != viewModel.Otp)
                    throw new Exception("The OTP you received, does not match with the system.");

                viewModel.UserName = user.UserName;
                viewModel.Password = _walletBaseService.Decrypt(viewModel.Password, FieldConstants.LoginIv);

                var result = await _identityManager.ChangePassword(viewModel);
                if (result)
                {
                    //var wallet = _identityManager.GetWallet(user.Id);
                    var returnViewModelSuccess = new ResponseModel
                    {
                        Status = true,
                        Result = status,
                        WalletBalance = user.Wallet ?? 0,
                        UserInfo = new UserViewModel
                        {
                            UserName = user.UserName,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            PhoneNumber = user.PhoneNumber,
                            Email = user.Email,
                            UserType = user.UserType
                        }
                    };

                    // Send Email & SMS
                    var emailBody = "Dear Customer, <br><br><br>You Your password is changed successfully. Kindly, don't disclose it to anybody.";
                    _emailService.SendEmailAsync(user.Email, "Change Password Confirmation", emailBody);
                    var smsBody = "Dear Customer, You Your password is changed successfully. Kindly, don't disclose it to anybody.";
                    TboBaseController.SendSms(user.UserName, smsBody, "Change Passsword Confirmation");

                    return Request.CreateResponse(HttpStatusCode.OK, returnViewModelSuccess);
                }

                status = "Invalid User Name or Password.";
            }
            catch (Exception exception)
            {
                status = LogHandler.LogError(exception).Message;
            }

            var returnViewModel = new ResponseModel
            {
                Status = false,
                Result = status,
                WalletBalance = 0,
            };
            return Request.CreateResponse(HttpStatusCode.OK, returnViewModel);
        }

        public HttpResponseMessage ForgotPassword(UserViewModel viewModel)
        {
            var status = "OTP sent successfully";
            try
            {
                if (null == viewModel)
                    throw new Exception("Bad Request");

                var user = _walletBaseService.GetUser(viewModel.UserName, _identityManager); //_identityManager.GetUserFromUserName(viewModel.UserName);
                if (null == user)
                    throw new Exception(viewModel.UserName + "'s Account does not exist!");

                // Generate & Send OTP
                var message = "Your OTP to change 4everPay Application Password is @otp. Please, don't disclose it to anybody.";
                GenerateAndSendOtp(user, message);

                var wallet = _identityManager.GetWallet(user.Id);
                var returnViewModelSuccess = new ResponseModel
                {
                    Status = true,
                    Result = status,
                    WalletBalance = wallet ?? 0,
                    //Otp = user.Otp
                    //UserInfo = new UserViewModel
                    //{
                    //    UserName = user.UserName,
                    //    FirstName = user.FirstName,
                    //    LastName = user.LastName,
                    //    PhoneNumber = user.PhoneNumber,
                    //    Email = user.Email,
                    //    UserType = user.UserType,
                    //    Otp = user.Otp
                    //}
                };
                return Request.CreateResponse(HttpStatusCode.OK, returnViewModelSuccess);
            }
            catch (Exception exception)
            {
                status = LogHandler.LogError(exception).Message;
            }

            var returnViewModel = new ResponseModel
            {
                Status = false,
                Result = status,
                WalletBalance = 0,
            };
            return Request.CreateResponse(HttpStatusCode.OK, returnViewModel);
        }

        public HttpResponseMessage LogOff(UserViewModel viewModel)
        {
            var status = "Logout Successful.";
            try
            {
                if (null == viewModel)
                    throw new Exception("Bad Request");

                _identityManager.SignOut();

                var user = _walletBaseService.GetUser(viewModel.UserName, _identityManager); //_identityManager.GetUserFromUserName(viewModel.UserName);
                if (null == user)
                    throw new Exception(viewModel.UserName + "'s Account does not exist!");
                var returnViewModelSuccess = new ResponseModel
                {
                    Status = true,
                    Result = status,
                    WalletBalance = 0,
                    UserInfo = new UserViewModel
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        PhoneNumber = user.PhoneNumber,
                        Email = user.Email,
                        UserType = user.UserType
                    }
                };
                return Request.CreateResponse(HttpStatusCode.OK, returnViewModelSuccess);
            }
            catch (Exception exception)
            {
                status = LogHandler.LogError(exception).Message;
            }
            var returnViewModel = new ResponseModel
            {
                Status = false,
                Result = status,
                WalletBalance = 0,
            };
            return Request.CreateResponse(HttpStatusCode.OK, returnViewModel);
        }
        public HttpResponseMessage SubmitAdhaarDetails(KYCModel kycModel)
        {
            var message = "Adhaar validated successfully";
            var status = true;
            try
            {
                if (null == kycModel)
                {
                    status = false;
                    throw new Exception("Bad Request");
                }

                var user = _walletBaseService.GetUser(kycModel.UserName, _identityManager);

                if (null == user)
                {
                    status = false;
                    throw new Exception(kycModel.UserName + "'s Account does not exist!");
                }
                _walletBaseService.ValidateDeviceId(new RequestModel { DeviceId = kycModel.DeviceId }, _walletBaseService.GetUser(kycModel.UserName, _identityManager));
                //var mobileWareService = (IMobileWareApiService)Bootstrapper.GetService(typeof(MobileWareApiService));

               // message = mobileWareService.CreateURL(kycModel);

                var kycService = (IUserKYCService)Bootstrapper.GetService(typeof(UserKYCService));
                var isExist = kycService.Get(w => w.UserName == user.UserName).FirstOrDefault();

                string subPath = WebConfigurationManager.AppSettings["KYCPath"] + "\\" + user.UserName; // your code goes here

                bool exists = System.IO.Directory.Exists(subPath);

                if (!exists)
                    System.IO.Directory.CreateDirectory(subPath);

                SaveImage(kycModel.AdharCardImageFront).Save(subPath +"\\" + user.UserName + "_AdharFront.jpg");
                SaveImage(kycModel.AdharCardImageBack).Save(subPath + "\\" + user.UserName + "_AdharBack.jpg");

                if (isExist != null)
                {
                    isExist.AdharCardImageFront = subPath + "\\" + user.UserName + "_AdharFront";
                    isExist.AdharCardImageBack = subPath + "\\" + user.UserName + "_AdharBack";

                    kycService.Update(isExist);
                    kycService.Save();
                }
                else
                {
                    if (kycModel.AdharCardNumber.ToString().Length != 12)
                    {
                        status = false;
                        throw new Exception("Invalid Adhaar Number");
                    }

                    var viewModel = new UserKYCModel
                    {
                        DeviceId = kycModel.DeviceId,
                        UserName = user.UserName,
                        AdharCardImageFront = subPath + "\\" + user.UserName + "_AdharFront",
                        AdharCardImageBack = subPath + "\\" + user.UserName + "_AdharBack"
                    };

                    kycService.Add(viewModel);
                    kycService.Save();
                }

                user.IsAdhaarSubmit = true;
                _identityManager.UpdateUser(user);
            }
            catch (Exception exception)
            {
                status = false;
                message = exception.Message;
                LogHandler.LogError(exception);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { Result = message, Status = status });
        }
        public HttpResponseMessage SubmitPanDetails(KYCModel kycModel)
        {
            var message = "PAN details submitted successfully";
            var smsmessage = "Your OTP to confirm submission of KYC is @otp. Please, don't disclose it to anybody.";
            var status = true;
            var otp = string.Empty;
            try
            {
                if (null == kycModel)
                {
                    status = false;
                    throw new Exception("Bad Request");
                }

                var user = _walletBaseService.GetUser(kycModel.UserName, _identityManager);

                if (null == user)
                {
                    status = false;
                    throw new Exception(kycModel.UserName + "'s Account does not exist!");
                }
                _walletBaseService.ValidateDeviceId(new RequestModel { DeviceId = kycModel.DeviceId }, _walletBaseService.GetUser(kycModel.UserName, _identityManager));
               // var mobileWareService = (IMobileWareApiService)Bootstrapper.GetService(typeof(MobileWareApiService));

                kycModel.DeviceId = _walletService.Decrypt(kycModel.DeviceId, FieldConstants.Iv);
                // var validationResult = mobileWareService.PANValidateURL(kycModel);

                // if (validationResult.Status != "00")
                //   throw new Exception("Invalid PAN number, Please check.");

                if (!Regex.Match(kycModel.PanNumber, @"[A-Z]{5}\d{4}[A-Z]{1}").Success)
                {
                    status = false;
                    throw new Exception("Invalid PAN number");
                }

                var kycService = (IUserKYCService)Bootstrapper.GetService(typeof(UserKYCService));
                var isExist = kycService.Get(w => w.UserName == user.UserName).FirstOrDefault();

                string subPath = WebConfigurationManager.AppSettings["KYCPath"] + "\\" + user.UserName; // your code goes here
                bool exists = System.IO.Directory.Exists(subPath);

                if (!exists)
                    System.IO.Directory.CreateDirectory(subPath);

                var path = subPath +"\\" + user.UserName + "_Pan.jpg";
                SaveImage(kycModel.PanImage).Save(path);

                if (isExist != null)
                {
                    isExist.PanNumber = _walletService.Encrypt(kycModel.PanNumber, FieldConstants.KYCIv);
                    isExist.PanImage = path;

                    kycService.Update(isExist);
                    kycService.Save();
                }
                else
                {
                    var viewModel = new UserKYCModel
                    {
                        DeviceId = kycModel.DeviceId,
                        UserName = user.UserName,
                        PanImage = path,
                        PanNumber = _walletService.Encrypt(kycModel.PanNumber, FieldConstants.KYCIv)
                };

                    kycService.Add(viewModel);
                    kycService.Save();
                }

                otp = GenerateOtp();

                // Send otp by sms
                smsmessage = smsmessage.Replace("@otp", otp);
                TboBaseController.SendSms(user.UserName, smsmessage, null);

                // Store Otp in user.
                user.Otp = otp;
                _identityManager.UpdateUser(user);

            }
            catch (Exception exception)
            {
                status = false;
                message = exception.Message;
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { Result = message, Status = status, Otp = otp});
        }

        public HttpResponseMessage SubmitAccountDetails (KYCModel viewModel)
        {
            var message = "Account details validated successfully";
            var status = true;

            try
            {
                if (null == viewModel)
                {
                    status = false;
                    throw new Exception("Bad Request"); 
                }

                var user = _walletBaseService.GetUser(viewModel.UserName, _identityManager);

                if (null == user)
                {
                    status = false;
                    throw new Exception(viewModel.UserName + "'s Account does not exist!"); 
                }
                _walletBaseService.ValidateDeviceId(new RequestModel { DeviceId = viewModel.DeviceId }, _walletBaseService.GetUser(viewModel.UserName, _identityManager));

                var aes256Service = (IAes256Service)Bootstrapper.GetService(typeof(Aes256Service));
                var hashKey = aes256Service.Md5Hash(FieldConstants.MerchantId + user.UserName + viewModel.AccountNumber + viewModel.IFSC);

               // var mobileWareService = (IMobileWareApiService)Bootstrapper.GetService(typeof(MobileWareApiService));
               // var validationResult = mobileWareService.AccountValidateURL(viewModel, hashKey, user);

               // if (validationResult.ErrorCode != "00")
                //    throw new Exception(validationResult.Reason);

                var kycService = (IUserKYCService)Bootstrapper.GetService(typeof(UserKYCService));
                var isExist = kycService.Get(w => w.UserName == user.UserName).FirstOrDefault();

                if (isExist != null)
                {

                    if (!Regex.Match(viewModel.IFSC, @"[A-Z|a-z]{4}[0][\d]{6}$").Success)
                    {
                        status = false;
                        throw new Exception("Invalid IFSC code");
                    }
                    isExist.AccountNumber = _walletService.Encrypt(viewModel.AccountNumber, FieldConstants.KYCIv);
                    isExist.IFSC = _walletService.Encrypt(viewModel.IFSC, FieldConstants.KYCIv);

                    kycService.Update(isExist);
                    kycService.Save();
                }
                else
                {
                    var kycmodel = new UserKYCModel
                    {
                        AccountNumber = viewModel.AccountNumber,
                        IFSC = viewModel.IFSC
                    };

                    kycService.Add(kycmodel);
                    kycService.Save();
                }

            }
            catch (Exception exception)
            {
                    status = false;
                    message = exception.Message;
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { Result = message, Status = status});
        }
        public HttpResponseMessage OtpConfirmKYC (UserViewModel viewModel)
        {
            var status = "Otp confirmed.";
            var bStatus = true;
            try
            {
                if (null == viewModel)
                    throw new Exception("Bad Request");

                var user = _walletBaseService.GetUser(viewModel.UserName, _identityManager); //_identityManager.GetUserFromUserName(viewModel.UserName);
                if (null == user)
                {
                    viewModel.UserName = _walletBaseService.Decrypt(viewModel.UserName, FieldConstants.LoginIv);
                    status = viewModel.UserName + "'s Account does not exist !";
                }
                else
                {
                   // _walletBaseService.ValidateDeviceId(new RequestModel { DeviceId = viewModel.DeviceId }, user);
                    if (user.Otp != viewModel.Otp)
                        throw new Exception("The OTP you received, does not match with the system");

                    user.IsKYCSubmit = true;
                    user.IsPanSubmit = true;
                    _identityManager.UpdateUser(user);
                }
            }
            catch (Exception exception)
            {
                status = LogHandler.LogError(exception).Message;
                bStatus = false;
            }

            var returnViewModel = new ResponseModel
            {
                Status = bStatus,
                Result = status,
                WalletBalance = 0,
                IsAuthenticated = bStatus
            };
            return Request.CreateResponse(HttpStatusCode.OK, returnViewModel);
        }

        private Image SaveImage (string image)
        {
            byte[] imageBytes = Convert.FromBase64String(image);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            ms.Write(imageBytes, 0, imageBytes.Length);
            System.Drawing.Image images = System.Drawing.Image.FromStream(ms, true);

            return images;  
        }

        [HttpGet]
        public HttpResponseMessage ConfirmEmail (string key)
        {
            var user = _walletBaseService.GetUser(key, _identityManager); //_identityManager.GetUserFromUserName(viewModel.UserName);

            user.EmailConfirmed = true;
            _identityManager.UpdateUser(user);
            var path = System.AppDomain.CurrentDomain.BaseDirectory.ToString();

            var response = new HttpResponseMessage();
            response.Content = new StringContent(File.ReadAllText(path + @"\EmailConfirm.html"));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }

        public HttpResponseMessage EmailConfirmGenerate(UserViewModel viewModel)
        {
            var status = "Email confirmation mail sent.";
            var bStatus = true;
            try
            {
                if (null == viewModel)
                    throw new Exception("Bad Request");

                var user = _walletBaseService.GetUser(viewModel.UserName, _identityManager); //_identityManager.GetUserFromUserName(viewModel.UserName);
                if (null == user)
                    throw new Exception(viewModel.UserName + "'s Account does not exist!");

                // Send Email
                var emailBody = string.Format("Dear Customer, <br><br><br> Please click on the link below to confirm the email id : <br><br> <a href = \""+ FieldConstants.LiveEmailUrl + "\"> Confirm My Email Id </a><br><br>Thank You,<br>Jugaad Team", System.Web.HttpUtility.UrlEncode(viewModel.UserName)); 
                _emailService.SendEmailAsync(user.Email, "Email Confirmation", emailBody);

                var smsBody = "Dear Customer, a link has been sent on your emailId registered with JUGAD ( "+ user.Email +" ) for Email-Verification";
                TboBaseController.SendSms(user.UserName, smsBody, "Email Verification");
            }
            catch (Exception exception)
            {
                status = LogHandler.LogError(exception).Message;
                bStatus = false;
            }

            var returnViewModel = new ResponseModel
            {
                Status = bStatus,
                Result = status
            };
            return Request.CreateResponse(HttpStatusCode.OK, returnViewModel);
        }

        [HttpGet]
        public HttpResponseMessage EndUserLicenseAgreement()
        {
            var path = System.AppDomain.CurrentDomain.BaseDirectory.ToString();

            var response = new HttpResponseMessage();
            response.Content = new StringContent(File.ReadAllText(path + @"\end-user-license-agreement.html"));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }
        public HttpResponseMessage OtpConfirmUpdateDevice(UserViewModel viewModel)
        {
            var status = "New device registered successfully!";
            var bStatus = true;
            try
            {
                if (null == viewModel)
                    throw new Exception("Bad Request");

                var user = _walletBaseService.GetUser(viewModel.UserName, _identityManager); //_identityManager.GetUserFromUserName(viewModel.UserName);
                if (null == user)
                {
                    viewModel.UserName = _walletBaseService.Decrypt(viewModel.UserName, FieldConstants.LoginIv);
                    status = viewModel.UserName + "'s Account does not exist !";
                }
                else
                {
                    viewModel.Otp = _walletBaseService.Decrypt(viewModel.Otp, FieldConstants.Iv);

                    // _walletBaseService.ValidateDeviceId(new RequestModel { DeviceId = viewModel.DeviceId }, user);
                    if (user.Otp != viewModel.Otp)
                        throw new Exception("The OTP does not match.");

                    user.DeviceId = viewModel.DeviceId;
                    _identityManager.UpdateUser(user);
                }
            }
            catch (Exception exception)
            {
                status = LogHandler.LogError(exception).Message;
                bStatus = false;
            }

            var returnViewModel = new ResponseModel
            {
                Status = bStatus,
                Result = status,
                WalletBalance = 0,
                IsAuthenticated = bStatus
            };
            return Request.CreateResponse(HttpStatusCode.OK, returnViewModel);
        }

        [HttpGet]
        public string CheckIfValidSocietyMember(int societyId, long memberId)
        {
            dynamic html;
            string url = string.Empty;

            try
            {
                if (memberId < 0)
                    throw new Exception("Invalid Society Id or Member Id, Please check.");

                switch (societyId)
                {
                    case 142:
                        url =  TboConstants.SSVUrl + memberId + "&SocietyId=" + societyId;
                        break;
                    case 143:
                        url =  TboConstants.SSCUrl + memberId + "&SocietyId=" + societyId;
                        break;
                    case 144:
                        url =  TboConstants.HUMUrl + memberId + "&SocietyId=" + societyId;
                        break;
                    case 146:
                        url =  TboConstants.LUCCUrl + memberId + "&SocietyId=" + societyId;
                        break;
                }

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    html = new JavaScriptSerializer().Deserialize<SocietyResponseModel[]>(reader.ReadToEnd());
                }

                if (!html[0].Status)
                {
                    var societyService = (ISocietyService)Bootstrapper.GetService(typeof(SocietyService));
                    var societyName = societyService.Get(w => w.Id == societyId).Select(w => w.SocietyName);

                    throw new Exception("This is not a valid Member Id for " + societyName + ", Please contact society support for further assistance");
                }
                return html[0].Message;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public HttpResponseMessage EditProfile(UserViewModel viewModel)
        {
            var response = new ResponseModel();
            try
            {
                if (viewModel == null)
                    throw new Exception("Bad Request");

                var user = _walletBaseService.GetUser(viewModel.UserName, _identityManager);

                var request = new RequestModel
                {
                    DeviceId = viewModel.DeviceId
                };

                // Validate Device ID
                _walletBaseService.ValidateDeviceId(request, user);

                response.Status = true;
                response.Result = StatusConstants.Success;

                user.FirstName = viewModel.FirstName;
                user.LastName = viewModel.LastName;
                user.IsSocietyMember = viewModel.IsSocietyMember;

                if (user.Email != viewModel.Email)
                {
                    user.Email = viewModel.Email;
                    user.EmailConfirmed = false;

                    // Send Email
                    var emailBody = string.Format("Dear Customer, <br><br><br> Please click on the link below to confirm the email id : <br><br> <a href = \"" + FieldConstants.LiveEmailUrl + "\"> Confirm My Email Id </a><br><br>Thank You,<br>Jugaad Team", System.Web.HttpUtility.UrlEncode(viewModel.UserName));
                    _emailService.SendEmailAsync(user.Email, "Email Confirmation", emailBody);

                    var smsBody = "Dear Customer, a link has been sent on your emailId registered with JUGAD ( " + user.Email + " ) for Email-Verification";
                    TboBaseController.SendSms(user.UserName, smsBody, "Email Verification");

                    response.Result = "Verify Email";
                }
                if (viewModel.IsSocietyMember)
                {
                    if (CheckIfValidSocietyMember(viewModel.SocietyId, viewModel.MemberId).ToUpper() != StatusConstants.Success.ToUpper())
                        throw new Exception("This is not a valid Member Id for society, Please contact society support for further assistance");
                    else
                    {
                        user.SocietyId = viewModel.SocietyId;
                        user.MemberId = viewModel.MemberId;
                    }
                }
                else
                {
                    user.SocietyId = 0;
                    user.MemberId = 0;
                }

                _identityManager.UpdateUser(user);

                //var _prePaidCardService = (IPrePaidCardService)Bootstrapper.GetService(typeof(PrePaidCardService));
                //_prePaidCardService.UpdateCustomerDetails(viewModel);
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Result = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        public string Encrypt(string number)
        {
            var _walletBaseService = (IWalletBaseService)Bootstrapper.GetService(typeof(WalletBaseService));
            return _walletBaseService.Encrypt(number, FieldConstants.LoginIv);
        }

        public HttpResponseMessage MobileVerificationOtp(UserViewModel viewModel)
        {
            var result = "OTP resend Successful";
            var bStatus = true;
            var otp = string.Empty;
            try
            {
                if (null == viewModel)
                    throw new Exception("Bad Request");

                var user = _walletBaseService.GetUser(viewModel.UserName, _identityManager); //_identityManager.GetUserFromUserName(viewModel.UserName));
                var message = "%3C%23%3E Your Jugaad verification code is @otp. Message ID: "+ ServiceConstants.SMSProd;
                if (null == user)
                {
                    viewModel.UserName = _walletBaseService.Decrypt(viewModel.UserName, FieldConstants.LoginIv);

                    // Generate Otp
                    otp = GenerateOtp();

                    // Send otp by sms
                    message = message.Replace("@otp", otp);
                    TboBaseController.SendOtp(viewModel.UserName, message, otp, null);
                    result = "User does not exists!";
                    //throw new Exception("User does not exists!");
                }
                else
                {
                    // Generate & Send OTP
                    //const string message = "Your OTP  is @otp. Please, don't disclose it to anybody.";
                    GenerateAndSendOtp(user, message);
                    otp = user.Otp;
                }
            }
            catch (Exception exception)
            {
                result = LogHandler.LogError(exception).Message;
                bStatus = false;
            }

            var returnViewModel = new ResponseModel
            {
                Status = bStatus,
                Result = result,
                WalletBalance = 0
                //Otp = otp
            };
            return Request.CreateResponse(HttpStatusCode.OK, returnViewModel);
        }

    }
}