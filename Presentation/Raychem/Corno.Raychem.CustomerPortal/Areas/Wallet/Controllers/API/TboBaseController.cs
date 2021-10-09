using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web.Http;
using Corno.Data.Helpers;
using Corno.Logger;
using Corno.Raychem.CustomerPortal.Areas.Admin.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Constants;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces;
using Corno.Services.Bootstrapper;
using Corno.Services.Http;
using Corno.Services.Http.Interfaces;
using Corno.Services.SMS;
using Corno.Services.SMS.Interfaces;
using Newtonsoft.Json;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Controllers
{
    public class TboBaseController : ApiController
    {
        #region -- Constructors --

        public TboBaseController(IdentityManager identityManager)
        {
            IdentityManager = identityManager;
        }
        #endregion

        #region -- Data Members --
        protected readonly IdentityManager IdentityManager;
        #endregion

        #region -- Methods --

        public static string SendSms(string mobileNo, string message, string transactionType)
        {
            var smsZoneSmsService = (ISmsOzoneSmsService)Bootstrapper.GetService(typeof(SmsOzoneSmsService));
            var smsResult = smsZoneSmsService.SendSmsOzone(mobileNo, message);
            //smsZoneSmsService.SaveSmsLog(mobileNo, message, smsResult, DateTime.Now, transactionType, 0);

            return smsResult;
        }

        public static string SendOtp(string mobileNo, string message, string otp, string transactionType)
        {
            var smsZoneSmsService = (ISmsOzoneSmsService)Bootstrapper.GetService(typeof(SmsOzoneSmsService));
            var smsResult = smsZoneSmsService.SendOtp(mobileNo, message, otp);
            //smsZoneSmsService.SaveSmsLog(mobileNo, message, smsResult, DateTime.Now, transactionType, 0);

            return smsResult;
        }

        public bool ValidateAppVersion(dynamic obj)
        {
            if (obj["versioncode"] == null)
                return false;

            var versionCode = obj["versioncode"].ToString();
            return ConversionHelper.ToInt(versionCode) >= 1024;
        }

        public ApplicationUser ValidateUser(dynamic obj)
        {
            var userName = obj["username"].ToString();

            var walletBaseService = (IWalletBaseService)Bootstrapper.GetService(typeof(WalletService));
            var user = walletBaseService.GetUser(userName, IdentityManager);
            if (null == user)
                throw new Exception("Account does not exist for " + userName + " in the system.");
            if (user.LockoutEnabled)
                throw new Exception("User is locked");
            return user;
        }

        public async Task<dynamic> GetRequestJsonObject(HttpRequestMessage request)
        {
            var jsonString = await request.Content.ReadAsStringAsync();
            dynamic parsedObject = JsonConvert.DeserializeObject(jsonString);
            return parsedObject;
        }

        public async Task<HttpResponseMessage> FetchCities(HttpRequestMessage request, string tboType)
        {
            var status = string.Empty;
            var bStatus = true;
            try
            {
                dynamic parsedObject = await GetRequestJsonObject(request);
                ValidateUser(parsedObject);

                string query = parsedObject["query"].ToString().Trim();
                var isDomestic = true;
                if (null != parsedObject["IsDomestic"])
                    isDomestic = ConversionHelper.ToBoolean(parsedObject["IsDomestic"].ToString());

                //const string countryName = "India";
                const string countryCode = "IN";
                switch (tboType)
                {
                    case ServiceConstants.Airline:
                        // Get Airports List

                        var airportService = (IAirportService)Bootstrapper.GetService(typeof(AirportService));
                        var airports = airportService.Get(a => a.CityName.ToLower().Contains(query.ToLower()))
                            .Select(a => new
                            {
                                a.AirportName,
                                a.AirportCode,
                                a.CityName,
                                a.CityCode,
                                a.CountryName,
                                a.CountryCode,
                                a.Nationalty,
                                a.Currency
                            });
                        airports = isDomestic ? airports.Where(a => a.CountryCode.ToLower().Trim() == countryCode.ToLower())
                            : airports;

                        return Request.CreateResponse(HttpStatusCode.OK, airports);
                    case ServiceConstants.Bus:
                        // Get Bus City List
                        var busCityService = (IBusCityService)Bootstrapper.GetService(typeof(BusCityService));
                        var busCities = busCityService.Get(a => a.Name.ToLower().Contains(query.ToLower()))
                            .Select(a => new
                            {
                                Id = a.Code,
                                a.Name,
                                a.CountryCode,
                            });
                        return Request.CreateResponse(HttpStatusCode.OK, busCities);
                    case ServiceConstants.Hotel:
                        if (query.Length < 3)
                            return Request.CreateResponse(HttpStatusCode.OK, "{}");

                        // Get Hotel cities List
                        var hotelService = (IHotelCityService)Bootstrapper.GetService(typeof(HotelCityService));
                        var hotelCities = hotelService.Get(a => a.Name.ToLower().Contains(query.ToLower()))
                            .Select(a => new
                            {
                                Id = a.Code,
                                a.Name,
                                a.CountryName,
                                a.CountryCode,
                            });

                        //hotelCities = isDomestic == true ? hotelCities.Where(a => a.CountryCode.ToLower().Trim() == countryCode.ToLower())
                        //    : hotelCities.Where(a => a.CountryCode.ToLower().Trim() != countryCode.ToLower());
                        return Request.CreateResponse(HttpStatusCode.OK, hotelCities);
                }
            }
            catch (Exception exception)
            {
                status = exception.Message;
            }

            var responseModel = new ResponseModel
            {
                Status = false,
                Result = status,
                WalletBalance = 0,
                Response = null
            };

            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        //public static object GetResponse(string requestData, string url)
        //{
        //    object jsonObject;
        //    try
        //    {
        //        var data = Encoding.UTF8.GetBytes(requestData);

        //        var request = (HttpWebRequest) WebRequest.Create(url);
        //        request.Method = "POST";
        //        request.ContentType = "application/json";
        //        request.Headers.Add("Accept-Encoding", "gzip");

        //        var dataStream = request.GetRequestStream();
        //        dataStream.Write(data, 0, data.Length);
        //        dataStream.Close();

        //        var webResponse = request.GetResponse();
        //        var stream = webResponse.GetResponseStream();
        //        if (stream == null) return string.Empty;

        //        using (var streamReader = new StreamReader(new GZipStream(stream, CompressionMode.Decompress)))
        //        {
        //            var responseText = streamReader.ReadToEnd();
        //            jsonObject = JsonConvert.DeserializeObject(responseText);
        //        }
        //    }
        //    catch (WebException webEx)
        //    {
        //        //get the response stream
        //        var response = webEx.Response;
        //        var stream = response.GetResponseStream();
        //        jsonObject = stream == null ? string.Empty : new StreamReader(stream).ReadToEnd();
        //    }

        //    return jsonObject;
        //}

        public async Task<ResponseModel> GetTboObject(HttpRequestMessage request, string url, string rawRequest = null)
        {
            var status = "Success";
            var bStatus = true;
            double? walletAmount = 0;
            object jsonResponseObj = null;
            try
            {
                dynamic parsedObject = await GetRequestJsonObject(request);
                ValidateUser(parsedObject);

                if (null == rawRequest)
                {
                    rawRequest = parsedObject["request"].ToString();
                    rawRequest = rawRequest?.Replace("\r\n", "");
                }

                var httpService = (IHttpService)Bootstrapper.GetService(typeof(HttpService));
                jsonResponseObj = httpService.JsonPost(rawRequest, url);
            }
            catch (Exception exception)
            {
                status = exception.Message;
                bStatus = false;
            }

            var responseModel = new ResponseModel
            {
                Status = bStatus,
                Result = status,
                WalletBalance = 0,
                Response = jsonResponseObj
            };

            return responseModel;
        }

        public async Task<HttpResponseMessage> TboRequset(HttpRequestMessage request, string url, string rawRequest = null)
        {
            var responseModel = await GetTboObject(request, url, rawRequest);
            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        public string SaveTransaction(string tboType, double amount,
            string pnr, string bookingId, string source, string destination, string tboOperator,
            HttpRequestMessage request, string response)
        {
            try
            {
                dynamic parsedRequest = JsonConvert.DeserializeObject(request.Content.ReadAsStringAsync().Result);
                var actualRequest = parsedRequest["request"].ToString();
                int transactionId = ConversionHelper.ToInt(parsedRequest["transactionId"].ToString());

                LogHandler.LogInfo("TransactionId : " + transactionId, LogHandler.LogType.General);

                var walletTransactionService =
                    (IWalletTransactionService)Bootstrapper.GetService(typeof(WalletTransactionService));
                var transaction = walletTransactionService.GetById(transactionId);
                if (null == transaction) return transactionId.ToString();

                transaction.Service = tboType;
                transaction.Pnr = pnr;
                transaction.BookingId = bookingId;
                transaction.Source = source;
                transaction.Destination = destination;
                transaction.Operator = tboOperator;
                transaction.Request = actualRequest;
                transaction.Response = response;
                transaction.Status = "Successfull";

                walletTransactionService.Update(transaction);
                walletTransactionService.Save();

                return transactionId.ToString();
            }
            catch (Exception exception)
            {
                LogHandler.LogError(exception);
            }

            return null;
        }

        public WalletTransaction GetTransactionById(string Id)
        {
            var transId = int.Parse(Id);
            var walletTransactionService = (IWalletTransactionService)Bootstrapper.GetService(typeof(WalletTransactionService));
            return walletTransactionService.Get(x => x.Id == transId).ToList().FirstOrDefault();
        }

        public void SaveUpdatedTransaction(WalletTransaction transaction)
        {
            var walletTransactionService = (IWalletTransactionService)Bootstrapper.GetService(typeof(WalletTransactionService));
            walletTransactionService.Update(transaction);
            walletTransactionService.Save();
        }
    #endregion
}
}