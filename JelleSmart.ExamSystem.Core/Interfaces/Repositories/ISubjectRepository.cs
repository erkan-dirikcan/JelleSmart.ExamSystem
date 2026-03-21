using JelleSmart.ExamSystem.Core.Entities;

namespace JelleSmart.ExamSystem.Core.Interfaces.Repositories
{
    public interface ISubjectRepository : IRepository<Subject>
    {
        Task<Subject?> GetWithGradesAsync(string id);
    }
}
