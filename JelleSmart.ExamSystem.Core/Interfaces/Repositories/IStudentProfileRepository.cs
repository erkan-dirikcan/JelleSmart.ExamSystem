using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.Enums;

namespace JelleSmart.ExamSystem.Core.Interfaces.Repositories
{
    public interface IStudentProfileRepository : IRepository<StudentProfile>
    {
        Task<StudentProfile?> GetByUserIdAsync(string userId);
        Task<StudentProfile?> GetWithParentsAsync(string userId);
        Task<StudentProfile?> GetWithGradeAsync(string userId);
        Task<bool> HasParentWithTypeAsync(int studentProfileId, ParentType parentType);
        Task<int> CountParentsAsync(int studentProfileId);
        Task<StudentParent?> GetParentAsync(int parentId);
        Task AddParentAsync(StudentParent parent);
        Task UpdateParentAsync(StudentParent parent);
        Task DeleteParentAsync(int parentId);
        Task AddAsync(StudentProfile entity);
        Task SaveChangesAsync();
    }
}
