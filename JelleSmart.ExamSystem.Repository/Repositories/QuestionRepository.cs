using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.Interfaces.Repositories;
using JelleSmart.ExamSystem.Repository.Data;
using Microsoft.EntityFrameworkCore;

namespace JelleSmart.ExamSystem.Repository.Repositories
{
    public class QuestionRepository : Repository<Question>, IQuestionRepository
    {
        public QuestionRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Question?> GetWithChoicesAsync(string id)
        {
            return await _dbSet
                .Include(q => q.Choices.Where(c => !c.IsDeleted))
                .Include(q => q.Topic)
                    .ThenInclude(t => t!.Unit)
                        .ThenInclude(u => u!.Grade)
                .Include(q => q.CreatedByUser)
                .FirstOrDefaultAsync(q => q.Id == id && !q.IsDeleted);
        }

        public async Task<IEnumerable<Question>> GetByTopicAsync(string topicId)
        {
            return await _dbSet
                .Include(q => q.Choices.Where(c => !c.IsDeleted))
                .Include(q => q.Topic)
                .Where(q => q.TopicId == topicId && !q.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Question>> GetByTeacherAsync(string teacherId)
        {
            return await _dbSet
                .Include(q => q.Choices.Where(c => !c.IsDeleted))
                .Include(q => q.Topic)
                    .ThenInclude(t => t!.Unit)
                .Where(q => q.CreatedByUserId == teacherId && !q.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Question>> GetRandomQuestionsAsync(string topicId, int count)
        {
            var query = _dbSet
                .Include(q => q.Choices.Where(c => !c.IsDeleted))
                .Where(q => q.TopicId == topicId && !q.IsDeleted);

            return await query.OrderBy(q => EF.Functions.Random()).Take(count).ToListAsync();
        }

        public async Task<IEnumerable<Question>> GetByIdsAsync(List<string> questionIds)
        {
            return await _dbSet
                .Include(q => q.Choices.Where(c => !c.IsDeleted))
                .Where(q => questionIds.Contains(q.Id!) && !q.IsDeleted)
                .ToListAsync();
        }
    }
}
