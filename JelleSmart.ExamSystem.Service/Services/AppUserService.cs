using JelleSmart.ExamSystem.Core.Entities.Identity;
using JelleSmart.ExamSystem.Core.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Net;

namespace JelleSmart.ExamSystem.Service.Services
{
    public class AppUserService : IAppUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly ILogger<AppUserService> _logger;

        public AppUserService(
            UserManager<AppUser> userManager,
            IEmailService emailService,
            ILogger<AppUserService> logger)
        {
            _userManager = userManager;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<(bool Success, string Error)> GeneratePasswordResetTokenAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);

                // Security: Don't reveal if user exists or not
                if (user == null)
                {
                    _logger.LogWarning("Password reset requested for non-existent email: {Email}", email);
                    return (true, "");
                }

                if (!user.IsActive)
                {
                    _logger.LogWarning("Password reset requested for inactive user: {Email}", email);
                    // Still return success to avoid user enumeration
                    return (true, "");
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encodedEmail = WebUtility.UrlEncode(email);
                var encodedToken = WebUtility.UrlEncode(token);
                var resetLink = $"/Account/ResetPassword?email={encodedEmail}&token={encodedToken}";

                await _emailService.SendPasswordResetEmailAsync(email, resetLink);

                _logger.LogInformation("Password reset token generated for: {Email}", email);
                return (true, "");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating password reset token for: {Email}", email);
                return (false, "Bir hata oluştu. Lütfen daha sonra tekrar deneyiniz.");
            }
        }

        public async Task<(bool Success, string Error)> ResetPasswordAsync(string email, string token, string newPassword)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    return (false, "Geçersiz işlem.");
                }

                if (!user.IsActive)
                {
                    return (false, "Hesabınız pasif durumda. Lütfen yönetici ile iletişime geçin.");
                }

                var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogWarning("Password reset failed for {Email}: {Errors}", email, errors);
                    return (false, errors);
                }

                _logger.LogInformation("Password reset successful for: {Email}", email);
                return (true, "");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for: {Email}", email);
                return (false, "Şifre sıfırlama sırasında bir hata oluştu.");
            }
        }

        public async Task<(bool Success, string Error)> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return (false, "Kullanıcı bulunamadı.");
                }

                if (!user.IsActive)
                {
                    return (false, "Hesabınız pasif durumda.");
                }

                var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return (false, errors);
                }

                _logger.LogInformation("Password changed successfully for user: {UserId}", userId);
                return (true, "");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user: {UserId}", userId);
                return (false, "Şifre değiştirme sırasında bir hata oluştu.");
            }
        }
    }
}
