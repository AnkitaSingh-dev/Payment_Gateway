using System.Net.Mail;

namespace Corno.Services.Email.Interfaces
{
    public interface IEmailService
    {
        #region -- Methods --
        string SendEmailAsync (string toAddress, string subject, string body);
        string SendEmailAsync(string toAddress, string subject, string body, string ServiceType);

        string SendEmailAsync(string senderId, string senderPassword,
            string smtpAddress, int smtpPort,
            string toAddress, string subject, string body, string filePath);

        string SendEmail(string toAddress, string subject, string body, byte[] invoice, byte[] eticket);

        byte[] GeneratePDF(string HtmlBody);

        #endregion
    }
}
