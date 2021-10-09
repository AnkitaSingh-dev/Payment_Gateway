using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
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
using System.Drawing.Printing;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models.Hotel;
using TuesPechkin;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Controllers
{
    public class HotelController : TboBaseController
    {
        #region -- Constructors --
        public HotelController(IdentityManager identityManager, IEmailService emailService,
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
            const string url = TboConstants.SharedServiceUrl + "/Authenticate";
            const string rawRequest = "{\"ClientId\": " + TboConstants.LiveClientId + ", \"UserName\": " + TboConstants.LiveUserName
                + ", \"Password\": " + TboConstants.LivePassword + ", \"EndUserIp\": " + TboConstants.EndUserIp + " }";

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
            var logService = (ILogService)Bootstrapper.GetService(typeof(LogService));
            logService.GenerateLog(ServiceConstants.Hotel, url, "Authentication", rawRequest, response.Content.ReadAsStringAsync().Result);

            return responseModel;
        }
        #endregion

        #region -- API --
        public async Task<HttpResponseMessage> Authentication(HttpRequestMessage request)
        {
            var responseModel = await GetAuthenticationObject(request);
            var response = Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
            // Generate Log
           // _logService.GenerateLog(ServiceConstants.Hotel, url, "Authentication", request.Content.ReadAsStringAsync().Result, response.Content.ReadAsStringAsync().Result);
            return response;
        }

        public async Task<HttpResponseMessage> AgencyBalance(HttpRequestMessage request)
        {
            var responseModel = await GetAuthenticationObject(request);
            if (null == responseModel.Response)
                return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);

            var tokenId = ((dynamic)responseModel.Response)["TokenId"].ToString();

            const string url = TboConstants.SharedServiceUrl + "/GetAgencyBalance";
            var rawRequest = "{\"ClientId\": " + TboConstants.LiveClientId + ", \"TokenAgencyId\": " + TboConstants.LiveAgencyId +
                             ", \"TokenMemberId\": " + TboConstants.LiveMemberId + ", \"TokenId\": \"" +
                             tokenId + "\", \"EndUserIp\": " + TboConstants.EndUserIp + " }";

            //return await TboRequset(request, url, rawRequest);
            var response = await TboRequset(request, url, rawRequest);
            // Generate Log
            _logService.GenerateLog(ServiceConstants.Hotel, url, "AgencyBalance", request.Content.ReadAsStringAsync().Result, response.Content.ReadAsStringAsync().Result);
            return response;
        }

        public async Task<HttpResponseMessage> FetchCities(HttpRequestMessage request)
        {
            return await FetchCities(request, "Hotel");
        }

        public async Task<HttpResponseMessage> Search(HttpRequestMessage request)
        {
            const string url = TboConstants.LiveHotelBaseUrl + "rest/GetHotelResult";
            //return await TboRequset(request, url);
            var response = await TboRequset(request, url);
            // Generate Log
            _logService.GenerateLog(ServiceConstants.Hotel, url, "Search", request.Content.ReadAsStringAsync().Result, response.Content.ReadAsStringAsync().Result);
            return response;
        }

        [HttpPost]
        public async Task<HttpResponseMessage> GetHotelRoom(HttpRequestMessage request)
        {
            const string url = TboConstants.LiveHotelBaseUrl + "rest/GetHotelRoom";
            //return await TboRequset(request, url);
            var response = await TboRequset(request, url);
            // Generate Log
            _logService.GenerateLog(ServiceConstants.Hotel, url, "GetHotelRoom", request.Content.ReadAsStringAsync().Result, response.Content.ReadAsStringAsync().Result);
            return response;
        }

        [HttpPost]
        public async Task<HttpResponseMessage> GetHotelInfo(HttpRequestMessage request)
        {
            const string url = TboConstants.LiveHotelBaseUrl + "rest/GetHotelInfo";
            //return await TboRequset(request, url);
            var response = await TboRequset(request, url);
            // Generate Log
            _logService.GenerateLog(ServiceConstants.Hotel, url, "GetHotelInfo", request.Content.ReadAsStringAsync().Result, response.Content.ReadAsStringAsync().Result);
            return response;
        }

        public async Task<HttpResponseMessage> BlockRoom(HttpRequestMessage request)
        {
            const string url = TboConstants.LiveHotelBaseUrl + "rest/BlockRoom";
            //return await TboRequset(request, url);
            var response = await TboRequset(request, url);
            // Generate Log
            _logService.GenerateLog(ServiceConstants.Hotel, url, "BlockRoom", request.Content.ReadAsStringAsync().Result, response.Content.ReadAsStringAsync().Result);
            return response;
        }

        public async Task<HttpResponseMessage> Book(HttpRequestMessage request)
        {
            const string url = TboConstants.TBOHotelBookUrl + "rest/Book";
            var responseModel = await GetTboObject(request, url);
            responseModel.Result = string.Empty;
            responseModel.Status = true;

            try
            {
                var jObject = JObject.Parse(responseModel.Response.ToString());

                var hotelResponse = JsonConvert.DeserializeObject<HotelResponse>(jObject.ToString());

              //  var errorCode = jObject["response"]["Error"]["ErrorCode"].ToString();
                //LogHandler.LogInfo(errorCode, LogHandler.LogType.General);
                if (hotelResponse.BookResult.Error.ErrorCode == "0")
                {
                    var invoice = hotelResponse.InvoiceNumber.ToString();
                    var bookingId = hotelResponse.BookingId.ToString();

                    var walletTransactionService =
                              (IWalletTransactionService)Bootstrapper.GetService(typeof(WalletTransactionService));
                    var transactionModel = new RequestModel
                    {
                        Pnr = invoice,
                        Service = ServiceConstants.Hotel,
                        BookingId = bookingId
                    };
                    walletTransactionService.AddTransaction(transactionModel);          

                    dynamic parsedObject = await GetRequestJsonObject(request);
                    ApplicationUser user = ValidateUser(parsedObject);

                //    _emailService.SendEmailAsync(user.Email, "Hotel Booking Confirmation", emailBody);

                    //var smsBody = "Dear Customer, Your hotel booking is confirmed. Your invoice number is " + invoice +
                    //               " and booking Id is " + bookingId;

                    //LogHandler.LogInfo(smsBody, LogHandler.LogType.General);
                   // SendSms(user.UserName, smsBody, "Hotel Booking Confirmation");
                }
                else
                {
                    responseModel.Status = false;
                    responseModel.Result = hotelResponse.BookResult.Error.ErrorMessage;
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
            _logService.GenerateLog(ServiceConstants.Hotel, url, "Book", request.Content.ReadAsStringAsync().Result, finalResponse.Content.ReadAsStringAsync().Result);
            return finalResponse;
        }

        [HttpPost]
        public async Task<HttpResponseMessage> GetHotelStaticData(HttpRequestMessage request)
        {
            const string url = TboConstants.LiveHotelStaticDataUrl + "rest/GetHotelStaticData";
            //return await TboRequset(request, url);
            var response = await TboRequset(request, url);
            // Generate Log
            _logService.GenerateLog(ServiceConstants.Hotel, url, "GetHotelStaticData", request.Content.ReadAsStringAsync().Result, response.Content.ReadAsStringAsync().Result);
            return response;
        }

        [HttpPost]
        public async Task<HttpResponseMessage> GetTaggingStaticData(HttpRequestMessage request)
        {
            const string url = TboConstants.LiveHotelStaticDataUrl + "rest/GetTaggingStaticData";
            //return await TboRequset(request, url);
            var response = await TboRequset(request, url);
            // Generate Log
            _logService.GenerateLog(ServiceConstants.Hotel, url, "GetTaggingStaticData", request.Content.ReadAsStringAsync().Result, response.Content.ReadAsStringAsync().Result);
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
                        _emailService.SendEmail("support@4everpayment.com", "Hotel Booking pdf.", emailBody, pdfObj, null);
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
            var checkoutDate = DateTime.Parse(responseObj.CheckOutDate).ToString("MM/dd/yyyy  hh:mm tt");
            string passengers = "", roomType = "";
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
                roomType += "<li style='margin-left:45px;list-style:none;'>" + responseObj.hotelRoomsDetails[i].RoomTypeName + "</li>";
            }
            htmlBody = htmlBody.Replace("@GuestList", "<tr align='center'>" + passengers + "</tr>");
            htmlBody = htmlBody.Replace("@RoomType", "<ul>" + roomType + "</ul>");
            htmlBody = htmlBody.Replace("@HotelAddress", responseObj.AddressLine1);
            if (responseObj.hotelRoomsDetails.Count > 0)
                htmlBody = htmlBody.Replace("@Currancy", responseObj.hotelRoomsDetails[0].price.CurrencyCode);
            htmlBody = htmlBody.Replace("@TotalFair", responseObj.hotelRoomsDetails[0].price.PublishedPriceRoundedOff.ToString());
            htmlBody = htmlBody.Replace("@HotelLocation", responseObj.HotelName + " " + responseObj.City);

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