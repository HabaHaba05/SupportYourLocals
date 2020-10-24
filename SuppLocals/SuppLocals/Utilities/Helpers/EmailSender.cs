using SuppLocals.Views;
using System;
using System.Net;
using System.Net.Mail;

namespace SuppLocals
{
    public class EmailSender : SignUp
    {
        public EmailSender()
        {
        }

        public void SendEmail(string email, string code)
        {
            var fromAddress = new MailAddress(email, "From Person");
            var toAddress = new MailAddress("lukasstc223@gmail.com", "To Person");


            const string fromPassword = "Support1234";
            const string subject = "Locals to Locals";
            var body = "This is temporary code to login: " + code;

            using var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
                Timeout = 10000
            };
            using var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            };
            smtp.Send(message);
        }

        public string GenerateRandomString()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();

            for (var i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new string(stringChars);
            return finalString;
        }
    }
}