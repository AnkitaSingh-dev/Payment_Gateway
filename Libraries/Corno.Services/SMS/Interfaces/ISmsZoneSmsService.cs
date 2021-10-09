using Corno.Data.SMS;

namespace Corno.Services.SMS.Interfaces
{
    public interface ISmsOzoneSmsService : ISmsService<SmsLog>
    {
        string SendSmsOzone(string phoneNo, string smsBody);
        string SendOtp(string phoneNo, string smsBody, string otp);
    }
}