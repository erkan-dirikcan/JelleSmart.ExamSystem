using JelleSmart.ExamSystem.Core.Entities.Identity;

namespace JelleSmart.ExamSystem.Core.Interfaces.Repositories
{
    public interface IAppUserRepository
    {
        Task<AppUser?> GetByEmailAsync(string email);
        Task<AppUser?> GetByIdWithProfileAsync(string id);
        Task<bool> IsEmailUniqueAsync(string email, string? excludeUserId = null);
        Task<IEnumerable<AppUser>> GetAllAsync();
        Task<AppUser?> GetByIdAsync(string id);
    }
}
