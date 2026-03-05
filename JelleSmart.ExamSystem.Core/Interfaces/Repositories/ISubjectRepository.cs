using JelleSmart.ExamSystem.Core.Entities;

namespace JelleSmart.ExamSystem.Core.Interfaces.Repositories
{
    public interface ISubjectRepository : IRepository<Subject>
    {
        Task<Subject?> GetWithUnitsAsync(int id);
        Task<Subject?> GetWithQuestionsAsync(int id);
    }
}
