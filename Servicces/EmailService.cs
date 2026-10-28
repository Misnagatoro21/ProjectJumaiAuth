using Microsoft.Extensions.Options;
using ProjectJumaiAuth.Models.Settings;
using ProjectJumaiAuth.Servicces;
using System.Net;
using System.Net.Mail;

namespace ProjectJumaiAuth.Services
{
    public class EmailService(IOptions<SmtpSettings> _SmtpSettings) : IEmailService
    {
        private readonly SmtpSettings _smtp = _SmtpSettings.Value;

        public void SendPasswordResetEmail(string toEmail, string body)
        {
            using var client = new SmtpClient(_smtp.Host, _smtp.Port)
            {
                Credentials = new NetworkCredential(_smtp.User, _smtp.Password),
                EnableSsl = true
            };
            var mail = new MailMessage
            {
                From = new MailAddress(_smtp.User, "Soporte de la autenticacion"),
                Subject = "Token de confirmacion/ Recuperacion",
                Body = body,
                IsBodyHtml = true
            };
            mail.To.Add(toEmail);
            client.Send(mail);
        }
    }
}
