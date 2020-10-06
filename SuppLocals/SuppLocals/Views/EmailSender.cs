using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace SuppLocals
{
    public class EmailSender : SignUp
    {
        public void SendEmail(string email)
        {
            var fromAddress = new MailAddress(email, "From Person");
            var toAddress = new MailAddress("toemail@gmail.com", "To Person");
            const string fromPassword = "password";
            const string subject = "Locals to Locals";
            const string body = "This is confirmation email.";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
                Timeout = 10000
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }
    }
}