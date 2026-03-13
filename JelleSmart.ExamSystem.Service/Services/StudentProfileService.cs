using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.DTOs;
using JelleSmart.ExamSystem.Core.Interfaces.Repositories;
using JelleSmart.ExamSystem.Core.Interfaces.Services;
using JelleSmart.ExamSystem.Core.Enums;
using Microsoft.Extensions.Logging;

namespace JelleSmart.ExamSystem.Service.Services
{
    public class StudentProfileService : IStudentProfileService
    {
        private readonly IStudentProfileRepository _studentProfileRepository;
        private readonly ILogger<StudentProfileService> _logger;

        public StudentProfileService(
            IStudentProfileRepository studentProfileRepository,
            ILogger<StudentProfileService> logger)
        {
            _studentProfileRepository = studentProfileRepository;
            _logger = logger;
        }

        public async Task<StudentProfile?> GetByUserIdAsync(string userId)
        {
            return await _studentProfileRepository.GetByUserIdAsync(userId);
        }

        public async Task<StudentProfile?> GetWithParentsAsync(string userId)
        {
            return await _studentProfileRepository.GetWithParentsAsync(userId);
        }

        public async Task<bool> CreateStudentProfileAsync(string userId, CreateStudentProfileDto dto)
        {
            try
            {
                var profile = new StudentProfile
                {
                    UserId = userId,
                    GradeId = dto.GradeId,
                    StudentNumber = dto.StudentNumber,
                    EnrollmentDate = DateOnly.FromDateTime(DateTime.UtcNow)
                };

                await _studentProfileRepository.AddAsync(profile);
                await _studentProfileRepository.SaveChangesAsync();

                // Add parents if provided
                if (dto.Parents != null && dto.Parents.Any())
                {
                    foreach (var parentDto in dto.Parents)
                    {
                        // Validate before adding
                        var (canAdd, error) = await ValidateParentAdditionAsync(profile.Id!, parentDto.ParentType);
                        if (!canAdd)
                        {
                            _logger.LogWarning("Parent addition validation failed during profile creation: {Error}", error);
                            continue;
                        }

                        var parent = new StudentParent
                        {
                            StudentProfileId = profile.Id,
                            ParentType = parentDto.ParentType,
                            FirstName = parentDto.FirstName,
                            LastName = parentDto.LastName,
                            PhoneNumber = parentDto.PhoneNumber,
                            Email = parentDto.Email
                        };

                        await _studentProfileRepository.AddParentAsync(parent);
                    }

                    await _studentProfileRepository.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating student profile for user: {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> UpdateStudentProfileAsync(string userId, UpdateStudentProfileDto dto)
        {
            try
            {
                var profile = await _studentProfileRepository.GetByUserIdAsync(userId);
                if (profile == null)
                    return false;

                if (!string.IsNullOrEmpty(dto.GradeId))
                    profile.GradeId = dto.GradeId;

                if (dto.StudentNumber != null)
                    profile.StudentNumber = dto.StudentNumber;

                await _studentProfileRepository.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating student profile for user: {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> AddParentAsync(string userId, CreateParentDto dto)
        {
            try
            {
                var profile = await _studentProfileRepository.GetByUserIdAsync(userId);
                if (profile == null)
                    return false;

                // Validate: Max 2 parents per student, ParentType must be unique
                var (canAdd, error) = await ValidateParentAdditionAsync(profile.Id!, dto.ParentType);
                if (!canAdd)
                {
                    _logger.LogWarning("Parent addition validation failed: {Error}", error);
                    return false;
                }

                var parent = new StudentParent
                {
                    StudentProfileId = profile.Id,
                    ParentType = dto.ParentType,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    PhoneNumber = dto.PhoneNumber,
                    Email = dto.Email
                };

                await _studentProfileRepository.AddParentAsync(parent);
                await _studentProfileRepository.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding parent for user: {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> UpdateParentAsync(string parentId, UpdateParentDto dto)
        {
            try
            {
                var parent = await _studentProfileRepository.GetParentAsync(parentId);
                if (parent == null)
                    return false;

                if (dto.FirstName != null)
                    parent.FirstName = dto.FirstName;

                if (dto.LastName != null)
                    parent.LastName = dto.LastName;

                if (dto.PhoneNumber != null)
                    parent.PhoneNumber = dto.PhoneNumber;

                if (dto.Email != null)
                    parent.Email = dto.Email;

                await _studentProfileRepository.UpdateParentAsync(parent);
                await _studentProfileRepository.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating parent: {ParentId}", parentId);
                return false;
            }
        }

        public async Task<bool> DeleteParentAsync(string parentId)
        {
            try
            {
                await _studentProfileRepository.DeleteParentAsync(parentId);
                await _studentProfileRepository.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting parent: {ParentId}", parentId);
                return false;
            }
        }

        public async Task<(bool Success, string Error)> ValidateParentAdditionAsync(string studentProfileId, ParentType parentType)
        {
            // Rule 1: Max 2 parents per student
            var parentCount = await _studentProfileRepository.CountParentsAsync(studentProfileId);
            if (parentCount >= 2)
            {
                return (false, "En fazla 2 veli kaydedebilirsiniz.");
            }

            // Rule 2: ParentType must be unique (no duplicate First or Second)
            var existingParent = await _studentProfileRepository.HasParentWithTypeAsync(studentProfileId, parentType);
            if (existingParent)
            {
                return (false, $"Bu veli türü ({GetParentTypeName(parentType)}) zaten kayıtlı.");
            }

            return (true, "");
        }

        private static string GetParentTypeName(ParentType parentType)
        {
            return parentType switch
            {
                ParentType.First => "1. Veli",
                ParentType.Second => "2. Veli",
                _ => "Bilinmeyen"
            };
        }
    }
}
