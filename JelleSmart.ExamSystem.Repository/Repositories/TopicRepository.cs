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

        public async Task<IEnumerable<Topic>> GetByUnitAsync(string unitId)
        {
            return await _dbSet
                .Include(t => t.Unit)
                .Where(t => t.UnitId == unitId && !t.IsDeleted)
                .ToListAsync();
        }

        public async Task<Topic?> GetByIdWithIncludesAsync(string id)
        {
            return await _dbSet
                .Include(t => t.Unit)
                    .ThenInclude(u => u!.Subject)
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
        }

        public async Task<IEnumerable<Topic>> GetAllWithIncludesAsync()
        {
            return await _dbSet
                .Include(t => t.Unit)
                    .ThenInclude(u => u!.Subject)
                .Where(t => !t.IsDeleted)
                .ToListAsync();
        }
    }
}
