using Corno.Data.SMS;
using Corno.Services.Base.Interfaces;
using Corno.Services.SMS.Interfaces;
using System;
using System.Net;

namespace Corno.Services.SMS
{

    public class SmsOzoneSmsService : SmsService<SmsLog>, ISmsOzoneSmsService
    {
        #region -- Constructors --
        public SmsOzoneSmsService(IUnitOfWork unitOfWork, IGenericRepository<SmsLog> smsLogRepository)
            : base(unitOfWork, smsLogRepository)
        {
        }
        #endregion

        #region -- Methods --
        public string SendSmsOzone(string phoneNo, string smsBody)
        {
            //var uri = "http://cloud.smsozone.com/secure/sendsms.php?";
            //uri += "authkey=9GPDEHw0E0";
            //uri += "&to=" + phoneNo;
            //uri += "&message=" + smsBody;

            var uri = "http://login.smsozone.com/api/mt/SendSMS?";
            uri += "APIKey=9GPDEHw0E0";
            uri += "&senderid=FOREPS";
            uri += "&channel=Trans";
            uri += "&number=" + phoneNo;
            uri += "&text=" + smsBody;
            uri += "&route=1055";
            uri += "&DCS=0&flashsms=0";

            //var uri = "http://smsozone.com/api/mt/SendSMS?";
            //uri += "user=" + UserName;
            //uri += "&password=" + Password;
            //uri += "&senderid=" + SenderId;
            //uri += "&number=" + phoneNo;
            //uri += "&channel=" + Channel;
            //uri += "&route=" + Route;
            //uri += "&text=" + smsBody;
            //uri += "&DCS=0&flashsms=0";

            // return uri;

            try
            {
                var req = (HttpWebRequest)WebRequest.Create(uri);
                req.UserAgent = ".NET Framework Test Client";
                req.ContentType = "application/x-www-form-urlencoded";
                var resp = (HttpWebResponse)req.GetResponse();
                var stream = resp.GetResponseStream();

                if (stream == null) return null;

                var sr = new System.IO.StreamReader(stream);
                return sr.ReadToEnd().Trim();
            }
            catch (WebException ex)
            {
                var httpWebResponse = ex.Response as HttpWebResponse;
                if (httpWebResponse == null) return null;
                switch (httpWebResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return "404:URL not found :" + uri;
                    case HttpStatusCode.BadRequest:
                        return "400:Bad Request";
                    default:
                        return httpWebResponse.StatusCode.ToString();
                }
            }
        }

        public string SendSms(string uri)
        {
            try
            {
                var req = (HttpWebRequest)WebRequest.Create(uri);
                req.UserAgent = ".NET Framework Test Client";
                req.ContentType = "application/x-www-form-urlencoded";
                var resp = (HttpWebResponse)req.GetResponse();
                var stream = resp.GetResponseStream();

                if (stream == null) return null;

                var sr = new System.IO.StreamReader(stream);
                return sr.ReadToEnd().Trim();
            }
            catch (WebException ex)
            {
                var httpWebResponse = ex.Response as HttpWebResponse;
                if (httpWebResponse == null) return null;
                switch (httpWebResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return "404:URL not found :" + uri;
                    case HttpStatusCode.BadRequest:
                        return "400:Bad Request";
                    default:
                        return httpWebResponse.StatusCode.ToString();
                }
            }
        }

        public string SendOtp(string phoneNo, string smsBody, string otp)
        {
            var uri = "http://smsalerts.ozonesms.com/api/otp.php?";
            uri += "authkey=183655Asa76Bkr3IS5a0e6bb8";
            uri += "&mobile=" + phoneNo;
            uri += "&message=" + smsBody;
            uri += "&sender=FOREPS";
            uri += "&otp=" + otp;

            return SendSms(uri);
        }

        public override void SaveSmsLog(string phoneNo, string smsBody, string smsResult,
            DateTime transactionDate, string transactionType, int departmentId)
        {
            var smsLog = new SmsLog
            {
                SendDate = DateTime.Now,
                TransactionDate = transactionDate,
                TransactionType = transactionType,
                DepartmentId = departmentId,
                FromNo = SenderId,
                ToNo = phoneNo,
                Message = smsBody,
                UserName = UserName,
                Password = Password,
                SenderId = SenderId,
                Priority = string.Empty,
                SmsType = string.Empty,
                SmsResult = smsResult
            };

            Add(smsLog);
            Save();
        }
    }
    #endregion
}