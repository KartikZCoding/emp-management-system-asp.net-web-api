using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration config)
        {
            _configuration = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var myEmail = _configuration["Email:SenderEmail"];
            var appPass = File.ReadAllText(_configuration["Email:Password"]);
            var email = new MailMessage();

            email.From = new MailAddress(myEmail);

            email.To.Add(toEmail);
            email.Subject = subject;
            email.Body = body;

            var smtp = new SmtpClient(_configuration["Email:SmtpServer"], int.Parse(_configuration["Email:Port"]));
            smtp.Credentials = new NetworkCredential(myEmail, appPass);
            smtp.EnableSsl = true;

            await smtp.SendMailAsync(email);
        }
    }
}
