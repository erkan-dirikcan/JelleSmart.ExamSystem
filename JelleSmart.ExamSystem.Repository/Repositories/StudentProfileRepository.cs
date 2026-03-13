using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.Interfaces.Repositories;
using JelleSmart.ExamSystem.Core.Enums;
using JelleSmart.ExamSystem.Repository.Data;
using Microsoft.EntityFrameworkCore;

namespace JelleSmart.ExamSystem.Repository.Repositories
{
    public class StudentProfileRepository : Repository<StudentProfile>, IStudentProfileRepository
    {
        private readonly AppDbContext _context;

        public StudentProfileRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<StudentProfile?> GetByUserIdAsync(string userId)
        {
            return await _context.StudentProfiles
                .Include(sp => sp.Grade)
                .Include(sp => sp.Parents)
                .FirstOrDefaultAsync(sp => sp.UserId == userId);
        }

        public async Task<StudentProfile?> GetWithParentsAsync(string userId)
        {
            return await _context.StudentProfiles
                .Include(sp => sp.Parents)
                .FirstOrDefaultAsync(sp => sp.UserId == userId);
        }

        public async Task<StudentProfile?> GetWithGradeAsync(string userId)
        {
            return await _context.StudentProfiles
                .Include(sp => sp.Grade)
                .FirstOrDefaultAsync(sp => sp.UserId == userId);
        }

        public async Task<bool> HasParentWithTypeAsync(string studentProfileId, ParentType parentType)
        {
            return await _context.StudentParents
                .AnyAsync(sp => sp.StudentProfileId == studentProfileId && sp.ParentType == parentType);
        }

        public async Task<int> CountParentsAsync(string studentProfileId)
        {
            return await _context.StudentParents
                .CountAsync(sp => sp.StudentProfileId == studentProfileId);
        }

        public async Task<StudentParent?> GetParentAsync(string parentId)
        {
            return await _context.StudentParents
                .FirstOrDefaultAsync(sp => sp.Id == parentId);
        }

        public async Task AddParentAsync(StudentParent parent)
        {
            await _context.StudentParents.AddAsync(parent);
        }

        public async Task UpdateParentAsync(StudentParent parent)
        {
            _context.StudentParents.Update(parent);
        }

        public async Task DeleteParentAsync(string parentId)
        {
            var parent = await _context.StudentParents
                .FirstOrDefaultAsync(sp => sp.Id == parentId);

            if (parent != null)
            {
                _context.StudentParents.Remove(parent);
            }
        }

        public async Task AddAsync(StudentProfile entity)
        {
            await _context.StudentProfiles.AddAsync(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
