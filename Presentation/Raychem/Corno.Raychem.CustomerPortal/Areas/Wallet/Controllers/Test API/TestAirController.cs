
using System;
using System.IO;
using System.Linq;
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
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models.Air;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces;
using Corno.Services.Bootstrapper;
using Corno.Services.Email.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Configuration;
using Corno.Globals.Enums;
using System.Drawing.Printing;
using TuesPechkin;
using System.Threading;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Controllers.Test_Server
{
    public class TestAirController : TboBaseController
    {
        #region -- Constructors --
        public TestAirController(IdentityManager identityManager, IEmailService emailService,
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
            const string url = TboConstants.TestAirlineBaseUrl + "/Authenticate";
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
            var logService = (ILogService)Bootstrapper.GetService(typeof(LogService));
            logService.GenerateLog(ServiceConstants.TestAirline, url, "Authentication", rawRequest, response.Content.ReadAsStringAsync().Result);

            return responseModel;
        }
        #endregion

        #region -- API --

        public async Task<HttpResponseMessage> FetchCities(HttpRequestMessage request)
        {
            return await FetchCities(request, ServiceConstants.TestAirline);
        }

        public async Task<HttpResponseMessage> Authentication(HttpRequestMessage request)
        {
            var responseModel = await GetAuthenticationObject(request);
            var response = Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);

            return response;
        }

        public async Task<HttpResponseMessage> AgencyBalance(HttpRequestMessage request)
        {
            var responseModel = await GetAuthenticationObject(request);
            if (null == responseModel.Response)
                return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);

            var tokenId = ((dynamic)responseModel.Response)["TokenId"].ToString();

            const string url = TboConstants.TestAirlineBaseUrl + "/GetAgencyBalance";
            var rawRequest = "{\"ClientId\": " + TboConstants.TestClientId + ", \"TokenAgencyId\": " + TboConstants.TestAgencyId +
                             ", \"TokenMemberId\": " + TboConstants.TestMemberId + ", \"TokenId\": \"" +
                             tokenId + "\", \"EndUserIp\": " + TboConstants.EndUserIp + " }";

            //return await TboRequset(request, url, rawRequest);

            var response = await TboRequset(request, url, rawRequest);
            // Generate Log
            _logService.GenerateLog(ServiceConstants.TestAirline, url, "GetAgencyBalance", request.Content.ReadAsStringAsync().Result, response.Content.ReadAsStringAsync().Result);
            return response;
        }

        public async Task<HttpResponseMessage> Search(HttpRequestMessage request)
        {
            const string url = TboConstants.TestAirlineBaseUrl1 + "/Search";
            var response = await TboRequset(request, url);
            // Generate Log
            _logService.GenerateLog(ServiceConstants.TestAirline, url, "Search", request.Content.ReadAsStringAsync().Result, response.Content.ReadAsStringAsync().Result);
            return response;
        }

        public async Task<HttpResponseMessage> PriceRbd(HttpRequestMessage request)
        {
            const string url = TboConstants.TestAirlineBaseUrl1 + "/PriceRBD";
            var response = await TboRequset(request, url);
            // Generate Log
            _logService.GenerateLog(ServiceConstants.TestAirline, url, "PriceRbd", request.Content.ReadAsStringAsync().Result, response.Content.ReadAsStringAsync().Result);
            return response;
        }

        public async Task<HttpResponseMessage> GetCalenderFare(HttpRequestMessage request)
        {
            const string url = TboConstants.TestAirlineBaseUrl1 + "/GetCalendarFare";
            var response = await TboRequset(request, url);
            // Generate Log
            _logService.GenerateLog(ServiceConstants.TestAirline, url, "GetCalenderFare", request.Content.ReadAsStringAsync().Result, response.Content.ReadAsStringAsync().Result);
            return response;
        }

        public async Task<HttpResponseMessage> UpdateCalenderFareOfDay(HttpRequestMessage request)
        {
            const string url = TboConstants.TestAirlineBaseUrl1 + "/UpdateCalendarFareOfDay";
            var response = await TboRequset(request, url);
            // Generate Log
            _logService.GenerateLog(ServiceConstants.TestAirline, url, "UpdateCalenderFareOfDay", request.Content.ReadAsStringAsync().Result, response.Content.ReadAsStringAsync().Result);
            return response;
        }

        public async Task<HttpResponseMessage> FareRule(HttpRequestMessage request)
        {
            const string url = TboConstants.TestAirlineBaseUrl1 + "/FareRule";
            var response = await TboRequset(request, url);
            // Generate Log
            _logService.GenerateLog(ServiceConstants.TestAirline, url, "FareRule", request.Content.ReadAsStringAsync().Result, response.Content.ReadAsStringAsync().Result);
            return response;
        }

        public async Task<HttpResponseMessage> FareQuote(HttpRequestMessage request)
        {
            const string url = TboConstants.TestAirlineBaseUrl1 + "/FareQuote";
            var response = await TboRequset(request, url);
            // Generate Log
            _logService.GenerateLog(ServiceConstants.TestAirline, url, "FareQuote", request.Content.ReadAsStringAsync().Result, response.Content.ReadAsStringAsync().Result);
            return response;
        }

        public async Task<HttpResponseMessage> Ssr(HttpRequestMessage request)
        {
            const string url = TboConstants.TestAirlineBaseUrl1 + "/SSR";
            var response = await TboRequset(request, url);
            // Generate Log
            _logService.GenerateLog(ServiceConstants.TestAirline, url, "Ssr", request.Content.ReadAsStringAsync().Result, response.Content.ReadAsStringAsync().Result);
            return response;
        }

        public async Task<HttpResponseMessage> Book(HttpRequestMessage request)
        {
            const string url = TboConstants.TestAirlineBaseUrl1 + "/Book";
            var response = await TboRequset(request, url);
            // Generate Log
            _logService.GenerateLog(ServiceConstants.TestAirline, url, "Book", request.Content.ReadAsStringAsync().Result, response.Content.ReadAsStringAsync().Result);
            return response;
        }

        public async Task<HttpResponseMessage> Ticket(HttpRequestMessage request)
        {
            const string url = TboConstants.TestAirlineBaseUrl1 + "/Ticket";
            var responseModel = await GetTboObject(request, url);
            try
            {
                var jObject = JObject.Parse(responseModel.Response.ToString());
                var errorCode = jObject["Response"]["Error"]["ErrorCode"].ToString();
                //LogHandler.LogInfo("Error Code : " + errorCode, LogHandler.LogType.General);
                if (errorCode == "0")
                {
                    var response = JsonConvert.DeserializeObject<Response>(jObject["Response"]["Response"].ToString());

                    var pnr = response.PNR;
                    var bookingId = response.BookingId.ToString();
                    var source = response.FlightItinerary.Origin;
                    var destination = response.FlightItinerary.Destination;
                    var tboOperator = response.FlightItinerary.AirlineCode;

                    // Save transaction in database.
                    var transactionId = SaveTransaction(ServiceConstants.Airline, 0,
                        pnr, bookingId, source, destination, tboOperator,
                        request, responseModel.Response.ToString());
                    responseModel.TransactionId = transactionId.PadLeft(6, '0');

                
                }
                else
                {
                    throw new Exception("Transaction Failed. Please contact : 022-28373737 for futher assistance");
                }
            }
            catch (Exception exception)
            {
                responseModel.Status = false;
                responseModel.Result = exception.Message;
                LogHandler.LogError(exception);
            }
            //return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);

            var finalResponse = Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
            // Generate Log
            _logService.GenerateLog(ServiceConstants.Airline, url, "Ticket", request.Content.ReadAsStringAsync().Result, finalResponse.Content.ReadAsStringAsync().Result);
            return finalResponse;
        }

        public async Task<HttpResponseMessage> GetBookingDetails(HttpRequestMessage request)
        {
            const string url = TboConstants.TestAirlineBaseUrl1 + "/GetBookingDetails";
            var response = await TboRequset(request, url);
            // Generate Log
            _logService.GenerateLog(ServiceConstants.TestAirline, url, "GetBookingDetails", request.Content.ReadAsStringAsync().Result, response.Content.ReadAsStringAsync().Result);
            return response;
        }

        public async Task<HttpResponseMessage> ReleasePnrRequest(HttpRequestMessage request)
        {
            const string url = TboConstants.TestAirlineBaseUrl1 + "/ReleasePNRRequest";
            var response = await TboRequset(request, url);
            // Generate Log
            _logService.GenerateLog(ServiceConstants.TestAirline, url, "ReleasePnrRequest", request.Content.ReadAsStringAsync().Result, response.Content.ReadAsStringAsync().Result);
            return response;
        }

        public async Task<HttpResponseMessage> SendChangeRequest(HttpRequestMessage request)
        {
            const string url = TboConstants.TestAirlineBaseUrl1 + "/SendChangeRequest";
            var response = await TboRequset(request, url);
            // Generate Log
            _logService.GenerateLog(ServiceConstants.TestAirline, url, "SendChangeRequest", request.Content.ReadAsStringAsync().Result, response.Content.ReadAsStringAsync().Result);
            return response;
        }

        [HttpPost]
        public async Task<HttpResponseMessage> GetChangeRequestStatus(HttpRequestMessage request)
        {
            const string url = TboConstants.TestAirlineBaseUrl1 + "/GetChangeRequestStatus";
            dynamic parsedObject = await GetRequestJsonObject(request);
            var tokenId = parsedObject["TokenId"].ToString();
            var changeRequestId = parsedObject["ChangeRequestId"].ToString();

            var rawRequest = "{\"ChangeRequestId\": " + changeRequestId + ", \"TokenId\": \"" +
                 tokenId + "\", \"EndUserIp\": " + TboConstants.EndUserIp + " }";
            var response = await TboRequset(request, url, rawRequest);
            // Generate Log
            _logService.GenerateLog(ServiceConstants.TestAirline, url, "GetChangeRequestStatus", request.Content.ReadAsStringAsync().Result, response.Content.ReadAsStringAsync().Result);
            return response;
        }

        public async Task<HttpResponseMessage> Logout(HttpRequestMessage request)
        {
            const string url = TboConstants.LiveAirlineBaseUrl + "/Logout";
            var response = await TboRequset(request, url);
            // Generate Log
            _logService.GenerateLog(ServiceConstants.TestAirline, url, "Logout", request.Content.ReadAsStringAsync().Result, response.Content.ReadAsStringAsync().Result);
            return response;
        }

        public async Task<HttpResponseMessage> BookingDetails(HttpRequestMessage request)
        {
            const string url = TboConstants.TestAirlineBaseUrl1 + "/GetBookingDetails"; //_NEwBookURl 
            byte[] invoice = null; byte[] eticket = null;
            Logger.LogHandler.LogInfo("1", Logger.LogHandler.LogType.Notify);
            dynamic parsedObject = await GetRequestJsonObject(request);
            Logger.LogHandler.LogInfo("2", Logger.LogHandler.LogType.Notify);
            var tokenId = parsedObject["TokenId"].ToString();
            var traceId = parsedObject["TraceId"].ToString();
            Logger.LogHandler.LogInfo("3", Logger.LogHandler.LogType.Notify);
            var PNR = parsedObject["PNR"].ToString();
            var bookingId = parsedObject["BookingId"].ToString();
            var tripIndicator = parsedObject["TripIndicator"].ToString();
            Logger.LogHandler.LogInfo("4", Logger.LogHandler.LogType.Notify);
            dynamic cabinClass = parsedObject["CabinClass"].ToString() == "2" ? TboCabinClass.Economy.ToString() : parsedObject["CabinClass"].ToString() == "3" ? TboCabinClass.PremiumEconomy.ToString() : parsedObject["CabinClass"].ToString() == "4" ? TboCabinClass.Business.ToString() : parsedObject["CabinClass"].ToString() == "5" ? TboCabinClass.PremiumBusiness.ToString() : parsedObject["CabinClass"].ToString() == "6" ? TboCabinClass.First.ToString() : TboCabinClass.All.ToString();
            Logger.LogHandler.LogInfo("5", Logger.LogHandler.LogType.Notify);
            var rawRequest = "{\"EndUserIp\":" + TboConstants.EndUserIp + ", \"TokenId\":\"" + tokenId
                             + "\", \"TraceId\":\"" + traceId + "\", \"PNR\":\"" + PNR + "\", \"BookingId\": \"" + bookingId
                             + "\", \"TripIndicator\": \"" + tripIndicator + "\" }";
            Logger.LogHandler.LogInfo("6", Logger.LogHandler.LogType.Notify);
            var response = await TboRequset(request, url, rawRequest);
            var jObject = JObject.Parse(response.Content.ReadAsStringAsync().Result);
            var errorCode = jObject["response"]["Response"]["Error"]["ErrorCode"].ToString();
            Logger.LogHandler.LogInfo("7", Logger.LogHandler.LogType.Notify);
            if (errorCode == "0")
            {
                var responseObj = JsonConvert.DeserializeObject<Response>(jObject["response"]["Response"].ToString());
                var onwardJourneyDate = DateTime.Parse(responseObj.FlightItinerary.Segments[0].Origin.DepTime).ToString("MMM dd yyyy");
                var bookingfilePath = System.Web.HttpContext.Current.Server.MapPath(@"~/Areas/Wallet/Email Formats/flight-bookingConfirmation-PNR.html");
                var invoicefilePath = System.Web.HttpContext.Current.Server.MapPath(@"~/Areas/Wallet/Email Formats/flight-invoice.html");
                for (int i = 0; i < 2; i++)
                {
                    if (i == 0)
                        invoice = GetHTMLBody(responseObj, invoicefilePath, "Flight Invoice", cabinClass);
                    else
                        eticket = GetHTMLBody(responseObj, bookingfilePath, "Flight Booking Confirmation", cabinClass);
                }
                Logger.LogHandler.LogInfo("8", Logger.LogHandler.LogType.Notify);
                var emailBody = @"<html><body><p>Dear " + responseObj.FlightItinerary.Passenger[0].FirstName + ",</p><p>&nbsp;&nbsp;&nbsp;&nbsp;Thank you for booking your flight tickets from 4everPay.</br>Kindly find attached the following documents for your travel starting on <b>" + onwardJourneyDate + "</b>.</p>"
                                    + @"<ul><li>Flight E-Ticket</li><li>Tax Invoice</li ></ul><br> Have a safe and wonderful journey!!!<br><br>&nbsp;Regards,<br>&nbsp;Team 4everPay <br><img src='" + System.Web.HttpContext.Current.Server.MapPath(@"~/Areas/Wallet/Email Formats/Email-Jugad-Logo.png") + "'/></body></html>";
                //(responseObj.FlightItinerary.Passenger[0].Email).ToString()
                Logger.LogHandler.LogInfo("9", Logger.LogHandler.LogType.Notify);
                _emailService.SendEmail((responseObj.FlightItinerary.Passenger[0].Email).ToString(), "Flight Booking pdfs.", emailBody, invoice, eticket);
                //var smsBody = "Dear Customer, Your flight ticket has been confirmed. Your PNR is " + responseObj.FlightItinerary.PNR +
                //                  " and booking Id is " + responseObj.FlightItinerary.BookingId;

                //SendSms((responseObj.FlightItinerary.Passenger[0].ContactNo).ToString(), smsBody, "Flight Booking"); //responseObj.FlightItinerary.Passenger[0].ContactNo                
            }
            // Generate Log               
            _logService.GenerateLog(ServiceConstants.Airline, url, "GetBookingDetails", request.Content.ReadAsStringAsync().Result, response.Content.ReadAsStringAsync().Result);
            return response;
        }

        public byte[] GetHTMLBody(dynamic responseObj, dynamic readfilePath, dynamic fname, dynamic flightCabinClass)
        {
            Logger.LogHandler.LogInfo("Enter Get HTML Body", LogHandler.LogType.Notify);
            var discountAmount = Convert.ToDouble(ConfigurationManager.AppSettings["DiscountAmount"].ToString());
            var convenienceFee = Convert.ToDouble(ConfigurationManager.AppSettings["ConvenienceFee"].ToString());
            var htmlBody = File.ReadAllText(readfilePath, Encoding.UTF8);
            var timeSpan = responseObj.FlightItinerary.Segments[0].Duration;
            var PNR = responseObj.FlightItinerary.PNR;
            var BookDate = DateTime.Parse(responseObj.FlightItinerary.Passenger[0].Ticket.IssueDate).ToString("MMM dd yyyy");
            var onwardJourneyDate = DateTime.Parse(responseObj.FlightItinerary.Segments[0].Origin.DepTime).ToString("MMM dd yyyy");

            htmlBody = htmlBody.Replace("@BookedBy", responseObj.FlightItinerary.Passenger[0].FirstName + " " + responseObj.FlightItinerary.Passenger[0].LastName);
            htmlBody = htmlBody.Replace("@MailID", responseObj.FlightItinerary.Passenger[0].Email);
            htmlBody = htmlBody.Replace("@MoNo", responseObj.FlightItinerary.Passenger[0].ContactNo);
            htmlBody = htmlBody.Replace("@BookID", responseObj.FlightItinerary.BookingId.ToString());
            htmlBody = htmlBody.Replace("@BookDate", BookDate);
            htmlBody = htmlBody.Replace("@OnwardJourneyDate", onwardJourneyDate);
            htmlBody = htmlBody.Replace("@AirLineCode", responseObj.FlightItinerary.Segments[0].Airline.AirlineCode);
            htmlBody = htmlBody.Replace("@AirLine", responseObj.FlightItinerary.Segments[0].Airline.AirlineName);
            htmlBody = htmlBody.Replace("@FlightNo", responseObj.FlightItinerary.Segments[0].Airline.FlightNumber);
            htmlBody = htmlBody.Replace("@AirportCodeFrom", responseObj.FlightItinerary.Segments[0].Origin.Airport.AirportCode);
            htmlBody = htmlBody.Replace("@From", responseObj.FlightItinerary.Segments[0].Origin.Airport.CityName);
            htmlBody = htmlBody.Replace("@AirportCodeTo", responseObj.FlightItinerary.Segments[0].Destination.Airport.AirportCode);
            htmlBody = htmlBody.Replace("@To", responseObj.FlightItinerary.Segments[0].Destination.Airport.CityName);
            if (responseObj.FlightItinerary.Passenger != null && responseObj.FlightItinerary.Passenger.Count > 0)
            {
                string passengerBody = "", passengerListPNRTkt = "";
                for (int i = 0; i < responseObj.FlightItinerary.Passenger.Count; i++)
                {
                    passengerBody += "<tr align='center'><td>" + responseObj.FlightItinerary.Passenger[i].FirstName + " " + responseObj.FlightItinerary.Passenger[i].LastName + "</td ></tr>";
                    passengerListPNRTkt += "<tr align='center'><td>" + responseObj.FlightItinerary.Passenger[i].FirstName + " " + responseObj.FlightItinerary.Passenger[i].LastName + "</td ><td>" + PNR + "</td >" + "<td>" + responseObj.FlightItinerary.Passenger[i].Ticket.TicketNumber + "</td></tr>";
                }
                htmlBody = htmlBody.Replace("@PassengersList", passengerBody);
                htmlBody = htmlBody.Replace("@PassengerListPNRTicket", passengerListPNRTkt);
            }
            htmlBody = htmlBody.Replace("@DepTime", responseObj.FlightItinerary.Segments[0].Origin.DepTime);
            htmlBody = htmlBody.Replace("@ArrTime", responseObj.FlightItinerary.Segments[0].Destination.ArrTime);
            htmlBody = htmlBody.Replace("@AirportNameFrom", responseObj.FlightItinerary.Segments[0].Origin.Airport.AirportName);
            htmlBody = htmlBody.Replace("@AirportNameTo", responseObj.FlightItinerary.Segments[0].Destination.Airport.AirportName);
            htmlBody = htmlBody.Replace("@BaggageCheckin", responseObj.FlightItinerary.Passenger[0].SegmentAdditionalInfo[0].Baggage);
            htmlBody = htmlBody.Replace("@TravelTime", (timeSpan / 60).ToString() + " hr" + (timeSpan % 60).ToString() + " Min");
            htmlBody = htmlBody.Replace("@TerminalFrom", responseObj.FlightItinerary.Segments[0].Origin.Airport.Terminal);
            htmlBody = htmlBody.Replace("@TermsAndConditions", responseObj.FlightItinerary.FareRules[0].FareRuleDetail);
            htmlBody = htmlBody.Replace("@LocationAddress", responseObj.FlightItinerary.Segments[0].Destination.Airport.AirportName);
            htmlBody = htmlBody.Replace("@Class", flightCabinClass);

            htmlBody = htmlBody.Replace("@Currency", responseObj.FlightItinerary.Fare.Currency);
            htmlBody = htmlBody.Replace("@BaseFare", (responseObj.FlightItinerary.Fare.BaseFare).ToString());
            htmlBody = htmlBody.Replace("@FuelCharge", (responseObj.FlightItinerary.Fare.YQTax).ToString());
            htmlBody = htmlBody.Replace("@OtherTaxes", (responseObj.FlightItinerary.Fare.Tax + responseObj.FlightItinerary.Fare.ServiceFee).ToString());
            htmlBody = htmlBody.Replace("@OtherCharges", (responseObj.FlightItinerary.Fare.OtherCharges).ToString());
            htmlBody = htmlBody.Replace("@JugadDiscount", discountAmount.ToString());
            htmlBody = htmlBody.Replace("@ConvenienceFee", convenienceFee.ToString());
            htmlBody = htmlBody.Replace("@GrandTotal", ((responseObj.FlightItinerary.Fare.BaseFare + responseObj.FlightItinerary.Fare.Tax + responseObj.FlightItinerary.Fare.ServiceFee + responseObj.FlightItinerary.Fare.OtherCharges) + discountAmount).ToString());
            Logger.LogHandler.LogInfo("10", Logger.LogHandler.LogType.Notify);
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