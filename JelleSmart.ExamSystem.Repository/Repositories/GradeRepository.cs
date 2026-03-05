using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.Interfaces.Repositories;
using JelleSmart.ExamSystem.Repository.Data;
using Microsoft.EntityFrameworkCore;

namespace JelleSmart.ExamSystem.Repository.Repositories
{
    public class GradeRepository : Repository<Grade>, IGradeRepository
    {
        public GradeRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Grade?> GetWithUnitsAsync(int id)
        {
            return await _dbSet
                .Include(g => g.Units.Where(u => !u.IsDeleted))
                .FirstOrDefaultAsync(g => g.Id == id && !g.IsDeleted);
        }

        public async Task<Grade?> GetWithStudentsAsync(int id)
        {
            return await _dbSet
                .Include(g => g.Students)
                .FirstOrDefaultAsync(g => g.Id == id && !g.IsDeleted);
        }
    }
}
