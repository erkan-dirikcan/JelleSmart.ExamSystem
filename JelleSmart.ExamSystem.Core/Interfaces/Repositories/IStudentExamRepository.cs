using JelleSmart.ExamSystem.Core.Entities;

namespace JelleSmart.ExamSystem.Core.Interfaces.Repositories
{
    public interface IStudentExamRepository : IRepository<StudentExam>
    {
        Task<StudentExam?> GetWithAnswersAsync(int id);
        Task<StudentExam?> GetByStudentAndExamAsync(string studentId, int examId);
        Task<IEnumerable<StudentExam>> GetByStudentAsync(string studentId);
        Task<IEnumerable<StudentExam>> GetByExamAsync(int examId);
    }
}
