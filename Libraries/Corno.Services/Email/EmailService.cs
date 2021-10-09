using System;
using System.Drawing.Printing;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Corno.Services.Email.Interfaces;
using Microsoft.AspNet.Identity;

namespace Corno.Services.Email
{
    public class EmailService : IEmailService, IIdentityMessageService
    {
        #region -- Methods --
        public Task SendAsync(IdentityMessage message)
        {
            var result = SendEmailAsync(message.Destination, message.Subject, message.Body);
            return Task.FromResult(result);
        }

        public string SendEmailAsync(string toAddress, string subject, string body)
        {
            var result = "Message Sent Successfully..!!";
            var senderId = "info@4everpayment.com";// use sender’s email id here..
            const string senderPassword = "r@hul@4everp@y"; // sender password here…
            try
            {
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com", // smtp server address here…
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(senderId, senderPassword),
                    Timeout = 30000
                };
                var message = new MailMessage(senderId, toAddress, subject, body) { IsBodyHtml = true };
                smtp.Send(message);
            }
            catch (Exception exception)
            {
                result = "Error sending email.!!! : " + exception.Message;
            }
            return result;
        }

        public string SendEmailAsync(string toAddress, string subject, string body, string ServiceType)
        {
            var result = "Message Sent Successfully..!!";
            var senderId = "info@4everpayment.com";// use sender’s email id here..
            const string senderPassword = "r@hul@4everp@y"; // sender password here…
            try
            {
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com", // smtp server address here…
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(senderId, senderPassword),
                    Timeout = 30000
                };
                var message = new MailMessage(senderId, toAddress, subject, body) { IsBodyHtml = true };
                var path = System.AppDomain.CurrentDomain.BaseDirectory.ToString();
                string pathLogo = path + @"\img\Images\JugadLogoBlack.png";
                string pathGif = path + @"\img\Images\okok.gif";
                string path4ever = path + @"\img\Images\4EverLogo.png";

                LinkedResource logo = new LinkedResource(pathLogo);
                logo.ContentId = "JugadLogo";

                LinkedResource gif = new LinkedResource(pathGif);
                gif.ContentId = "OkokLogo";

                LinkedResource ever = new LinkedResource(path4ever);
                ever.ContentId = "4everLogo";

                //now do the HTML formatting
                AlternateView av1 = AlternateView.CreateAlternateViewFromString(body,null, MediaTypeNames.Text.Html);

                //now add the AlternateView
                av1.LinkedResources.Add(logo);
                av1.LinkedResources.Add(gif);
                av1.LinkedResources.Add(ever);

                //now append it to the body of the mail
                message.AlternateViews.Add(av1);

                smtp.Send(message);
            }
            catch (Exception exception)
            {
                result = "Error sending email.!!! : " + exception.Message;
            }
            return result;
        }

        public string SendEmailAsync(string senderId, string senderPassword,
            string smtpAddress, int smtpPort,
            string toAddress, string subject, string body, string filePath)
        {
            const string result = "Message Sent Successfully..!!";
            var smtp = new SmtpClient
            {
                Host = smtpAddress, // smtp server address here…
                Port = smtpPort,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(senderId, senderPassword),
                Timeout = 30000
            };
            var message = new MailMessage(senderId, toAddress, subject, body) { IsBodyHtml = true };
            if(!string.IsNullOrEmpty(filePath))
                message.Attachments.Add(new Attachment(filePath));
            smtp.SendAsync(message, null);

            return result;
        }

        public string SendEmail(string toAddress, string subject, string body, byte[] invoice=null, byte[] eticket=null)
        {
            string result = "Message Sent Successfully...!!!";
            try
            {               
                var senderMailId = "info@4everpayment.com";// use sender’s email id here..
                const string senderMailPassword = "r@hul@4everp@y"; // sender password here…
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com", // smtp server address here…
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(senderMailId, senderMailPassword),
                    Timeout = 30000
                };
                var message = new MailMessage(senderMailId, toAddress, subject, body) { IsBodyHtml = true };

                //string[] files = Directory.GetFiles(filePath);
                //if (!string.IsNullOrEmpty(filePath) && files.Length > 0)
                //{
                //    foreach (string fileName in files)
                //    {
                if(invoice != null)
                    message.Attachments.Add(new Attachment(new MemoryStream(invoice),"Invoice.pdf"));
                if(eticket != null)
                    message.Attachments.Add(new Attachment(new MemoryStream(eticket), "ETicket.pdf"));
                //    }
                //}
                smtp.Send(message);
                message.Dispose();
                smtp.Dispose();                
            }
            catch (Exception)
            {

                throw;
            }
            return result;
        }


        public byte[] GeneratePDF(string HtmlBody)
        {
            Logger.LogHandler.LogInfo("Creating PDF...", Logger.LogHandler.LogType.Notify);
            //var document = new HtmlToPdfDocument
            //{
            //    GlobalSettings =
            //    {
            //        ProduceOutline = true,
            //        DocumentTitle = "JUGAD",
            //        PaperSize = PaperKind.A4, // Implicit conversion to PechkinPaperSize
            //        Margins =
            //                {
            //                     All = 1.375,
            //                     Unit = Unit.Centimeters
            //                }
            //    },
            //    Objects = { new ObjectSettings { HtmlText = HtmlBody } }
            //};
            //IConverter converter = new StandardConverter(new PdfToolset(new WinAnyCPUEmbeddedDeployment(new TempFolderDeployment())));
            //var pdfObj = converter.Convert(document);
            //document = null;
            //Logger.LogHandler.LogInfo("Sending PDF...", Logger.LogHandler.LogType.Notify);
            return null;
        }
        #endregion
    }
}
