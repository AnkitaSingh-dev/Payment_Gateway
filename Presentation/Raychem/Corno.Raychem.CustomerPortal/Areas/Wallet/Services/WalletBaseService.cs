using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using CCA.Util;
using Corno.Globals.Constants;
using Corno.Raychem.CustomerPortal.Areas.Admin.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Constants;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces;
using Corno.Services.Bootstrapper;
using Corno.Services.Encryption;
using Corno.Services.Encryption.Interfaces;
using Corno.Services.Http;
using Corno.Services.Http.Interfaces;
using Newtonsoft.Json;
using RestSharp;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Services
{
    public class WalletBaseService : IWalletBaseService
    {
        #region -- Constructors --
        public WalletBaseService(IdentityManager identityManager)
        {
            _identityManager = identityManager;
        }
        #endregion

        #region -- Data Members --
        private readonly IdentityManager _identityManager;
        #endregion

        #region -- Methods --
        public ApplicationUser ValidateServiceViewModel(RequestModel requestModel)
        {
            //throw new Exception("User Agent : " + HttpContext.Current.Request.UserAgent);

            if (null == requestModel)
                throw new Exception("Bad Request");

             //Validate User Agent
                 ValidateUserAgent(requestModel.Agent);

            switch (requestModel.Service)
            {
                case ServiceConstants.Dmt:
                    if (requestModel.Type == "3")
                    {
                        if (requestModel.Amount <= 0)
                            throw new Exception("This is an invalid Amount. Kindly enter a valid Amount.");
                    }
                    break;
                default:
                    if (requestModel.Amount <= 0)
                        throw new Exception("This is an invalid Amount. Kindly enter a valid Amount.");
                    break;
            }

            var _walletService = (IWalletService)Bootstrapper.GetService(typeof(WalletService));
            if (requestModel.PaymentMode != PaymentMode.Web)
            {
                var limitResponse = _walletService.CheckLimit(requestModel);

                if (!limitResponse.Status)
                    throw new Exception(limitResponse.Result);
            }

            var user = GetUser(requestModel.UserName, _identityManager); //_identityManager.GetUserFromUserName(requestModel.UserName);
            if (null == user)
                throw new Exception(requestModel.UserName + "'s Account does not exist!");

            if (user.PhoneNumberConfirmed == false || user.EmailConfirmed == false)
                throw new Exception("Please confirm your phone number and emailId to continue transaction");

           //if (user.IsAdhaarSubmit == false && requestModel.PaymentMode != PaymentMode.Web)
           //    throw new Exception("Please submit KYC to continue transactions");

            if (user.LockoutEnabled)
                throw new Exception("User is locked");
            requestModel.UserName = user.UserName;

            //Validate Device ID
            if (requestModel.PaymentMode != PaymentMode.Web)
                ValidateDeviceId(requestModel, user);


            switch (requestModel.Service)
            {
                case ServiceConstants.Dmt:
                    if (requestModel.Type == "3")
                        ValidatePaymentMode(requestModel, user);
                    break;
                case ServiceConstants.Card:
                    break;
                default:
                    ValidatePaymentMode(requestModel, user);
                    break;
            }

            return user;
        }

        public ApplicationUser GetUser(string cypherText, IdentityManager identityManager)
        {
            var aes256Service = (IAes256Service) Bootstrapper.GetService(typeof(Aes256Service));
            var key = aes256Service.GetHashSha256(FieldConstants.Ever4KeyText, 32); //32 bytes = 256 bits
            var userName = aes256Service.Decrypt(cypherText, key, FieldConstants.LoginIv);
            return identityManager.GetUserFromUserName(userName);
        }

        public string Decrypt(string cypherText, string initVector)
        {
            var aes256Service = (IAes256Service) Bootstrapper.GetService(typeof(Aes256Service));
            var key = aes256Service.GetHashSha256(FieldConstants.Ever4KeyText, 32); //32 bytes = 256 bits
            return aes256Service.Decrypt(cypherText, key, initVector);
        }

        public string Encrypt(string value, string initVector)
        {
            var aes256Service = (IAes256Service) Bootstrapper.GetService(typeof(Aes256Service));
            var key = aes256Service.GetHashSha256(FieldConstants.Ever4KeyText, 32); //32 bytes = 256 bits
            return aes256Service.Encrypt(value, key, initVector);
        }

        public void ValidateUserAgent(string agent)
        {
            var userAgent = Decrypt(agent, FieldConstants.Iv);
            //userAgent = "19099950044";
            //throw new Exception(userAgent);

            // Validate User Agent
            //throw new Exception(HttpContext.Current.Request.UserAgent);
            if (HttpContext.Current.Request.UserAgent != null &&
                (HttpContext.Current.Request.UserAgent.Contains("Android") ||
                HttpContext.Current.Request.UserAgent.Contains("android") ||
                HttpContext.Current.Request.UserAgent.Contains("okhttp")))
            {
                if (userAgent != "19088850044")
                    throw new Exception("This is an invalid Client !");
            }
            else
            {
                //if (HttpContext.Current.Request.UserAgent != null &&
                //    (HttpContext.Current.Request.UserAgent.Contains("Edge") ||
                //     HttpContext.Current.Request.UserAgent.Contains("edge")))
                //{
                if (userAgent != "19099950044")
                    throw new Exception("This is an invalid Client !");
                //}
                //else 
                //throw new Exception("Invalid Clientl");
            }
        }

        public void ValidateDeviceId(RequestModel requestModel, ApplicationUser user)
        {
            //var aes256Service = (IAes256Service) Bootstrapper.GetService(typeof(Aes256Service));
            try
            {
                //const string keyText = "8c3acb8b96b2a3b8a95b6a26d15029cf060444ec";

                //var plainText = "ccdefcdd";
                //var iv = aes256Service.GenerateRandomIv(16); //16 bytes = 128 bits
                //var key = aes256Service.GetHashSha256(keyText, 32); //32 bytes = 256 bits
                //var cypherText = aes256Service.Encrypt(plainText, key, iv);
                ////iv = aes256Service.GenerateRandomIv(16); //16 bytes = 128 bits
                //var decryptedText = aes256Service.Decrypt(cypherText, key, iv);
                //iv = aes256Service.GenerateRandomIv(16); //16 bytes = 128 bits

                //var key = aes256Service.GetHashSha256(FieldConstants.Ever4KeyText, 32); //32 bytes = 256 bits
                //var deviceId = aes256Service.Decrypt(requestModel.DeviceId, key, FieldConstants.Iv);
                var deviceId = Decrypt(requestModel.DeviceId, FieldConstants.Iv);
                var userDeviceId = Decrypt(user.DeviceId, FieldConstants.Iv);
                if (string.IsNullOrEmpty(userDeviceId) || userDeviceId != deviceId)
                    throw new Exception("The Transaction attempted is for a non-registered User.Make sure to choose a registered User.");
            }
            catch (Exception)
            {
                throw new Exception("Looks like your device is not verified. Please verify your device to continue.");
            }
        }

        private void ValidatePaymentMode(RequestModel viewModel, ApplicationUser user)
        {
            switch (viewModel.PaymentMode)
            {
                case PaymentMode.Wallet:
                case PaymentMode.Web:
                    if (viewModel.Service != ServiceConstants.Credit)
                        if (null == user.Wallet || user.Wallet <= 0 || user.Wallet < viewModel.Amount)
                            throw new Exception("Your account has Insufficient balance.");
                    break;
                case PaymentMode.Appnit:
                    ValidateAppnitPgTransId(viewModel);
                    break;
                case PaymentMode.Mpos:
                    ValidateMposTransId(viewModel);
                    break;
                case PaymentMode.KotakPG:
                    ValidateKotakTransId(viewModel);
                    break;
                default:
                    throw new Exception("Invalid Payment Mode : " + viewModel.PaymentMode);
            }
        }

        public void ValidateAppnitPgTransId(RequestModel viewModel)
        {
            var _walletBaseService = (IWalletBaseService)Bootstrapper.GetService(typeof(WalletBaseService));
            var user = _identityManager.GetUserFromUserName(viewModel.UserName);

            if (user.PhoneNumberConfirmed == false || user.EmailConfirmed == false)
                throw new Exception("Please confirm your phone number and emailId to continue transaction");

            //if (user.IsKYCSubmit == false)
            //    throw new Exception("Please submit KYC to continue transactions");

            var mid = Decrypt(viewModel.Mid, FieldConstants.Iv);
            var pgTxnId = Decrypt(viewModel.PgTxnId, FieldConstants.Iv);
            var orderId = Decrypt(viewModel.OrderId, FieldConstants.Iv);

            var uri = "https://secure.payplutus.in/AppnitPG/service/txn/getstatus?" +
                      "mid=" + mid +
                      "&pgTxnId=" + pgTxnId +
                      "&orderId=" + orderId;

            var httpService = (IHttpService) Bootstrapper.GetService(typeof(HttpService));
            var status = httpService.Get(uri);
            if (status != "SUCCESS")
                throw new Exception("Payment transaction id Validation Status : " + status);

            // Don't allow the repeat transaction Id.
            var walletTransactionService = (IWalletTransactionService) Bootstrapper.GetService(typeof(WalletTransactionService));

            var duplicates = walletTransactionService.Get(w => w.PaymentTransactionId == pgTxnId).ToList();
            if (duplicates.Count > 0)
                throw new Exception("Your payment transaction id is already consumed.");

            viewModel.PgTxnId = pgTxnId;
        }

        private void ValidateMposTransId(RequestModel viewModel)
        {
            Logger.LogHandler.LogInfo("------ START MPOS -----", Logger.LogHandler.LogType.Notify);

            Logger.LogHandler.LogInfo("User validation Success", Logger.LogHandler.LogType.Notify);

            var client = new RestClient("https://www.ezetap.com/api/2.0/txn/details");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", "{\"appKey\" : \"" + viewModel.AppKey + "\",\"txnId\": \"" + viewModel.PaymentTransactionId + "\",\"username\": \"" + viewModel.MposUserName + "\"}", ParameterType.RequestBody);

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            IRestResponse response = client.Execute(request);

            //var url = "https://www.ezetap.com/api/2.0/txn/details"; //"https://demo.ezetap.com/api/2.0/txn/details"
            //var request = "{" +
            //              "\"appKey\" : \"" + viewModel.AppKey + "\"," +
            //              "\"txnId\": \"" + viewModel.PaymentTransactionId + "\"," +
            //              "\"username\": \"" + viewModel.MposUserName + "\"" +
            //              "}";
            //JsonConvert.DeserializeObject<cardRequestModel>(response.Content);

            var jsonRequest = JsonConvert.DeserializeObject(response.Content);

            //var httpService = (IHttpService) Bootstrapper.GetService(typeof(HttpService));
            //dynamic jsonResponseObj = httpService.JsonPost(jsonRequest.ToString(), url);

            string str = response.Content.ToString().Replace("\r\n", "");
            str = str.Replace("\"", "'");

            Logger.LogHandler.LogInfo(str, Logger.LogHandler.LogType.Notify);

            if (str.Contains("'success': false"))
                throw new Exception("The MPOS transaction details do not match!");

            Logger.LogHandler.LogInfo("------ END MPOS -----", Logger.LogHandler.LogType.Notify);
        }

        public void ValidateNumber(RequestModel viewModel)
        {
            if (!string.IsNullOrEmpty(viewModel.Number)) return;

            switch (viewModel.Service)
            {
                case ServiceConstants.MobilePrepaid:
                case ServiceConstants.MobilePostpaid:
                    throw new Exception("This is an invalid Mobile number!");
                case ServiceConstants.Dth:
                case ServiceConstants.Gas:
                    throw new Exception("This is an Invalid Customer number!");
                case ServiceConstants.DataCardPrepaid:
                    throw new Exception("This is an Invalid Card number!");
                case ServiceConstants.Electricity:
                case ServiceConstants.Dmt:
                    throw new Exception("Invalid Consumer No.");
                case ServiceConstants.GiftCard:
                    break;
                    //default:
                    //throw new Exception("Service (" + viewModel.Service + ") doesn't support validation.");
            }
        }

        public void ValidateKotakTransId(RequestModel viewModel)
        {
            try
            {
                string accessCode = "AVBC74EK18AS92CBSA";//from avenues
                //string workingKey = "3557769DAEF31DE81DD079180CF70BE4";//put in the 32bit alpha numeric key in the quotes provided here
                string workingKey = "69632435AEC5279071255F6A66F52D28"; //Production Key
                var pgTxnId = Decrypt(viewModel.PgTxnId, FieldConstants.Iv);
                var orderId = Decrypt(viewModel.OrderId, FieldConstants.Iv);
                string orderStatusQueryJson = "{ \"reference_no\":\""+ pgTxnId + "\", \"order_no\":\""+ orderId + "\" }"; //Ex. { "reference_no":"CCAvenue_Reference_No" , "order_no":"123456"} 
                string encJson = "";
                Logger.LogHandler.LogInfo("Kotak URL", Logger.LogHandler.LogType.Notify);
                string queryUrl = "https://login.ccavenue.com/apis/servlet/DoWebTrans";
                Logger.LogHandler.LogInfo("Query params : " + pgTxnId +" " + orderId, Logger.LogHandler.LogType.Notify);
                CCACrypto ccaCrypto = new CCACrypto();
                encJson = ccaCrypto.Encrypt(orderStatusQueryJson, workingKey);

                // make query for the status of the order to ccAvenues change the command param as per your need
                string authQueryUrlParam = "enc_request=" + encJson + "&access_code=" + accessCode + "&command=orderStatusTracker&request_type=JSON&response_type=JSON";
                Logger.LogHandler.LogInfo("Query URL : " + queryUrl + "?" + authQueryUrlParam, Logger.LogHandler.LogType.Notify);
                // Url Connection
                String message = postPaymentRequestToGateway(queryUrl, authQueryUrlParam);
                Logger.LogHandler.LogInfo(message, Logger.LogHandler.LogType.Notify);
                //Response.Write(message);
                NameValueCollection param = getResponseMap(message);
                String status = "";
                String encResJson = "";
                if (param != null && param.Count == 2)
                {
                    for (int i = 0; i < param.Count; i++)
                    {
                        if ("status".Equals(param.Keys[i]))
                        {
                            status = param[i];
                        }
                        if ("enc_response".Equals(param.Keys[i]))
                        {
                            encResJson = param[i];
                            //Response.Write(encResXML);
                        }
                    }
                    if (!"".Equals(status) && status.Equals("0"))
                    {
                        String ResJson = ccaCrypto.Decrypt(encResJson, workingKey);
                        Logger.LogHandler.LogInfo(ResJson, Logger.LogHandler.LogType.Notify);
                        var pgTransaction = JsonConvert.DeserializeObject<Transaction>(ResJson);

                        var _pgTransactionService = (IPGTransactionService)Bootstrapper.GetService(typeof(PGTransactionService));

                        _pgTransactionService.Add(pgTransaction.Order_Status_Result);
                        _pgTransactionService.Save();
                    }
                    else if (!"".Equals(status) && status.Equals("1"))
                    {
                        Console.WriteLine("failure response from ccAvenues: " + encResJson);
                    }

                    // Don't allow the repeat transaction Id.
                    var walletTransactionService = (IWalletTransactionService)Bootstrapper.GetService(typeof(WalletTransactionService));

                    var duplicates = walletTransactionService.Get(w => w.OperatorTransId == pgTxnId).ToList();
                    if (duplicates.Count > 0)
                        throw new Exception("Transaction failed or cancelled.");

                    viewModel.PgTxnId = orderId.ToString();
                    viewModel.OperatorTransId = pgTxnId.ToString();

                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message);
            }
        }

        private string postPaymentRequestToGateway(String queryUrl, String urlParam)
        {

            String message = "";
            try
            {
                StreamWriter myWriter = null;// it will open a http connection with provided url
                WebRequest objRequest = WebRequest.Create(queryUrl);//send data using objxmlhttp object
                objRequest.Method = "POST";
                //objRequest.ContentLength = TranRequest.Length;
                objRequest.ContentType = "application/x-www-form-urlencoded";//to set content type
                myWriter = new System.IO.StreamWriter(objRequest.GetRequestStream());
                myWriter.Write(urlParam);//send data
                myWriter.Close();//closed the myWriter object

                // Getting Response
                System.Net.HttpWebResponse objResponse = (System.Net.HttpWebResponse)objRequest.GetResponse();//receive the responce from objxmlhttp object 
                using (System.IO.StreamReader sr = new System.IO.StreamReader(objResponse.GetResponseStream()))
                {
                    message = sr.ReadToEnd();
                    //Response.Write(message);
                }
            }
            catch (Exception exception)
            {
                Console.Write("Exception occured while connection." + exception);
            }
            return message;

        }

        private NameValueCollection getResponseMap(String message)
        {
            NameValueCollection Params = new NameValueCollection();
            if (message != null || !"".Equals(message))
            {
                string[] segments = message.Split('&');
                foreach (string seg in segments)
                {
                    string[] parts = seg.Split('=');
                    if (parts.Length > 0)
                    {
                        string Key = parts[0].Trim();
                        string Value = parts[1].Trim();
                        Params.Add(Key, Value);
                    }
                }
            }
            return Params;
        }
        #endregion
    }
}