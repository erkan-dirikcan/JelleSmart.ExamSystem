using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.DTOs;

namespace JelleSmart.ExamSystem.Core.Interfaces.Services
{
    public interface IStudentExamService
    {
        Task<StudentExam?> GetByIdAsync(int id);
        Task<StudentExam?> GetByStudentAndExamAsync(string studentId, int examId);
        Task<StudentExam?> GetWithAnswersAsync(int id);
        Task<IEnumerable<StudentExam>> GetByStudentAsync(string studentId);
        Task<StudentExam> StartExamAsync(int examId, string studentId);
        Task SubmitAnswerAsync(SubmitAnswerDto dto);
        Task<StudentExam> CompleteExamAsync(int studentExamId);
        Task<ExamResultDto> GetExamResultAsync(int studentExamId);
    }
}
