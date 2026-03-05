using JelleSmart.ExamSystem.Core.Entities;

namespace JelleSmart.ExamSystem.Core.Interfaces.Repositories
{
    public interface IUnitRepository : IRepository<Unit>
    {
        Task<Unit?> GetWithTopicsAsync(int id);
        Task<IEnumerable<Unit>> GetBySubjectAsync(int subjectId);
        Task<IEnumerable<Unit>> GetByGradeAsync(int gradeId);
    }
}
