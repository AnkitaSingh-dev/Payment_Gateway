using Corno.Data.Login;
using Corno.Globals.Constants;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Constants;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces;
using Corno.Services.Bootstrapper;
using Corno.Services.Encryption;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Script.Serialization;
using static Corno.Raychem.CustomerPortal.Areas.Wallet.Models.PrePaidCardModel;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Controllers.v2
{
    public class aggController : ApiController
    {
        #region -- Constructors --

        public aggController(IWalletBaseService walletBaseService, IWalletService walletService, IdentityManager identityManager,
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

        //public cardRequestModel notification([FromBody]cardRequestModel request)
        //{
        //    var responseCode = "0000";
        //    var message = "Transaction Successfull";
        //    var txnId = 0;
        //    var walletTxnId = string.Empty;
        //    var result = new cardRequestModel();

        //    try
        //    {
        //        var response = _prePaidCardService.DecryptResponse(request.payload, "AES");

        //      //  var responseModel = JsonConvert.DeserializeObject<cardRequestModel>(response);

        //      //  txnId = _safexWalletTransactionService.AddSafexWalletTransaction(responseModel.cardTransactionReflection, user, walletTxnId);

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

        //    // JsonConvert.SerializeObject(result,new JsonSerializerSettings{ContractResolver = new CamelCasePropertyNamesContractResolver()});

        //    _logService.GenerateLog(ServiceConstants.CardOperations, "Card Transaction Capture", "prepaidtxn", new JavaScriptSerializer().Serialize(safexResponse), payload);

        //    return result;
        //}
        #endregion
    }
}
