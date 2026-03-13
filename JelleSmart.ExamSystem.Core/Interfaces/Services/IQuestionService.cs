using JelleSmart.ExamSystem.Core.Entities;

namespace JelleSmart.ExamSystem.Core.Interfaces.Services
{
    public interface IQuestionService
    {
        Task<IEnumerable<Question>> GetAllAsync();
        Task<Question?> GetByIdAsync(string id);
        Task<Question?> GetWithChoicesAsync(string id);
        Task<IEnumerable<Question>> GetBySubjectAsync(string subjectId);
        Task<IEnumerable<Question>> GetByTeacherAsync(string teacherId);
        Task<Question> CreateAsync(Question question);
        Task UpdateAsync(Question question);
        Task DeleteAsync(string id);
        Task<string> UploadImageAsync(string questionId, string fileName, string contentType, Stream stream);
        Task DeleteImageAsync(string questionId, string imagePath);
    }
}
