using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.Interfaces.Repositories;
using JelleSmart.ExamSystem.Repository.Data;
using Microsoft.EntityFrameworkCore;

namespace JelleSmart.ExamSystem.Repository.Repositories
{
    public class SubjectRepository : Repository<Subject>, ISubjectRepository
    {
        public SubjectRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Subject?> GetWithUnitsAsync(string id)
        {
            return await _dbSet
                .Include(s => s.Units.Where(u => !u.IsDeleted))
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
        }

        public async Task<Subject?> GetWithQuestionsAsync(string id)
        {
            return await _dbSet
                .Include(s => s.Questions.Where(q => !q.IsDeleted))
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
        }
    }
}
