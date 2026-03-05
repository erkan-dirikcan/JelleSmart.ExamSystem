using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.Interfaces.Repositories;
using JelleSmart.ExamSystem.Repository.Data;
using Microsoft.EntityFrameworkCore;

namespace JelleSmart.ExamSystem.Repository.Repositories
{
    public class TopicRepository : Repository<Topic>, ITopicRepository
    {
        public TopicRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Topic>> GetByUnitAsync(int unitId)
        {
            return await _dbSet
                .Include(t => t.Unit)
                .Where(t => t.UnitId == unitId && !t.IsDeleted)
                .ToListAsync();
        }
    }
}
