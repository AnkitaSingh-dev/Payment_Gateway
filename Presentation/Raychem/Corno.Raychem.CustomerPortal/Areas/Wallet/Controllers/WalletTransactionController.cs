using System;
using System.Web.Mvc;
using Corno.Globals.Constants;
using Corno.Raychem.CustomerPortal.Areas.Admin.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Constants;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces;
using Corno.Raychem.CustomerPortal.Controllers;
using Corno.Services.Bootstrapper;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Controllers
{
    public class WalletTransactionController : BaseController
    {
        #region -- Constructors -- 
        public WalletTransactionController(IWalletService walletService, IdentityManager identityManager)
        {
            _walletService = walletService;
            _identityManager = identityManager;
        }
        #endregion

        #region -- Data Members --
        private readonly IdentityManager _identityManager;
        private readonly IWalletService _walletService;
        #endregion

        #region -- Methods --


        #endregion

        // GET: /WalletTransaction/Create
        public ActionResult Create()
        {
            try
            {
                return View(new RequestModel());
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
            }
            return View();
        }

        // POST: /WalletTransaction/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RequestModel model, string submitType)
        {
            if (!ModelState.IsValid) return View(model);

            var _prePaidServices = (IFederalPrePaidCardService)Bootstrapper.GetService(typeof(FederalPrePaidCardService));
            try
            {
                ResponseModel returnInfo = null;
                var successString = string.Empty;
                
                model.PaymentMode = PaymentMode.Web;
                
                // Get User
                var user = _identityManager.GetUserFromUserName(model.UserName);
                if (null == user)
                    throw new Exception("Account does not exist for " + model.UserName + " in the system.");

                //var admin = _identityManager.GetUserFromUserName("8657393904");
                //if (admin.Otp != model.OneTimePassword)
                //    throw new Exception("Incorrect OTP");

                model.Number = model.UserName;
                model.UserName = _walletService.Encrypt(model.UserName, FieldConstants.LoginIv);
                model.DeviceId = user.DeviceId;
                model.Agent = _walletService.Encrypt("19099950044", FieldConstants.Iv);
                model.Operator = ServiceConstants.JugadAdmin;
               
               // model.CyberPlatTransId = "JW" + new Guid().ToString();
                switch (submitType)
                {
                    case "LoadWallet":
                        model.Service = ServiceConstants.Credit;
                        returnInfo = _walletService.Credit(model);
                       // _prePaidServices.ReflectTransaction(model);
                        successString = "Amount of Rs." + model.Amount + "/- credited to user (" + user.UserName + ") successfully, against " + model.BookingId;
                        break;
                    case "RevokeWallet":
                        model.Service = ServiceConstants.Debit;
                        returnInfo = _walletService.Debit(model);
                        //_prePaidServices.ReflectTransaction(model);
                        successString = "Amount of Rs." + model.Amount + "/- debited from user (" + user.UserName + ") successfully, against " + model.BookingId;
                        break;
                }
                if (null == returnInfo)
                    throw new Exception("The Transaction attempted does not have a Valid Response !");
                if(returnInfo.Status == false)
                    throw new Exception(returnInfo.Result);

                TempData["Success"] = successString;

                //var emailService = (IEmailService) Bootstrapper.GetService(typeof(EmailService));
                //var emailBody = "Dear Customer, <br><br><br>You received this message because you registered to our application. <br>Welcome to Jugad Pay community.";
                //emailService.SendEmailAsync("umesh.kale@concsystems.com", "Registration Confirmation", emailBody);
                // Send SMS
                var customerMessage = successString;//"Rs. " + model.Amount + " added successfully to your Jugad wallet.";
                TboBaseController.SendSms(model.Number, customerMessage, string.Empty);
                TboBaseController.SendSms("8657393904", TempData["Success"].ToString(), string.Empty);

                throw new Exception(TempData["Success"].ToString());

                return RedirectToAction("Create");
            }
            catch (Exception exception)
            {
                TempData["Success"] = null;
                HandleControllerException(exception);
            }

            model = null;
            return View(model);
        }

        [HttpPost]
        public ActionResult ResendOtp(UserViewModel viewModel)
        {
            var otp = string.Empty;
            try
            {
                if (null == viewModel)
                    throw new Exception("Bad Request");

                var _walletBaseService = (IWalletBaseService)Bootstrapper.GetService(typeof(WalletBaseService));

                var user = _walletBaseService.GetUser(viewModel.UserName, _identityManager);
                var message = "Your OTP  is @otp. Please, don't disclose it to anybody.";

                    GenerateAndSendOtp(user, message);
                    otp = user.Otp;

            }
            catch (Exception exception)
            {
                TempData["Success"] = null;
                HandleControllerException(exception);
            }
            ModelState.Clear();
            return View();
        }

        public string GenerateOtp()
        {
            // Generate 6 digit otp
            var generator = new Random();
            return generator.Next(0, 1000000).ToString("D6");
        }

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

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        //_sppointmentService.Dispose(disposing);
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
