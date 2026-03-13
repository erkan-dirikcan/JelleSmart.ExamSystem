using JelleSmart.ExamSystem.Core.Entities;

namespace JelleSmart.ExamSystem.Core.Interfaces.Repositories
{
    public interface ITeacherProfileRepository : IRepository<TeacherProfile>
    {
        Task<TeacherProfile?> GetByUserIdAsync(string userId);
        Task<TeacherProfile?> GetWithSubjectsAsync(string userId);
        Task<bool> HasSubjectAsync(string teacherProfileId, string subjectId);
        Task AddSubjectAsync(TeacherSubject teacherSubject);
        Task RemoveSubjectAsync(string teacherProfileId, string subjectId);
        Task AddAsync(TeacherProfile entity);
        Task SaveChangesAsync();
    }
}
