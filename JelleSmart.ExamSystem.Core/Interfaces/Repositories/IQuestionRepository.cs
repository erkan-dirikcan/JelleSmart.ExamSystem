using JelleSmart.ExamSystem.Core.Entities;

namespace JelleSmart.ExamSystem.Core.Interfaces.Repositories
{
    public interface IQuestionRepository : IRepository<Question>
    {
        Task<Question?> GetWithChoicesAsync(int id);
        Task<IEnumerable<Question>> GetBySubjectAsync(int subjectId);
        Task<IEnumerable<Question>> GetByTeacherAsync(string teacherId);
        Task<IEnumerable<Question>> GetRandomQuestionsAsync(int subjectId, int? unitId, int? topicId, int count);
        Task<IEnumerable<Question>> GetByIdsAsync(List<int> questionIds);
    }
}
