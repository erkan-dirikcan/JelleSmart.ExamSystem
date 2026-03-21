using JelleSmart.ExamSystem.Core.Entities;

namespace JelleSmart.ExamSystem.Core.Interfaces.Repositories
{
    public interface IQuestionRepository : IRepository<Question>
    {
        Task<Question?> GetWithChoicesAsync(string id);
        Task<IEnumerable<Question>> GetByTopicAsync(string topicId);
        Task<IEnumerable<Question>> GetByTeacherAsync(string teacherId);
        Task<IEnumerable<Question>> GetRandomQuestionsAsync(string topicId, int count);
        Task<IEnumerable<Question>> GetByIdsAsync(List<string> questionIds);
    }
}
