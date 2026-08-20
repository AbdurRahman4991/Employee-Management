using System.Net;
using System.Net.Mail;
using MyFirstApi.Services.Interfaces;

namespace MyFirstApi.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(
            string to,
            string subject,
            string body)
        {
            var smtpHost =
                _configuration["Email:SmtpHost"];

            var smtpPort =
                int.Parse(
                    _configuration["Email:SmtpPort"]!
                );

            var username =
                _configuration["Email:Username"];

            var password =
                _configuration["Email:Password"];

            var fromEmail =
                _configuration["Email:FromEmail"];

            using var message = new MailMessage();

            message.From = new MailAddress(
                fromEmail!,
                "MyFirstApi"
            );

            message.To.Add(to);

            message.Subject = subject;

            message.Body = body;

            message.IsBodyHtml = true;

            using var smtpClient =
                new SmtpClient(
                    smtpHost,
                    smtpPort
                );

            smtpClient.EnableSsl = true;

            smtpClient.Credentials =
                new NetworkCredential(
                    username,
                    password
                );

            await smtpClient.SendMailAsync(message);
        }
    }
}