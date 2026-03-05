using JelleSmart.ExamSystem.Core.Entities;

namespace JelleSmart.ExamSystem.Core.Interfaces.Services
{
    public interface IGradeService
    {
        Task<IEnumerable<Grade>> GetAllAsync();
        Task<Grade?> GetByIdAsync(int id);
        Task<Grade> CreateAsync(Grade grade);
        Task UpdateAsync(Grade grade);
        Task DeleteAsync(int id);
    }
}
