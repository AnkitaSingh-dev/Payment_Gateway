using System;
using System.Linq;
using System.Web.Http;
using Corno.Services.Encryption;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces;
using static Corno.Raychem.CustomerPortal.Areas.Wallet.Models.PrePaidCardModel;
using Corno.Raychem.CustomerPortal.Areas.Admin.Models;
using Corno.Globals.Constants;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Constants;
using System.Web.Script.Serialization;
using Corno.Services.Bootstrapper;
using System.Web;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Controllers.API
{
    public class aggegratorController : ApiController
    {
        #region -- Constructors --

        public aggegratorController(IWalletBaseService walletBaseService, IWalletService walletService, IdentityManager identityManager,
            ILogService logService, ISafexWalletTransactionService safexWalletTransactionService,
            IJugadUserSafexUserService jugadUserSafexUserService, IPrePaidCardService prePaidCardService)
        {
            _safexWalletTransactionService = safexWalletTransactionService;
            _identityManager = identityManager;
            _jugadUserSafexUserService = jugadUserSafexUserService;
            _walletBaseService = walletBaseService;
            _walletService = walletService;
            _logService = logService;
            _prePaidCardService = prePaidCardService;
        }
        #endregion

        #region -- Data Members --
        private readonly ISafexWalletTransactionService _safexWalletTransactionService;
        private readonly IdentityManager _identityManager;
        private readonly IJugadUserSafexUserService _jugadUserSafexUserService;
        private readonly IWalletBaseService _walletBaseService;
        private readonly IWalletService _walletService;
        private readonly ILogService _logService;
        private readonly IPrePaidCardService _prePaidCardService;
        #endregion

        #region -- Methods --

        //public cardRequestModel prepaidtxn([FromBody]cardRequestModel request)
        //{
        //    var responseCode = "0000";
        //    var message = "Transaction Successfull";
        //    var txnId = 0;
        //    var walletTxnId = string.Empty;
        //    var result = new cardRequestModel();

        //    try
        //    {
        //        var response = _prePaidCardService.DecryptResponse(request.payload, "AES");

        //        var responseModel = response; //JsonConvert.DeserializeObject<cardRequestModel>(response);

        //        var safexUser = _jugadUserSafexUserService.Get(x => x.CustId == responseModel.cardTransactionReflection.userId).FirstOrDefault();

        //        if (safexUser == null)
        //        {
        //            responseCode = "0001";
        //            throw new Exception("User doesnot exist");
        //        }

        //        var userName = _walletBaseService.Encrypt(safexUser.UserName, FieldConstants.LoginIv);

        //        // Validate service model
        //        var user = _walletBaseService.GetUser(userName, _identityManager);

        //        if (user.LockoutEnabled)
        //        {
        //            responseCode = "0002";
        //            throw new Exception("Account Locked");
        //        }

        //        if (user.Wallet < responseModel.cardTransactionReflection.amount)
        //        {
        //            responseCode = "0005";
        //            throw new Exception("Insuffcient Balance in user account");
        //        }

        //        //if (responseModel.cardTransactionReflection.safexTxnType == "PREPAIDTXN")
        //        //{
        //        //    var debitRequest = new RequestModel()
        //        //    {
        //        //        Service = ServiceConstants.CardOperations,
        //        //        Operator = "SafexPay",
        //        //        Agent = _walletService.Encrypt("19099950044", FieldConstants.Iv),
        //        //        DeviceId = user.DeviceId,
        //        //        UserName = _walletBaseService.Encrypt(user.UserName, FieldConstants.LoginIv),
        //        //        Amount = responseModel.cardTransactionReflection.amount,
        //        //        PaymentMode = PaymentMode.Wallet,
        //        //        BookingId = responseModel.cardTransactionReflection.switchMerchName

        //        //    };
        //        //    var walletReturnInfo = _walletService.Debit(debitRequest);
        //        //    if (!walletReturnInfo.Status)
        //        //    {
        //        //        throw new Exception(walletReturnInfo.Result);
        //        //    }
        //        //    walletTxnId = walletReturnInfo.TransactionId;
        //        //}

        //        //else if (responseModel.cardTransactionReflection.safexTxnType == "PREPAIDREV")
        //        //{
        //        //    var creditRequest = new RequestModel()
        //        //    {
        //        //        Service = ServiceConstants.CardOperations,
        //        //        Operator = "SafexPay",
        //        //        Agent = _walletService.Encrypt("19099950044", FieldConstants.Iv),
        //        //        DeviceId = user.DeviceId,
        //        //        UserName = _walletBaseService.Encrypt(user.UserName, FieldConstants.LoginIv),
        //        //        Amount = responseModel.cardTransactionReflection.amount,
        //        //        PaymentMode = PaymentMode.Wallet
        //        //    };
        //        //    var walletReturnInfo = _walletService.Credit(creditRequest);
        //        //    if (!walletReturnInfo.Status)
        //        //    {
        //        //        throw new Exception(walletReturnInfo.Result);
        //        //    }
        //        //    walletTxnId = walletReturnInfo.TransactionId;
        //        //}

        //        //txnId = _safexWalletTransactionService.AddSafexWalletTransaction(responseModel.cardTransactionReflection, user, walletTxnId);

        //        Task t = Task.Delay(2500);
        //        var statusResult = _prePaidCardService.CardTransactionStatus(txnId);
        //        if (!statusResult)
        //        {
        //            responseCode = "0004";
        //            throw new Exception("Status capture failed");
        //        }

        //    }
        //    catch (Exception exception)
        //    {
        //        responseCode = responseCode == "0000" ? "0008" : responseCode;
        //        message = exception.Message;
        //    }

        //    var safexResponse = new CardResponse
        //    {
        //        response = new response()
        //        {
        //            code = responseCode,
        //            description = message
        //        },
        //        cardTransactionReflection = new cardTransactionReflection()
        //        {
        //            partnerRefId = txnId.ToString()
        //        }
        //    };

        //    var payload = _prePaidCardService.GetToken(new JavaScriptSerializer().Serialize(safexResponse));

        //    var aesServices = (IMyCryptoClass)Bootstrapper.GetService(typeof(MyCryptoClass));
        //    payload = aesServices.encrypt(payload);

        //    result = new cardRequestModel()
        //    {
        //        agId = PrePaidConstants.AggregatorId,
        //        meId = PrePaidConstants.AggregatorId,
        //        payload = payload,
        //        encType = "AES"
        //    };

        //   // JsonConvert.SerializeObject(result,new JsonSerializerSettings{ContractResolver = new CamelCasePropertyNamesContractResolver()});

        //    _logService.GenerateLog(ServiceConstants.CardOperations, "Card Transaction Capture", "prepaidtxn", new JavaScriptSerializer().Serialize(safexResponse), payload);

        //    return result;
        //}
        #endregion

    }
}
