using JelleSmart.ExamSystem.Core.DTOs;

namespace JelleSmart.ExamSystem.Core.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendPasswordResetEmailAsync(string email, string resetLink);
        Task SendResetPasswordEmailAsync(ResetPasswordEmailDto dto);
        Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
    }
}
