using JelleSmart.ExamSystem.Core.Entities;

namespace JelleSmart.ExamSystem.Core.Interfaces.Repositories
{
    public interface IGradeRepository : IRepository<Grade>
    {
        Task<Grade?> GetWithUnitsAsync(int id);
        Task<Grade?> GetWithStudentsAsync(int id);
    }
}
