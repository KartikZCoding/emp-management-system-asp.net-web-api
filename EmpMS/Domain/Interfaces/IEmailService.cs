using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
