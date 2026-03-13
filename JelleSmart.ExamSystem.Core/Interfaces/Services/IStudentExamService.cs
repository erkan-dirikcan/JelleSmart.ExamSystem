using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.DTOs;

namespace JelleSmart.ExamSystem.Core.Interfaces.Services
{
    public interface IStudentExamService
    {
        Task<StudentExam?> GetByIdAsync(string id);
        Task<StudentExam?> GetByStudentAndExamAsync(string studentId, string examId);
        Task<StudentExam?> GetWithAnswersAsync(string id);
        Task<IEnumerable<StudentExam>> GetByStudentAsync(string studentId);
        Task<StudentExam> StartExamAsync(string examId, string studentId);
        Task SubmitAnswerAsync(SubmitAnswerDto dto);
        Task<StudentExam> CompleteExamAsync(string studentExamId);
        Task<ExamResultDto> GetExamResultAsync(string studentExamId);
    }
}
