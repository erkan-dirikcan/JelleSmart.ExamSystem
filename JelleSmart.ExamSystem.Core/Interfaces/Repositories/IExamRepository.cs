using JelleSmart.ExamSystem.Core.Entities;

namespace JelleSmart.ExamSystem.Core.Interfaces.Repositories
{
    public interface IExamRepository : IRepository<Exam>
    {
        Task<Exam?> GetWithQuestionsAsync(string id);
        Task<Exam?> GetWithStudentExamsAsync(string id);
        Task<IEnumerable<Exam>> GetByTeacherAsync(string teacherId);
        Task<IEnumerable<Exam>> GetBySubjectAsync(string subjectId);
        Task<IEnumerable<Exam>> GetActiveExamsAsync();
    }
}
