
using System.Net;
using System.Net.Mail;

namespace Demonstration.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly string _email;
        private readonly string _appPassword;

        public EmailSender(IConfiguration configuration)
        {
            _email = configuration["EmailSettings:mail"];
            _appPassword = configuration["EmailSettings:appPassword"];
        }

        public async Task SendEmailAsync(string email, string subject, string context)
        {
            try
            {
                var client = new SmtpClient("smtp.gmail.com", 587)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(_email, _appPassword)
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_email),
                    Subject = subject,
                    Body = context,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);

                Console.WriteLine("Email sent!");
                await client.SendMailAsync(mailMessage);         
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Email send failed: " + ex.Message);
                throw;
            }
        }
    }
}
