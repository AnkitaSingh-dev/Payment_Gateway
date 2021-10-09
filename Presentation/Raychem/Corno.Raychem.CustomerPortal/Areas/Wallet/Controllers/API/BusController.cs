using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml;
using System.Xml.Serialization;
using Corno.Raychem.CustomerPortal.Areas.Admin.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Constants;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Enums;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces;
using Corno.Raychem.CustomerPortal.BusService;
using Corno.Services.Email.Interfaces;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters.Soap;
using System.Linq;
using iTextSharp.text;
using System.Net.Mail;
using iTextSharp.text.pdf;
using static iTextSharp.text.Font;
using Corno.Services.Bootstrapper;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services;
using System.Collections.Generic;
using Corno.Globals.Constants;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models.Bus;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing.Printing;
using TuesPechkin;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Controllers
{
    public class BusController : TboBaseController
    {
        #region -- Constructors --
        public BusController(IdentityManager identityManager, IEmailService emailService, ILogService logService)
            : base(identityManager)
        {
            _soapClient = new BusBookingAPISoapClient();
            _emailService = emailService;
            _logService = logService;
            _identityManager = identityManager;
        }
        #endregion

        #region -- Data Members --
        private readonly BusBookingAPISoapClient _soapClient;
        private readonly IEmailService _emailService;
        private readonly ILogService _logService;
        private readonly IdentityManager _identityManager;
        #endregion

        #region -- Methods --

        private AuthenticationData GetAuthenticationData()
        {
            var authenticationData = new AuthenticationData
            {
                SiteName = string.Empty,
                AccountCode = string.Empty,
                UserName = TboConstants.LiveUserName.Replace("\"", ""),
                Password = TboConstants.LivePassword.Replace("\"", "")
            };

            return authenticationData;
        }

        private async Task<ResponseModel> GetTboObject(HttpRequestMessage request, BusRequestType requestType)
        {
            var status = "Success";
            var bStatus = true;
            double? walletAmount = 0;
            object response = null;
            object busRequest = null;
            //const string url = "http://api.tektravels.com/BusBookingAPI/Service.asmx"; //test url for BUS API
            const string url = "http://airapi.travelboutiqueonline.com/TBOBusAPI_V1/Service.asmx";
            try
            {
                dynamic parsedObject = await GetRequestJsonObject(request);
                ApplicationUser user = ValidateUser(parsedObject);
                
                switch (requestType)
                {
                    case BusRequestType.AgencyBalance:
                        response = _soapClient.GetAgencyBalance(GetAuthenticationData(), false);
                        break;
                    case BusRequestType.Search:
                        busRequest = (WSBusSearchRequest)JsonConvert.DeserializeObject<WSBusSearchRequest>(parsedObject["request"].ToString());
                        response = _soapClient.Search(GetAuthenticationData(), (WSBusSearchRequest)busRequest);
                        _logService.GenerateLog(ServiceConstants.Bus, url, "Search", ToXml(busRequest), ToXml(response));
                        break;
                    case BusRequestType.GetSeatLayOut:
                    case BusRequestType.GetBusRouteDetail:
                        busRequest = (WSBusDetailRequest)JsonConvert.DeserializeObject<WSBusDetailRequest>(parsedObject["request"].ToString());
                        switch (requestType)
                        {
                            case BusRequestType.GetSeatLayOut:
                                response = _soapClient.GetSeatLayOut(GetAuthenticationData(), (WSBusDetailRequest)busRequest);
                                _logService.GenerateLog(ServiceConstants.Bus, url, "GetSeatLayOut", ToXml(busRequest as WSBusDetailRequest), ToXml(response));
                                break;
                            case BusRequestType.GetBusRouteDetail:
                                response = _soapClient.GetBusRouteDetail(GetAuthenticationData(), busRequest as WSBusDetailRequest);
                                _logService.GenerateLog(ServiceConstants.Bus, url, "GetBusRouteDetail", ToXml(busRequest as WSBusDetailRequest), ToXml(response));
                                break;
                        }
                        break;
                    case BusRequestType.Book:
                        busRequest = JsonConvert.DeserializeObject<WSBookRequest>(parsedObject["request"].ToString());
                        var transactionId = parsedObject["transactionId"].ToString();
                        response = _soapClient.Book(GetAuthenticationData(), (WSBookRequest)busRequest);
                        _logService.GenerateLog(ServiceConstants.Bus, url, "Book", ToXml(busRequest), ToXml(response));

                        var requestModel = ((WSBookRequest)busRequest);
                        var wsresponseModel = ((WSBookResponse)response);

                        var transaction = GetTransactionById(transactionId);

                        transaction.Service = ServiceConstants.Bus;
                        transaction.Amount = Convert.ToDouble(requestModel.TotalFare);
                        transaction.TransactionDate = DateTime.Now;
                        transaction.UserName = user.UserName;
                        transaction.DeviceId = user.DeviceId;
                        
                        // Send Email & SMS
                        if (((WSBookResponse)response).status == WSBookResponseStatus.Successful)
                        {

                            transaction.Pnr = wsresponseModel.travelOperatorPNR;
                            transaction.BookingId = wsresponseModel.BusId.ToString();
                            transaction.Source = requestModel.SourceName;
                            transaction.Destination = requestModel.DestinationName;
                            transaction.Operator = requestModel.TravelName;
                            transaction.Status = StatusConstants.Success;
                            transaction.PaymentTransactionId = wsresponseModel.ticketNo;
                            transaction.Request = requestModel.ToString();
                            transaction.Response = wsresponseModel.ToString();
                            SaveUpdatedTransaction(transaction);
                         
                        }
                        else if (((WSBookResponse)response).status == WSBookResponseStatus.Failed)
                        {

                            transaction.OpeningBalance = user.Wallet;
                            transaction.Credit = Convert.ToDouble(requestModel.TotalFare.ToString());
                            transaction.Debit = 0.0;
                            transaction.ClosingBalance = user.Wallet + Math.Round((double)requestModel.TotalFare);
                            transaction.Status = StatusConstants.Failure;

                            SaveUpdatedTransaction(transaction);

                            _identityManager.UpdateWallet(user.Id, transaction.ClosingBalance);
                        }
                        else
                        {
                            throw new Exception("Transaction Failed. Please contact : 022-28373737 for futher assistance");
                        }
                        break;
                    case BusRequestType.CancelBooking:
                        busRequest = JsonConvert.DeserializeObject<WSCancellationRequest>(parsedObject["request"].ToString());
                        response = _soapClient.CancelBooking(GetAuthenticationData(), busRequest as WSCancellationRequest);
                        _logService.GenerateLog(ServiceConstants.Bus, url, "CancelBooking", ToXml(busRequest), ToXml(response));

                        // 0
                        if (((WSCancellationResponse)response).Status.StatusCode != "0")
                        {
                            var cancelBody = "Your bus ticket no " +
                                             ((WSCancellationResponse)response).CancellationTaxNo +
                                             " is cancelled successfully.";
                            _emailService.SendEmailAsync(user.Email, "Bus Booking Cancellation", cancelBody);
                            SendSms(user.PhoneNumber, cancelBody, "Bus Cancel Booking");  
                        }
                        break;
                    case BusRequestType.GetBusBooking:
                        busRequest = JsonConvert.DeserializeObject<WSGetBusBookingRequest>(parsedObject["request"].ToString());
                        response = _soapClient.GetBusBooking(GetAuthenticationData(), busRequest as WSGetBusBookingRequest);
                        _logService.GenerateLog(ServiceConstants.Bus, url, "GetBusBooking", ToXml(busRequest), ToXml(response));
                        break;
                    case BusRequestType.GenerateInvoice:
                        busRequest = JsonConvert.DeserializeObject<WSGenerateInvoiceRequest>(parsedObject["request"].ToString());
                        response = _soapClient.GenerateInvoice(GetAuthenticationData(), busRequest as WSGenerateInvoiceRequest);
                        _logService.GenerateLog(ServiceConstants.Bus, url, "GenerateInvoice", ToXml(busRequest), ToXml(response));
                        break;
                }

                // Generate Log
                if (null == response)
                    response = string.Empty;
                _logService.GenerateLog(ServiceConstants.Bus, url, requestType.ToString(), ObjectToSoap(busRequest), ObjectToSoap(response));
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
                Response = response
            };

            return responseModel;
        }

        private async Task<HttpResponseMessage> TboRequset(HttpRequestMessage request, BusRequestType requestType)
        {
            var response = await GetTboObject(request, requestType);
            return Request.CreateResponse(HttpStatusCode.OK, response, JsonMediaTypeFormatter.DefaultMediaType);
        }

        public static string ObjectToSoap(object Object)
        {
            if (Object == null)
            {
                throw new ArgumentException("Object can not be null");
            }
            using (var stream = new MemoryStream())
            {
                var serializer = new SoapFormatter();
                serializer.Serialize(stream, Object);
                stream.Flush();
                return Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Position);
            }
        }

        public string ToXml(object oObject)
        {
            var xmlDoc = new XmlDocument();
            var xmlSerializer = new XmlSerializer(oObject.GetType());
            using (var xmlStream = new MemoryStream())
            {
                xmlSerializer.Serialize(xmlStream, oObject);
                xmlStream.Position = 0;
                xmlDoc.Load(xmlStream);
                return xmlDoc.InnerXml;
            }
        }

        #endregion

                
        #region -- API --
        public async Task<HttpResponseMessage> FetchCities(HttpRequestMessage request)
        {
            return await FetchCities(request, "Bus");
        }

        public async Task<HttpResponseMessage> AgencyBalance(HttpRequestMessage request)
        {
            return await TboRequset(request, BusRequestType.AgencyBalance);
        }

        public async Task<HttpResponseMessage> Search(HttpRequestMessage request)
        {
            return await TboRequset(request, BusRequestType.Search);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> GetSeatLayOut(HttpRequestMessage request)
        {
            return await TboRequset(request, BusRequestType.GetSeatLayOut);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> GetBusRouteDetail(HttpRequestMessage request)
        {
            return await TboRequset(request, BusRequestType.GetBusRouteDetail);
        }

        public async Task<HttpResponseMessage> Book(HttpRequestMessage request)
        {
            return await TboRequset(request, BusRequestType.Book);
        }

        public async Task<HttpResponseMessage> CancelBooking(HttpRequestMessage request)
        {
            return await TboRequset(request, BusRequestType.CancelBooking);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> GetBusBooking(HttpRequestMessage request)
        {
            var response = await TboRequset(request, BusRequestType.GetBusBooking);
            var jObject = JObject.Parse(response.Content.ReadAsStringAsync().Result);
            var responseObj = JsonConvert.DeserializeObject<Response>(jObject["response"].ToString());
            if (responseObj.busitinerary != null)
            {
                Logger.LogHandler.LogInfo("Getting response Successfully.", Logger.LogHandler.LogType.Notify);
                var bookingfilePath = System.Web.HttpContext.Current.Server.MapPath(@"~/Areas/Wallet/Email Formats/bus-bookingConfirmation.html");
                var pdfObj = GetHTMLBody(responseObj, bookingfilePath);
                Logger.LogHandler.LogInfo("PDF file generated.", Logger.LogHandler.LogType.Notify);
                var dateofJourney = DateTime.Parse(responseObj.busitinerary.dateofjourney).ToString("MMM dd yyyy");
                string emailBody = @"<html><body><p>Dear " + responseObj.busitinerary.paxdetail.firstname + ",</p><p>&nbsp;&nbsp;&nbsp;&nbsp;Thank you for booking your Bus ticket from 4everPay.<br/> Kindly find the following attached document for your travel starting from <b>" + dateofJourney + "</b></p>"
                                            + @"<br/> Have a safe and wonderful journey!<br><br>&nbsp;Regards,<br>&nbsp;Team 4everPay <br><img src='" + System.Web.HttpContext.Current.Server.MapPath(@"~/Areas/Wallet/Email Formats/Email-Jugad-Logo.png") + "'/></body></html>";
                _emailService.SendEmail(responseObj.busitinerary.paxdetail.email.ToString(), "Bus Booking pdf.", emailBody, null, pdfObj);
                _emailService.SendEmail("support@4everpayment.com", "Bus Booking pdf.", emailBody, null, pdfObj);
                Logger.LogHandler.LogInfo("Send email.", Logger.LogHandler.LogType.Notify);

                var smsBody = "Dear Customer, your bus ticket has been confirmed. Your PNR is " + responseObj.busitinerary.traveloperatorpnr.ToString() +
                              " and Ticket No. is " + responseObj.busitinerary.ticketno.ToString();

                SendSms(responseObj.busitinerary.paxdetail.phoneno.ToString(), smsBody, "Bus Booking"); //responseObj.busitinerary.paxdetail.phoneno.ToString()
                Logger.LogHandler.LogInfo("Send SMS successfully.", Logger.LogHandler.LogType.Notify);                
            }
            return response;
        }

        public async Task<HttpResponseMessage> GenerateInvoice(HttpRequestMessage request)
        {
            return await TboRequset(request, BusRequestType.GenerateInvoice);
        }      


        public byte[] GetHTMLBody(dynamic responseObj, dynamic readfilePath)
        {
            var htmlBody = File.ReadAllText(readfilePath, Encoding.UTF8);
          
            var JourneyDate = DateTime.Parse(responseObj.busitinerary.dateofjourney).ToString("MMM dd yyyy");
            var journeyDay = DateTime.Parse(responseObj.busitinerary.dateofjourney).ToString("dddd");
            var Departuretime = DateTime.Parse(responseObj.busitinerary.departuretime).ToString("hh:mm tt");
            var PassengerCount = responseObj.busitinerary.noofseats.ToString();
            string seatNos = "";
            htmlBody = htmlBody.Replace("@TicketNo", responseObj.busitinerary.ticketno);
            htmlBody = htmlBody.Replace("@PNR", responseObj.busitinerary.traveloperatorpnr.ToString());
            htmlBody = htmlBody.Replace("@From", responseObj.busitinerary.sourcename);
            htmlBody = htmlBody.Replace("@To", responseObj.busitinerary.destinationname);
            htmlBody = htmlBody.Replace("@JourneryDate", JourneyDate);
            htmlBody = htmlBody.Replace("@JourneryDay", journeyDay);
            htmlBody = htmlBody.Replace("@DepartureTime", Departuretime);
            htmlBody = htmlBody.Replace("@PassengerCount ", PassengerCount);
            for (int i = 0; i < responseObj.busitinerary.seatdetail.Count; i++)
            {
                seatNos += responseObj.busitinerary.seatdetail[i].seatname + ",";
            }
            htmlBody = htmlBody.Replace("@PassengerList", "<tr align='center'><td>" + responseObj.busitinerary.paxdetail.firstname + " " + responseObj.busitinerary.paxdetail.lastname + "</td><td>" + seatNos.TrimEnd(',') + "</td></tr>");
            htmlBody = htmlBody.Replace("@Location", responseObj.busitinerary.boardingpointdetails.citypointlocation);
            htmlBody = htmlBody.Replace("@Landmark", responseObj.busitinerary.boardingpointdetails.citypointlandmark);
            htmlBody = htmlBody.Replace("@Address", responseObj.busitinerary.boardingpointdetails.citypointaddress);
            htmlBody = htmlBody.Replace("@BusBoardingPoint", responseObj.busitinerary.sourcename + " "+ responseObj.busitinerary.boardingpointdetails.citypointlocation);
            htmlBody = htmlBody.Replace("@TickerPrice", (Convert.ToDecimal(responseObj.busitinerary.seatdetail[0].seatfare) * responseObj.busitinerary.seatdetail.Count).ToString());            
            htmlBody = htmlBody.Replace("@Currancy", responseObj.busitinerary.currency);           
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