using JelleSmart.ExamSystem.Core.Entities;

namespace JelleSmart.ExamSystem.Core.Interfaces.Repositories
{
    public interface IQuestionRepository : IRepository<Question>
    {
        Task<Question?> GetWithChoicesAsync(string id);
        Task<IEnumerable<Question>> GetBySubjectAsync(string subjectId);
        Task<IEnumerable<Question>> GetByTeacherAsync(string teacherId);
        Task<IEnumerable<Question>> GetRandomQuestionsAsync(string subjectId, string? unitId, string? topicId, int count);
        Task<IEnumerable<Question>> GetByIdsAsync(List<string> questionIds);
    }
}
