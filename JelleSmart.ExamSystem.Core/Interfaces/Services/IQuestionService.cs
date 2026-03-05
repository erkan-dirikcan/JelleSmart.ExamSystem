using JelleSmart.ExamSystem.Core.Entities;

namespace JelleSmart.ExamSystem.Core.Interfaces.Services
{
    public interface IQuestionService
    {
        Task<IEnumerable<Question>> GetAllAsync();
        Task<Question?> GetByIdAsync(int id);
        Task<Question?> GetWithChoicesAsync(int id);
        Task<IEnumerable<Question>> GetBySubjectAsync(int subjectId);
        Task<IEnumerable<Question>> GetByTeacherAsync(string teacherId);
        Task<Question> CreateAsync(Question question);
        Task UpdateAsync(Question question);
        Task DeleteAsync(int id);
        Task<string> UploadImageAsync(int questionId, string fileName, string contentType, Stream stream);
        Task DeleteImageAsync(int questionId, string imagePath);
    }
}
