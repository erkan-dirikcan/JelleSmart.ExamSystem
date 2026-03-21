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

        public async Task<Subject?> GetWithGradesAsync(string id)
        {
            return await _dbSet
                .Include(s => s.SubjectGrades)
                    .ThenInclude(sg => sg.Grade)
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
        }
    }
}
