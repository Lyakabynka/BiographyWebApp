using Azure.Core;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace BiographyWebApp.Services
{
    public class EmailSenderService
    {
        private readonly MailAddress fromEmail;
        private readonly string fromEmailPassword;
        private readonly SmtpClient smtp;
        public EmailSenderService()
        {
            fromEmail = new("biographywebapplication@gmail.com", "Biography Web Application");

            fromEmailPassword = "*";

            smtp = new SmtpClient()
             {
                 Host = "smtp.gmail.com",
                 Port = 587,
                 EnableSsl = true,
                 DeliveryMethod = SmtpDeliveryMethod.Network,
                 UseDefaultCredentials = false,
                 Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword),
             };
        }
        public async Task SendVerificationLinkEmailAsync(HttpContext context, string email, string activationCode)
        {
            string verificationLink = "/User/VerifyAccount/" + activationCode;
            string link = context.Request.GetDisplayUrl().Replace(context.Request.Path, verificationLink);

            string subject = "Your account has been successfully created!";

            string body = "We are exited to tell you that your Biography Web Application account has been successfully created.<br/>" +
                "Please, click on the below link to verify your account.<br/>" +
                $"<a href='{link}'>*Click*</a>";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendForgotPasswordLinkEmailAsync(HttpContext context, string email, string resetPasswordCode)
        {
            string verificationLink = "/User/ResetPassword/" + resetPasswordCode;
            string link = context.Request.GetDisplayUrl().Replace(context.Request.Path, verificationLink);

            string subject = "Reset password";

            string body = "Dear User, we got a request for resetting your password.<br/>"
                + "Please, click on the link below to reset your password.<br/>"
                + $"<a href='{link}'>*Click*</a>";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendEmailAsync(string email, string subject, string body)
        {
            MailAddress toEmail = new MailAddress(email);
            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true

            })
            {
                smtp.Send(message);
            }
        }
    }
}
