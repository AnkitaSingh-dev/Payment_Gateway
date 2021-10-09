using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web.Http;
using Corno.Logger;
using Corno.Raychem.CustomerPortal.Areas.Admin.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Constants;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces;
using Corno.Services.Bootstrapper;
using Corno.Services.Email.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models.Hotel;
using System.IO;
using System.Text;
using System.Drawing.Printing;
using TuesPechkin;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Controllers.Test_Server
{
    public class TestHotelController : TboBaseController
    {
        #region -- Constructors --
        public TestHotelController(IdentityManager identityManager, IEmailService emailService,
            ILogService logService) : base(identityManager)
        {
            _emailService = emailService;
            _logService = logService;
        }
        #endregion

        #region -- Data Members --

        private readonly IEmailService _emailService;
        private readonly ILogService _logService;
        #endregion

        #region -- Methods --
        private async Task<ResponseModel> GetAuthenticationObject(HttpRequestMessage request)
        {
            const string url = TboConstants.TestHotelAuthenticateUrl  + "rest/Authenticate";
            const string rawRequest = "{\"ClientId\": " + TboConstants.TestClientId + ", \"UserName\": " + TboConstants.TestUserName
                + ", \"Password\": " + TboConstants.TestPassword + ", \"EndUserIp\": " + TboConstants.EndUserIp + " }";

            ResponseModel responseModel = new ResponseModel();

            try
            {
                responseModel = await GetTboObject(request, url, rawRequest);

                // Validate for version
                dynamic parsedObject = await GetRequestJsonObject(request);
                if (!ValidateAppVersion(parsedObject))
                {
                    var modifiedResponse = responseModel.Response.ToString()
                        .Replace("\"ErrorCode\": 0", "\"ErrorCode\": 1");
                    modifiedResponse = modifiedResponse.Replace("\"ErrorMessage\": \"\"",
                        "\"ErrorMessage\": \"Invalid Application Version. Kindly update from Google Play Store.\"");
                    responseModel.Response = JsonConvert.DeserializeObject(modifiedResponse);
                    responseModel.Result = responseModel.AddionalInfo = "Invalid Application Version. Kindly update from Google Play Store.";
                    responseModel.Status = false;
                }
            }
            catch (Exception exception)
            {
                responseModel.Status = false;
                responseModel.Result = exception.Message;
                LogHandler.LogError(exception);
            }

            // Generate Log
            var response = Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
            var logService = (ILogService) Bootstrapper.GetService(typeof(LogService));
            logService.GenerateLog(ServiceConstants.TestHotel, url, "Authentication", rawRequest, response.Content.ReadAsStringAsync().Result);

            return responseModel;
        }
        #endregion

        #region -- API --
        public async Task<HttpResponseMessage> Authentication(HttpRequestMessage request)
        {
            var responseModel = await GetAuthenticationObject(request);
            var response = Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
            //// Generate Log
            //_logService.GenerateLog(ServiceConstants.TestHotel, url, "Authentication", request.Content.ReadAsStringAsync().Result, response.Content.ReadAsStringAsync().Result);
            return response;
        }

        public async Task<HttpResponseMessage> AgencyBalance(HttpRequestMessage request)
        {
            var responseModel = await GetAuthenticationObject(request);
            if (null == responseModel.Response)
                return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);

            var tokenId = ((dynamic) responseModel.Response)["TokenId"].ToString();

            const string url = TboConstants.TestHotelAuthenticateUrl + "rest/GetAgencyBalance";
            var rawRequest = "{\"ClientId\": " + TboConstants.TestClientId + ", \"TokenAgencyId\": " + TboConstants.TestAgencyId +
                             ", \"TokenMemberId\": " + TboConstants.TestMemberId + ", \"TokenId\": \"" +
                             tokenId + "\", \"EndUserIp\": " + TboConstants.EndUserIp + " }";

            //return await TboRequset(request, url, rawRequest);
            var response = await TboRequset(request, url, rawRequest);
            // Generate Log
            _logService.GenerateLog(ServiceConstants.TestHotel, url, "AgencyBalance", request.Content.ReadAsStringAsync().Result, response.Content.ReadAsStringAsync().Result);
            return response;
        }

        public async Task<HttpResponseMessage> FetchCities(HttpRequestMessage request)
        {
            return await FetchCities(request, "Hotel");
        }

        public async Task<HttpResponseMessage> Search(HttpRequestMessage request)
        {
            const string url = TboConstants.TestHotelBaseUrl + "rest/GetHotelResult";
            //return await TboRequset(request, url);
            var response = await TboRequset(request, url);
            // Generate Log
            _logService.GenerateLog(ServiceConstants.TestHotel, url, "Search", request.Content.ReadAsStringAsync().Result, response.Content.ReadAsStringAsync().Result);
            return response;
        }

        [HttpPost]
        public async Task<HttpResponseMessage> GetHotelRoom(HttpRequestMessage request)
        {
            const string url = TboConstants.TestHotelBaseUrl + "rest/GetHotelRoom";
            //return await TboRequset(request, url);
            var response = await TboRequset(request, url);
            // Generate Log
            _logService.GenerateLog(ServiceConstants.TestHotel, url, "GetHotelRoom", request.Content.ReadAsStringAsync().Result, response.Content.ReadAsStringAsync().Result);
            return response;
        }
        
        [HttpPost]
        public async Task<HttpResponseMessage> GetHotelInfo(HttpRequestMessage request)
        {
            const string url = TboConstants.TestHotelBaseUrl + "rest/GetHotelInfo";
            //return await TboRequset(request, url);
            var response = await TboRequset(request, url);
            // Generate Log
            _logService.GenerateLog(ServiceConstants.TestHotel, url, "GetHotelInfo", request.Content.ReadAsStringAsync().Result, response.Content.ReadAsStringAsync().Result);
            return response;
        }

        public async Task<HttpResponseMessage> BlockRoom(HttpRequestMessage request)
        {
            const string url = TboConstants.TestHotelBaseUrl + "rest/BlockRoom";
            //return await TboRequset(request, url);
            var response = await TboRequset(request, url);
            // Generate Log
            _logService.GenerateLog(ServiceConstants.TestHotel, url, "BlockRoom", request.Content.ReadAsStringAsync().Result, response.Content.ReadAsStringAsync().Result);
            return response;
        }

        public async Task<HttpResponseMessage> Book(HttpRequestMessage request)
        {
            const string url = TboConstants.TestHotelBaseUrl + "rest/Book";
            var responseModel = await GetTboObject(request, url);
            try
            {
                if (responseModel.Response != null)
                {
                    var jObject = JObject.Parse(responseModel.Response.ToString());
                    var errorCode = jObject["BookResult"]["Error"]["ErrorCode"].ToString();
                    LogHandler.LogInfo(errorCode, LogHandler.LogType.General);
                    if (errorCode == "0")
                    {
                        var  a= jObject.ToString();
                           var response = JsonConvert.DeserializeObject<GetBookingDetailResult>(jObject["BookResult"].ToString());                        
                        var pnr = response.BookingRefNo;
                        var bookingId = response.BookingId.ToString();

                        // Save transaction in database.
                        var transactionId = SaveTransaction(ServiceConstants.TestHotel, 0,
                            pnr, bookingId, null, null, null,
                            request, responseModel.Response.ToString());

                    }
                }
            }
            catch (Exception exception)
            {
                responseModel.Status = false;
                responseModel.Result = exception.Message;
                LogHandler.LogError(exception);
            }

            // Generate Log
            var finalResponse = Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
            _logService.GenerateLog(ServiceConstants.TestHotel, url, "Book", request.Content.ReadAsStringAsync().Result, finalResponse.Content.ReadAsStringAsync().Result);
            return finalResponse;
        }

        [HttpPost]
        public async Task<HttpResponseMessage> GetHotelStaticData(HttpRequestMessage request)
        {
            const string url = TboConstants.TestHotelStaticDataUrl + "rest/GetHotelStaticData";
            //return await TboRequset(request, url);
            var response = await TboRequset(request, url);
            // Generate Log
            _logService.GenerateLog(ServiceConstants.TestHotel, url, "GetHotelStaticData", request.Content.ReadAsStringAsync().Result, response.Content.ReadAsStringAsync().Result);
            return response;
        }

        [HttpPost]
        public async Task<HttpResponseMessage> GetTaggingStaticData(HttpRequestMessage request)
        {
            const string url = TboConstants.TestHotelStaticDataUrl + "rest/GetTaggingStaticData";
            //return await TboRequset(request, url);
            var response = await TboRequset(request, url);
            // Generate Log
            _logService.GenerateLog(ServiceConstants.TestHotel, url, "GetTaggingStaticData", request.Content.ReadAsStringAsync().Result, response.Content.ReadAsStringAsync().Result);
            return response;
        }

        public async Task<HttpResponseMessage> BookingDetails(HttpRequestMessage request)
        {
            const string url = TboConstants.TestHotelBaseUrl + "rest/GetBookingDetail";            
            var response = await TboRequset(request, url);
            var jObject = JObject.Parse(response.Content.ReadAsStringAsync().Result);            
            if (response != null)
            {                
                var errorCode = jObject["response"]["GetBookingDetailResult"]["Error"]["ErrorCode"].ToString();                
                Logger.LogHandler.LogInfo("7", Logger.LogHandler.LogType.Notify);
                if (errorCode == "0")
                {
                    Logger.LogHandler.LogInfo("Getting response Successfully.", Logger.LogHandler.LogType.Notify);
                    var responseObj = JsonConvert.DeserializeObject<GetBookingDetailResult>(jObject["response"]["GetBookingDetailResult"].ToString());
                    var bookingfilePath = System.Web.HttpContext.Current.Server.MapPath(@"~/Areas/Wallet/Email Formats/hotel-bookingConfirmation.html");
                    var pdfObj = GetHTMLBody(responseObj, bookingfilePath);
                    Logger.LogHandler.LogInfo("PDF file generated.", Logger.LogHandler.LogType.Notify);
                    if (responseObj.hotelRoomsDetails.Count > 0)
                    {
                        var dateofStay = DateTime.Parse(responseObj.CheckInDate).ToString("MMM dd yyyy");
                        string emailBody = @"<html><body><p>Dear " + responseObj.hotelRoomsDetails[0].hotelPassenger[0].FirstName + ",</p><p>&nbsp;&nbsp;&nbsp;&nbsp;Thank you for booking your Hotel rooms from 4everPay.<br/> Kindly find the following attached document for your hotel stay starting from <b>" + dateofStay + "</b></p>"
                                          + @"<br> Have a wonderful stay!<br><br>&nbsp;Regards,<br>&nbsp;Team 4everPay <br><img src='" + System.Web.HttpContext.Current.Server.MapPath(@"~/Areas/Wallet/Email Formats/Email-Jugad-Logo.png") + "'/></body></html>";
                        _emailService.SendEmail((responseObj.hotelRoomsDetails[0].hotelPassenger[0].Email).ToString(), "Hotel Booking pdf.", emailBody, pdfObj, null);
                        Logger.LogHandler.LogInfo("Send email.", Logger.LogHandler.LogType.Notify);
                        var smsBody = "Dear Customer, your hotel ticket has been confirmed. Your Confirmation No is " + responseObj.ConfirmationNo +
                                      " and Booking Id is " + responseObj.BookingId.ToString();

                        SendSms((responseObj.hotelRoomsDetails[0].hotelPassenger[0].Phoneno).ToString(), smsBody, "Hotel Booking"); //responseObj.hotelRoomsDetails[0].hotelPassenger[0].Phoneno).ToString()
                        Logger.LogHandler.LogInfo("Send SMS successfully.", Logger.LogHandler.LogType.Notify);
                    }                    
                }
            }
            // Generate Log
            _logService.GenerateLog(ServiceConstants.Hotel, url, "GetTaggingStaticData", request.Content.ReadAsStringAsync().Result, response.Content.ReadAsStringAsync().Result);
            return response;
        }

        public byte[] GetHTMLBody(dynamic responseObj, dynamic readfilePath)
        {            
            var htmlBody = File.ReadAllText(readfilePath, Encoding.UTF8);
            var checkinDate = DateTime.Parse(responseObj.CheckInDate).ToString("MM/dd/yyyy  hh:mm tt");
            var checkoutDate= DateTime.Parse(responseObj.CheckOutDate).ToString("MM/dd/yyyy  hh:mm tt");
            string passengers = "",roomType="";
            htmlBody = htmlBody.Replace("@BoolingRefNo", responseObj.BookingRefNo);
            htmlBody = htmlBody.Replace("@ConfirmationNo", responseObj.ConfirmationNo);            
            htmlBody = htmlBody.Replace("@BookingDate", DateTime.Parse(responseObj.BookingDate).ToString("dd MMM yyyy"));
            htmlBody = htmlBody.Replace("@HotelName", responseObj.HotelName);
            htmlBody = htmlBody.Replace("@City", responseObj.City);
            htmlBody = htmlBody.Replace("@NoOfRooms", responseObj.NoOfRooms);
            //htmlBody = htmlBody.Replace("@NoOfNights", journeyDay);            
            htmlBody = htmlBody.Replace("@CheckIn", Convert.ToString(responseObj.CheckInDate));
            htmlBody = htmlBody.Replace("@CheckOut", Convert.ToString(responseObj.CheckOutDate));            
            for (int i = 0; i < responseObj.hotelRoomsDetails.Count; i++)
            {
                passengers += "<td>" + responseObj.hotelRoomsDetails[i].hotelPassenger[i].FirstName + " " + responseObj.hotelRoomsDetails[i].hotelPassenger[i].LastName + "</td>";
                roomType += "<li style='margin-left:45px;list-style:none;'>"+ responseObj.hotelRoomsDetails[i].RoomTypeName + "</li>";
            }
            htmlBody = htmlBody.Replace("@GuestList", "<tr align='center'>" + passengers + "</tr>");
            htmlBody = htmlBody.Replace("@RoomType", "<ul>" + roomType + "</ul>");
            htmlBody = htmlBody.Replace("@HotelAddress", responseObj.AddressLine1); 
            if(responseObj.hotelRoomsDetails.Count > 0)
                htmlBody = htmlBody.Replace("@Currancy", responseObj.hotelRoomsDetails[0].price.CurrencyCode);
            htmlBody = htmlBody.Replace("@TotalFair", responseObj.hotelRoomsDetails[0].price.PublishedPriceRoundedOff.ToString());
            htmlBody = htmlBody.Replace("@HotelLocation", responseObj.HotelName +" "+ responseObj.City);

            Logger.LogHandler.LogInfo("Html body created.", Logger.LogHandler.LogType.Notify);
            return GeneratePDF(htmlBody);
        }

        public byte[] GeneratePDF(string HtmlBody)
        {
            Logger.LogHandler.LogInfo("Creating PDF...", Logger.LogHandler.LogType.Notify);
            var document = new HtmlToPdfDocument
            {
                GlobalSettings =
                {
                    ProduceOutline = true,
                    DocumentTitle = "JUGAD",
                    PaperSize = PaperKind.A4, // Implicit conversion to PechkinPaperSize
                    Margins =
                            {
                                 All = 1.375,
                                 Unit = Unit.Centimeters
                            }
                },
                Objects = { new ObjectSettings { HtmlText = HtmlBody } }
            };

            var tempFolderDeployment = new TempFolderDeployment();
            var winAnyCPUEmbeddedDeployment = new WinAnyCPUEmbeddedDeployment(tempFolderDeployment);
            var remotingToolset = new RemotingToolset<PdfToolset>(winAnyCPUEmbeddedDeployment);

            var converter = new ThreadSafeConverter(remotingToolset);

            byte[] pdfBuf = converter.Convert(document);
            remotingToolset.Unload();

            //IConverter converter = new StandardConverter(new PdfToolset(new WinAnyCPUEmbeddedDeployment(new TempFolderDeployment())));
            //var pdfObj = converter.Convert(document);

            Logger.LogHandler.LogInfo("Sending PDF...", Logger.LogHandler.LogType.Notify);
            return pdfBuf;
        }
        #endregion
    }
}