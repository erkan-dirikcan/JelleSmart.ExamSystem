using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.Interfaces.Repositories;
using JelleSmart.ExamSystem.Repository.Data;
using Microsoft.EntityFrameworkCore;

namespace JelleSmart.ExamSystem.Repository.Repositories
{
    public class UnitRepository : Repository<Unit>, IUnitRepository
    {
        public UnitRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Unit?> GetWithTopicsAsync(string id)
        {
            return await _dbSet
                .Include(u => u.Topics.Where(t => !t.IsDeleted))
                .Include(u => u.Subject)
                .Include(u => u.Grade)
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        }

        public async Task<IEnumerable<Unit>> GetBySubjectAsync(string subjectId)
        {
            return await _dbSet
                .Include(u => u.Grade)
                .Where(u => u.SubjectId == subjectId && !u.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Unit>> GetByGradeAsync(string gradeId)
        {
            return await _dbSet
                .Include(u => u.Subject)
                .Where(u => u.GradeId == gradeId && !u.IsDeleted)
                .ToListAsync();
        }

        public async Task<Unit?> GetByIdWithIncludesAsync(string id)
        {
            return await _dbSet
                .Include(u => u.Subject)
                .Include(u => u.Grade)
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        }

        public async Task<IEnumerable<Unit>> GetAllWithIncludesAsync()
        {
            return await _dbSet
                .Include(u => u.Subject)
                .Include(u => u.Grade)
                .Where(u => !u.IsDeleted)
                .ToListAsync();
        }
    }
}
