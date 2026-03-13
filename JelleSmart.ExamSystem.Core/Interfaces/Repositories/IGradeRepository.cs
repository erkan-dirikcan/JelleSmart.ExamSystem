using JelleSmart.ExamSystem.Core.Entities;

namespace JelleSmart.ExamSystem.Core.Interfaces.Repositories
{
    public interface IGradeRepository : IRepository<Grade>
    {
        Task<Grade?> GetWithUnitsAsync(string id);
        Task<Grade?> GetWithStudentsAsync(string id);
    }
}
