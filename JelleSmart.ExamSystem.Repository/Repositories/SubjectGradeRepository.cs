using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.Interfaces.Repositories;
using JelleSmart.ExamSystem.Repository.Data;
using Microsoft.EntityFrameworkCore;

namespace JelleSmart.ExamSystem.Repository.Repositories
{
    public class SubjectGradeRepository : ISubjectGradeRepository
    {
        private readonly AppDbContext _context;

        public SubjectGradeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SubjectGrade>> GetAllAsync()
        {
            return await _context.SubjectGrades
                .Include(sg => sg.Subject)
                .Include(sg => sg.Grade)
                .ToListAsync();
        }

        public async Task<SubjectGrade?> GetByIdAsync(string subjectId, string gradeId)
        {
            return await _context.SubjectGrades
                .Include(sg => sg.Subject)
                .Include(sg => sg.Grade)
                .FirstOrDefaultAsync(sg => sg.SubjectId == subjectId && sg.GradeId == gradeId);
        }

        public async Task<SubjectGrade?> GetRelationAsync(string subjectId, string gradeId)
        {
            return await _context.SubjectGrades
                .FirstOrDefaultAsync(sg => sg.SubjectId == subjectId && sg.GradeId == gradeId);
        }

        public async Task AddAsync(SubjectGrade subjectGrade)
        {
            await _context.SubjectGrades.AddAsync(subjectGrade);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(SubjectGrade subjectGrade)
        {
            _context.SubjectGrades.Remove(subjectGrade);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(string subjectId, string gradeId)
        {
            return await _context.SubjectGrades
                .AnyAsync(sg => sg.SubjectId == subjectId && sg.GradeId == gradeId);
        }

        public async Task<IEnumerable<Grade>> GetGradesBySubjectIdAsync(string subjectId)
        {
            return await _context.SubjectGrades
                .Where(sg => sg.SubjectId == subjectId)
                .Select(sg => sg.Grade!)
                .OrderBy(g => g.Level)
                .ToListAsync();
        }

        public async Task<IEnumerable<Subject>> GetSubjectsByGradeIdAsync(string gradeId)
        {
            return await _context.SubjectGrades
                .Where(sg => sg.GradeId == gradeId)
                .Select(sg => sg.Subject!)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }
    }
}
