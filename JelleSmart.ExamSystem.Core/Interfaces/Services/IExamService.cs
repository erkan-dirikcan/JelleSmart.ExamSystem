using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.DTOs;

namespace JelleSmart.ExamSystem.Core.Interfaces.Services
{
    public interface IExamService
    {
        Task<IEnumerable<Exam>> GetAllAsync();
        Task<Exam?> GetByIdAsync(string id);
        Task<Exam?> GetWithQuestionsAsync(string id);
        Task<IEnumerable<Exam>> GetByTeacherAsync(string teacherId);
        Task<Exam> CreateAsync(CreateExamDto dto);
        Task UpdateAsync(Exam exam);
        Task DeleteAsync(string id);
        Task ActivateExamAsync(string examId);
        Task DeactivateExamAsync(string examId);
        Task<Exam?> GetExamForStudentAsync(string examId, string studentId);
    }
}
