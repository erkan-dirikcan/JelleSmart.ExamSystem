using JelleSmart.ExamSystem.Core.Interfaces.Services;
using JelleSmart.ExamSystem.Core.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace JelleSmart.ExamSystem.Service.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IConfiguration _config;

        public EmailService(ILogger<EmailService> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public async Task SendPasswordResetEmailAsync(string email, string resetLink)
        {
            _logger.LogInformation("PASSWORD RESET EMAIL - To: {Email}, Link: {ResetLink}", email, resetLink);
            string subject = "JelloSmart - Şifre Sıfırlama İstediği";
            string body = $"<p>Şifrenizi sıfırlamak için aşağıdaki linke tıklayınız:</p><p><a href='{resetLink}'>Şifremi Sıfırla</a></p>";
            await SendEmailAsync(email, subject, body, true);
        }

        public async Task SendResetPasswordEmailAsync(ResetPasswordEmailDto dto)
        {
            _logger.LogInformation("PASSWORD RESET EMAIL (DTO) - To: {Email}", dto.Email);
            string subject = "JelloSmart - Şifre Sıfırlama";
            string body = $"<p>Sayın {dto.Name} {dto.Lastname},</p><p>Şifrenizi sıfırlamak için aşağıdaki linke tıklayınız:</p><p><a href='{dto.ResetLink}'>Şifremi Sıfırla</a></p><p><br/>Şifre sıfırlama işlemi talep etmediyseniz bu e-postayı dikkate almayınız.</p>";
            await SendEmailAsync(dto.Email, subject, body, true);
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            _logger.LogInformation("EMAIL - To: {To}, Subject: {Subject}, BodyLength: {BodyLength}", to, subject, body.Length);
            
            try
            {
                var emailSettings = _config.GetSection("EmailSettings");
                var email = new MimeMessage();
                
                string senderName = emailSettings["SenderName"] ?? "JelloSmart";
                string senderEmail = emailSettings["SenderEmail"] ?? "matterapist@jellosmart.com";

                email.From.Add(new MailboxAddress(senderName, senderEmail));
                email.To.Add(new MailboxAddress("", to));
                email.Subject = subject;
                
                var format = isHtml ? TextFormat.Html : TextFormat.Plain;
                email.Body = new TextPart(format) { Text = body };

                using var smtp = new SmtpClient();
                // Port 465 is used for SslOnConnect
                int port = int.TryParse(emailSettings["Port"], out int p) ? p : 465;
                string server = emailSettings["Server"] ?? "mail.jellosmart.com";
                
                await smtp.ConnectAsync(server, port, SecureSocketOptions.SslOnConnect);
                
                string user = emailSettings["UserName"] ?? "matterapist@jellosmart.com";
                string pass = emailSettings["Password"] ?? "As987987!";
                await smtp.AuthenticateAsync(user, pass);
                
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
                
                _logger.LogInformation("Email sent successfully to {To}", to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while sending email to {To}", to);
                throw;
            }
        }
    }
}
