using JelleSmart.ExamSystem.Core.Entities;

namespace JelleSmart.ExamSystem.Core.Interfaces.Repositories
{
    public interface IUnitRepository : IRepository<Unit>
    {
        Task<Unit?> GetWithTopicsAsync(string id);
        Task<IEnumerable<Unit>> GetBySubjectAsync(string subjectId);
        Task<IEnumerable<Unit>> GetByGradeAsync(string gradeId);
        Task<Unit?> GetByIdWithIncludesAsync(string id);
        Task<IEnumerable<Unit>> GetAllWithIncludesAsync();
    }
}
