using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.Enums;

namespace JelleSmart.ExamSystem.Core.Interfaces.Repositories
{
    public interface IStudentProfileRepository : IRepository<StudentProfile>
    {
        Task<StudentProfile?> GetByUserIdAsync(string userId);
        Task<StudentProfile?> GetWithParentsAsync(string userId);
        Task<StudentProfile?> GetWithGradeAsync(string userId);
        Task<bool> HasParentWithTypeAsync(string studentProfileId, ParentType parentType);
        Task<int> CountParentsAsync(string studentProfileId);
        Task<StudentParent?> GetParentAsync(string parentId);
        Task AddParentAsync(StudentParent parent);
        Task UpdateParentAsync(StudentParent parent);
        Task DeleteParentAsync(string parentId);
        Task AddAsync(StudentProfile entity);
        Task SaveChangesAsync();
    }
}
