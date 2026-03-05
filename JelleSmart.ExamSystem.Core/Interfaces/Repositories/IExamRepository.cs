using JelleSmart.ExamSystem.Core.Entities;

namespace JelleSmart.ExamSystem.Core.Interfaces.Repositories
{
    public interface IExamRepository : IRepository<Exam>
    {
        Task<Exam?> GetWithQuestionsAsync(int id);
        Task<Exam?> GetWithStudentExamsAsync(int id);
        Task<IEnumerable<Exam>> GetByTeacherAsync(string teacherId);
        Task<IEnumerable<Exam>> GetBySubjectAsync(int subjectId);
        Task<IEnumerable<Exam>> GetActiveExamsAsync();
    }
}
