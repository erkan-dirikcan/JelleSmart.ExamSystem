using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.DTOs;
using JelleSmart.ExamSystem.Core.Enums;

namespace JelleSmart.ExamSystem.Core.Interfaces.Services
{
    public interface IStudentProfileService
    {
        Task<StudentProfile?> GetByUserIdAsync(string userId);
        Task<StudentProfile?> GetWithParentsAsync(string userId);
        Task<bool> CreateStudentProfileAsync(string userId, CreateStudentProfileDto dto);
        Task<bool> UpdateStudentProfileAsync(string userId, UpdateStudentProfileDto dto);
        Task<bool> AddParentAsync(string userId, CreateParentDto dto);
        Task<bool> UpdateParentAsync(int parentId, UpdateParentDto dto);
        Task<bool> DeleteParentAsync(int parentId);
        Task<(bool Success, string Error)> ValidateParentAdditionAsync(int studentProfileId, ParentType parentType);
    }
}
