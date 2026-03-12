using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.DTOs;

namespace JelleSmart.ExamSystem.Core.Interfaces.Services
{
    public interface ITeacherProfileService
    {
        Task<TeacherProfile?> GetByUserIdAsync(string userId);
        Task<TeacherProfile?> GetWithSubjectsAsync(string userId);
        Task<bool> CreateTeacherProfileAsync(string userId, CreateTeacherProfileDto dto);
        Task<bool> UpdateTeacherProfileAsync(string userId, UpdateTeacherProfileDto dto);
        Task<bool> AddSubjectAsync(string userId, int subjectId);
        Task<bool> RemoveSubjectAsync(string userId, int subjectId);
        Task<bool> HasSubjectAsync(string userId, int subjectId);
    }
}
