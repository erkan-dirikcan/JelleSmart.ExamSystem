using JelleSmart.ExamSystem.Core.Entities;

namespace JelleSmart.ExamSystem.Core.Interfaces.Repositories
{
    public interface IStudentExamRepository : IRepository<StudentExam>
    {
        Task<StudentExam?> GetWithAnswersAsync(string id);
        Task<StudentExam?> GetByStudentAndExamAsync(string studentId, string examId);
        Task<IEnumerable<StudentExam>> GetByStudentAsync(string studentId);
        Task<IEnumerable<StudentExam>> GetByExamAsync(string examId);
    }
}
