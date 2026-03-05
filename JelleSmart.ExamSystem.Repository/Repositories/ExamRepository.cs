using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.Interfaces.Repositories;
using JelleSmart.ExamSystem.Repository.Data;
using Microsoft.EntityFrameworkCore;

namespace JelleSmart.ExamSystem.Repository.Repositories
{
    public class ExamRepository : Repository<Exam>, IExamRepository
    {
        public ExamRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Exam?> GetWithQuestionsAsync(int id)
        {
            return await _context.Exams
                .Include(e => e.ExamQuestions.Where(eq => !eq.IsDeleted))
                    .ThenInclude(eq => eq.Question)
                        .ThenInclude(q => q.Choices.Where(c => !c.IsDeleted))
                .Include(e => e.Subject)
                .Include(e => e.Grade)
                .Include(e => e.CreatedByUser)
                .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
        }

        public async Task<Exam?> GetWithStudentExamsAsync(int id)
        {
            return await _context.Exams
                .Include(e => e.StudentExams.Where(se => !se.IsDeleted))
                .Include(e => e.Subject)
                .Include(e => e.Grade)
                .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
        }

        public async Task<IEnumerable<Exam>> GetByTeacherAsync(string teacherId)
        {
            return await _context.Exams
                .Include(e => e.Subject)
                .Include(e => e.Grade)
                .Where(e => e.CreatedByUserId == teacherId && !e.IsDeleted)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Exam>> GetBySubjectAsync(int subjectId)
        {
            return await _context.Exams
                .Include(e => e.Subject)
                .Include(e => e.Grade)
                .Where(e => e.SubjectId == subjectId && !e.IsDeleted)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Exam>> GetActiveExamsAsync()
        {
            return await _context.Exams
                .Include(e => e.Subject)
                .Include(e => e.Grade)
                .Where(e => e.IsActive && !e.IsDeleted)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }
    }
}
