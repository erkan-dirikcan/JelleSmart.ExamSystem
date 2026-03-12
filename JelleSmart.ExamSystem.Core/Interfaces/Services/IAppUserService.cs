namespace JelleSmart.ExamSystem.Core.Interfaces.Services
{
    public interface IAppUserService
    {
        Task<(bool Success, string Error)> GeneratePasswordResetTokenAsync(string email);
        Task<(bool Success, string Error)> ResetPasswordAsync(string email, string token, string newPassword);
        Task<(bool Success, string Error)> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
    }
}
