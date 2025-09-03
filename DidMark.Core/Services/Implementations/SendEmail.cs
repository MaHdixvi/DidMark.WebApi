using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DidMark.Core.Services.Interfaces;

namespace DidMark.Core.Services.Implementations
{
    public class SendEmail : IMailSender
    {
        public void Send(string to, string subject, string body)
        {
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            mail.From = new MailAddress("narges.moharrami9779@gmail.com", "DidMark");
            mail.To.Add(to);
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            //SmtpServer.UseDefaultCredentials = false;
            SmtpServer.Port = 587;
            //SmtpServer.Credentials = new NetworkCredential("narges.moharrami97@gmail.com", "pgfgldweybcxmhbz");
            SmtpServer.Credentials = new NetworkCredential("narges.moharrami9779@gmail.com", "svvslpmsuqcdylaw");
            Console.WriteLine("start!");

            SmtpServer.EnableSsl = true; // only for port 465
            try
            {
                SmtpServer.Send(mail);
                Console.WriteLine("Email sent successfully!");
            }
            catch (SmtpException ex)
            {
                // Handle the exception
                Console.WriteLine($"Failed to send email: {ex.Message}");
            }
        }

    }
}
