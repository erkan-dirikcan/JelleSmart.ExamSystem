using JelleSmart.ExamSystem.Core.Entities;

namespace JelleSmart.ExamSystem.Core.Interfaces.Repositories
{
    public interface ITeacherProfileRepository : IRepository<TeacherProfile>
    {
        Task<TeacherProfile?> GetByUserIdAsync(string userId);
        Task<TeacherProfile?> GetWithSubjectsAsync(string userId);
        Task<bool> HasSubjectAsync(int teacherProfileId, int subjectId);
        Task AddSubjectAsync(TeacherSubject teacherSubject);
        Task RemoveSubjectAsync(int teacherProfileId, int subjectId);
        Task AddAsync(TeacherProfile entity);
        Task SaveChangesAsync();
    }
}
