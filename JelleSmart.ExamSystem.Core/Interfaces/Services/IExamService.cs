using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.DTOs;

namespace JelleSmart.ExamSystem.Core.Interfaces.Services
{
    public interface IExamService
    {
        Task<IEnumerable<Exam>> GetAllAsync();
        Task<Exam?> GetByIdAsync(int id);
        Task<Exam?> GetWithQuestionsAsync(int id);
        Task<IEnumerable<Exam>> GetByTeacherAsync(string teacherId);
        Task<Exam> CreateAsync(CreateExamDto dto);
        Task UpdateAsync(Exam exam);
        Task DeleteAsync(int id);
        Task ActivateExamAsync(int examId);
        Task DeactivateExamAsync(int examId);
        Task<Exam?> GetExamForStudentAsync(int examId, string studentId);
    }
}
