using JelleSmart.ExamSystem.Core.Entities.Identity;
using JelleSmart.ExamSystem.Core.Interfaces.Repositories;
using JelleSmart.ExamSystem.Repository.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JelleSmart.ExamSystem.Repository.Repositories
{
    public class AppUserRepository : IAppUserRepository
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public AppUserRepository(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<AppUser?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<AppUser?> GetByIdWithProfileAsync(string id)
        {
            return await _context.Users
                .Include(u => u.TeacherProfile)
                    .ThenInclude(tp => tp.Subjects)
                        .ThenInclude(ts => ts.Subject)
                .Include(u => u.StudentProfile)
                    .ThenInclude(sp => sp.Grade)
                .Include(u => u.StudentProfile)
                    .ThenInclude(sp => sp.Parents)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<bool> IsEmailUniqueAsync(string email, string? excludeUserId = null)
        {
            var query = _context.Users.Where(u => u.Email == email);

            if (excludeUserId != null)
            {
                query = query.Where(u => u.Id != excludeUserId);
            }

            return !await query.AnyAsync();
        }

        public async Task<IEnumerable<AppUser>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<AppUser?> GetByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }
    }
}
