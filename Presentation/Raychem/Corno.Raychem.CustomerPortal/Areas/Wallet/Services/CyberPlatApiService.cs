using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces;
using System.Web;
using Corno.Data.Helpers;
using Corno.Globals.Constants;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Constants;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Services
{
    public class CyberPlatApiService : ICyberPlatApiService
    {
        #region -- Constructors --

        public CyberPlatApiService(ICyberPlatUrlService cyberPlatUrlService, ICyberPlatErrorService cyberPlatErrorService,
            ILogService logService, IWalletBaseService walletBaseService)
        {
            _cyberPlatUrlService = cyberPlatUrlService;
            _cyberPlatErrorService = cyberPlatErrorService;
            _logService = logService;
            _walletBaseService = walletBaseService;
        }
        #endregion

        #region -- Data Members --

        private readonly ICyberPlatUrlService _cyberPlatUrlService;
        private static ICyberPlatErrorService _cyberPlatErrorService;
        private readonly ILogService _logService;
        private readonly IWalletBaseService _walletBaseService;


        private const string PassPhrase = "dtpl12345#@"; //4ever@123
        private const string SdCode = "332185";
        private const string ApCode = "332435";
        private const string OpCode = "332436";

        #endregion

        #region -- Methods --

        public PrivKey GetSecretKey()
        {
            PrivKey secretKey;
            var path = HttpContext.Current.Server.MapPath("~/Areas/Wallet/Keys/") + "secret.key";
            try
            {
                secretKey = Priv.OpenSecretKey(path, PassPhrase);
            }
            catch (PrivException exception)
            {
                throw new Exception(exception + " and Code is :" + exception.code + ". Open secret key error.");
            }
            return secretKey;
        }

        private string CreateSession()
        {
            // Create session
            //var dt = DateTime.Parse(DateTime.Now.ToString(CultureInfo.InvariantCulture));
            return DateTime.Now.ToString("ddmmyyyyHHmmss");
        }

        private string GetUrl(string service, string vOperator, string urlType)
        {
            var url = _cyberPlatUrlService.Get(c => c.Service == service && c.Operator == vOperator).FirstOrDefault();
            if (null == url)
                throw new Exception("Validation Url for service ( " + service + " ) and operator ( " + vOperator + " ) is not available in the system.");

            switch (urlType)
            {
                case FieldConstants.Verification:
                    return url.Verification;
                case FieldConstants.Payment:
                    return url.Payment;
                case FieldConstants.StatusCheck:
                    return url.StatusCheck;
            }

            throw new Exception("Invalid Url Type");
        }

        //public double GetCommission(string service, string vOperator, string userType)
        //{
        //    var url = _cyberPlatUrlService.Get(c => c.Service == service && c.Operator == vOperator).FirstOrDefault();
        //    if (null == url)
        //        throw new Exception("Validation Url for service ( " + service + " ) and operator ( " + vOperator + " ) is not available in the system.");

        //    switch (userType)
        //    {
        //        case "Customer":
        //            return url.CustomerCommission ?? 0;
        //        case "Merchant":
        //            return url.MerchantCommission ?? 0;
        //    }

        //    throw new Exception("Invalid user type for Commission.");
        //}

        private static void ParseResponse(string response, RequestModel viewModel)
        {
            var errorCode = response.Between("ERROR=", Environment.NewLine);
            var result = response.Between("RESULT=", Environment.NewLine);

            if (errorCode == "0" || result == "0") return;

            var error = _cyberPlatErrorService.Get(c => c.Code == errorCode).FirstOrDefault();
            switch (viewModel.Service)
            {
                case ServiceConstants.Dmt:
                    error = _cyberPlatErrorService.Get(c => c.Service == viewModel.Service &&
                                                            c.Type == viewModel.Type && c.Code == errorCode).FirstOrDefault();
                    break;
            }

            if (error == null)
                throw new Exception("Invalid Error Code : " + errorCode);
            throw new Exception(error.SimpleDescription, new Exception(response));
        }

        private string MakeRequest(RequestModel viewModel, string url, string session)
        {
            // Initialize
            Priv.Initialize();

            var request = new StringBuilder();
            var secretKey = GetSecretKey();

            request.Append("SD=" + SdCode + Environment.NewLine);
            request.Append("AP=" + ApCode + Environment.NewLine);
            request.Append("OP=" + OpCode + Environment.NewLine);
            request.Append("SESSION=" + session + Environment.NewLine);
            request.Append("NUMBER=" + viewModel.Number + Environment.NewLine);
            request.Append("ACCOUNT=" + viewModel.Account + Environment.NewLine);
            request.Append("Authenticator3=" + viewModel.Authenticator3 + Environment.NewLine);
            request.Append("Authenticator4=" + viewModel.Authenticator4 + Environment.NewLine);
            request.Append("AMOUNT=" + viewModel.Amount + Environment.NewLine);
            request.Append("AMOUNT_ALL=" + viewModel.AmountAll + Environment.NewLine);
            request.Append("TERM_ID=" + viewModel.TermId + Environment.NewLine);
            request.Append("COMMENT=" + viewModel.Comment + Environment.NewLine);
            request.Append("AID=" + viewModel.AgentId + Environment.NewLine);

            request.Append("routingType=" + viewModel.RoutingType + Environment.NewLine);
            request.Append("transRefId=" + viewModel.TransRefId + Environment.NewLine);

            request.Append("otc=" + viewModel.OneTimePassword + Environment.NewLine);
            request.Append("otcRefCode=" + viewModel.OneTimePasswordRefCode + Environment.NewLine);

            request.Append("fName=" + viewModel.FirstName + Environment.NewLine);
            request.Append("lName=" + viewModel.LastName + Environment.NewLine);
            request.Append("mothersMaidenName=" + viewModel.MothersMaidenName + Environment.NewLine);
            request.Append("address=" + viewModel.Address + Environment.NewLine);
            request.Append("birthday=" + viewModel.Account + Environment.NewLine);
            request.Append("state=" + viewModel.State + Environment.NewLine);
            request.Append("gender=" + viewModel.Gender + Environment.NewLine);
            request.Append("Pin=" + viewModel.Pin + Environment.NewLine);
            request.Append("remId=" + viewModel.RemId + Environment.NewLine);

            request.Append("benAccount=" + viewModel.BeneficiaryAccount + Environment.NewLine);
            request.Append("benMobile=" + viewModel.BeneficiaryMobile + Environment.NewLine);
            request.Append("benIFSC=" + viewModel.BeneficiaryIfsc + Environment.NewLine);
            request.Append("benName=" + viewModel.BeneficiaryName + Environment.NewLine);
            request.Append("benCode=" + viewModel.BeneficiaryCode + Environment.NewLine);
            request.Append("benBankName=" + viewModel.BenBankName + Environment.NewLine);
            //request.Append("benMobile=" + viewModel.BenMobile + Environment.NewLine);
            request.Append("benId=" + viewModel.BenId + Environment.NewLine);

            request.Append("Type=" + viewModel.Type + Environment.NewLine);

            request.Append("Category_id=" + viewModel.CategoryId + Environment.NewLine);
            request.Append("Product_id=" + viewModel.ProductId + Environment.NewLine);
            request.Append("billing_email=" + viewModel.BillingEmail + Environment.NewLine);
            request.Append("benfName=" + viewModel.BeneficiaryFirstName + Environment.NewLine);
            request.Append("benlName=" + viewModel.BeneficiaryLastName + Environment.NewLine);
            request.Append("Email=" + viewModel.Email + Environment.NewLine);
            request.Append("price=" + viewModel.Price + Environment.NewLine);
            request.Append("qty=" + viewModel.Quantity + Environment.NewLine);
            request.Append("giftmessage=" + viewModel.GiftMessage + Environment.NewLine);
            request.Append("theme=" + viewModel.Theme + Environment.NewLine);
            request.Append("Producttype=" + viewModel.ProductType + Environment.NewLine);
            request.Append("AgentTransId=" + viewModel.AgentTransId + Environment.NewLine);

            request.Append("REQ_TYPE=" + viewModel.Type + Environment.NewLine);
            request.Append("PlanOffer=" + viewModel.PlanOffer + Environment.NewLine);
            request.Append("KeyKycStatus=" + viewModel.KeyKycStatus + Environment.NewLine);
            request.Append("Rcode=" + viewModel.Rcode + Environment.NewLine);
            request.Append("RequestFor=" + viewModel.RequestFor + Environment.NewLine);
            request.Append("originalmerchantTranId=" + viewModel.CyberPlatTransId + Environment.NewLine);

            request.Append("transFrom=" + viewModel.FromDate + Environment.NewLine);
            request.Append("transTo=" + viewModel.ToDate + Environment.NewLine);

            var httpWReq = (HttpWebRequest) WebRequest.Create(url);

            var encoding = new ASCIIEncoding();

            var message = "inputmessage=" + HttpUtility.UrlEncode(secretKey.SignText(request.ToString()));
            var data = encoding.GetBytes(message);
            httpWReq.Method = "POST";
            httpWReq.ContentType = "application/x-www-form-urlencoded";
            httpWReq.ContentLength = data.Length;
            using (var stream = httpWReq.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse) httpWReq.GetResponse();
            {
                var stream = response.GetResponseStream();

                if (null == stream) return string.Empty;

                var reader = new StreamReader(stream);
                var htmlText = reader.ReadToEnd();

                Priv.Done();

                // Decode the response.
                htmlText = HttpUtility.UrlDecode(htmlText);

                // Write Log
                var type = viewModel.Type;
                if (string.IsNullOrEmpty(type))
                    type = viewModel.Service;
                _logService.GenerateLog(viewModel.Service, url, type, request.ToString(), htmlText, viewModel.UserName);

                ParseResponse(htmlText, viewModel);

                return htmlText;
            }
        }

        public string GetBalance()
        {
            var url = "https://in.cyberplat.com/cgi-bin/mts_espp/mtspay_rest.cgi";
            var session = CreateSession();
            return MakeRequest(new RequestModel(), url, session);
        }

        public string ValidationCheck(RequestModel viewModel)
        {
            var session = CreateSession();
            var url = GetUrl(viewModel.Service, viewModel.Operator, FieldConstants.Verification);
            return MakeRequest(viewModel, url, session);
        }

        public string Payment(RequestModel requestModel)
        {
            string session;
            switch (requestModel.Service)
            {
                case ServiceConstants.Dmt:
                case ServiceConstants.GiftCard:
                    session = CreateSession();
                    var validationUrl = GetUrl(requestModel.Service, requestModel.Operator, FieldConstants.Verification);
                    MakeRequest(requestModel, validationUrl, session);
                    break;
                default:
                    // Decrpypt session Id
                    session = _walletBaseService.Decrypt(requestModel.SessionId, FieldConstants.SessionIv);
                    session = session.Substring(10);
                    break;
            }

            var paymentUrl = GetUrl(requestModel.Service, requestModel.Operator, FieldConstants.Payment);
            return MakeRequest(requestModel, paymentUrl, session);
        }

        public string StatusCheck(RequestModel viewModel)
        {
            var session = CreateSession();
            var url = GetUrl(viewModel.Service, viewModel.Operator, FieldConstants.StatusCheck);
            return MakeRequest(viewModel, url, session);
        }

        public string GetCommissionOperator(string service, string vOperator)
        {
            var url = _cyberPlatUrlService.Get(c => c.Service == service && c.Operator == vOperator).FirstOrDefault();
            if (null == url)
                return string.Empty;

            return url.OperatorComissionName;
        }
        #endregion
    }
}