using Corno.Data.SMS;
using Corno.Services.Base.Interfaces;
using Corno.Services.SMS.Interfaces;
using System;
using System.Net;

namespace Corno.Services.SMS
{

    public class BhashSmsService : SmsService<SmsLog>, IBhashSmsService
    {
        #region -- Constructors --
        public BhashSmsService(IUnitOfWork unitOfWork, IGenericRepository<SmsLog> smsLogRepository)
            : base(unitOfWork, smsLogRepository)
        {
        }
        #endregion

        #region -- Methods --
        public override string SendSms(string phoneNo, string smsBody)
        {
            var uri = "http://bhashsms.com/api/sendmsg.php?";
            uri += "user=" + UserName;
            uri += "&pass=" + Password;
            uri += "&sender=" + SenderId;
            uri += "&phone=" + phoneNo;
            uri += "&text=" + smsBody;
            uri += "&priority=ndnd&stype=normal";

            try
            {
                //var webClient = new System.Net.WebClient();Hide admin menu items based on permissions
                //webClient.DownloadString(URI);
                var req = (HttpWebRequest)WebRequest.Create(uri);
                req.UserAgent = ".NET Framework Client";
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