using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.Interfaces.Repositories;
using JelleSmart.ExamSystem.Repository.Data;
using Microsoft.EntityFrameworkCore;

namespace JelleSmart.ExamSystem.Repository.Repositories
{
    public class StudentExamRepository : Repository<StudentExam>, IStudentExamRepository
    {
        public StudentExamRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<StudentExam?> GetWithAnswersAsync(int id)
        {
            return await _context.StudentExams
                .Include(se => se.StudentAnswers.Where(sa => !sa.IsDeleted))
                    .ThenInclude(sa => sa.Choice)
                .Include(se => se.StudentAnswers)
                    .ThenInclude(sa => sa.Question)
                        .ThenInclude(q => q.Choices.Where(c => !c.IsDeleted))
                .Include(se => se.Exam)
                .Include(se => se.Student)
                .FirstOrDefaultAsync(se => se.Id == id && !se.IsDeleted);
        }

        public async Task<StudentExam?> GetByStudentAndExamAsync(string studentId, int examId)
        {
            return await _context.StudentExams
                .Include(se => se.Exam)
                .FirstOrDefaultAsync(se => se.StudentUserId == studentId && se.ExamId == examId && !se.IsDeleted);
        }

        public async Task<IEnumerable<StudentExam>> GetByStudentAsync(string studentId)
        {
            return await _context.StudentExams
                .Include(se => se.Exam)
                    .ThenInclude(e => e.Subject)
                .Where(se => se.StudentUserId == studentId && !se.IsDeleted)
                .OrderByDescending(se => se.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<StudentExam>> GetByExamAsync(int examId)
        {
            return await _context.StudentExams
                .Include(se => se.Student)
                .Where(se => se.ExamId == examId && !se.IsDeleted)
                .ToListAsync();
        }
    }
}
