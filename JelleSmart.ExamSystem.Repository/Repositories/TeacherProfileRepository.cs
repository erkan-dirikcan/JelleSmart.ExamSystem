using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.Interfaces.Repositories;
using JelleSmart.ExamSystem.Repository.Data;
using Microsoft.EntityFrameworkCore;

namespace JelleSmart.ExamSystem.Repository.Repositories
{
    public class TeacherProfileRepository : Repository<TeacherProfile>, ITeacherProfileRepository
    {
        private readonly AppDbContext _context;

        public TeacherProfileRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<TeacherProfile?> GetByUserIdAsync(string userId)
        {
            return await _context.TeacherProfiles
                .Include(tp => tp.Subjects)
                    .ThenInclude(ts => ts.Subject)
                .FirstOrDefaultAsync(tp => tp.UserId == userId);
        }

        public async Task<TeacherProfile?> GetWithSubjectsAsync(string userId)
        {
            return await _context.TeacherProfiles
                .Include(tp => tp.Subjects)
                    .ThenInclude(ts => ts.Subject)
                .FirstOrDefaultAsync(tp => tp.UserId == userId);
        }

        public async Task<bool> HasSubjectAsync(string teacherProfileId, string subjectId)
        {
            return await _context.TeacherSubjects
                .AnyAsync(ts => ts.TeacherProfileId == teacherProfileId && ts.SubjectId == subjectId);
        }

        public async Task AddSubjectAsync(TeacherSubject teacherSubject)
        {
            await _context.TeacherSubjects.AddAsync(teacherSubject);
        }

        public async Task RemoveSubjectAsync(string teacherProfileId, string subjectId)
        {
            var teacherSubject = await _context.TeacherSubjects
                .FirstOrDefaultAsync(ts => ts.TeacherProfileId == teacherProfileId && ts.SubjectId == subjectId);

            if (teacherSubject != null)
            {
                _context.TeacherSubjects.Remove(teacherSubject);
            }
        }

        public async Task AddAsync(TeacherProfile entity)
        {
            await _context.TeacherProfiles.AddAsync(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
