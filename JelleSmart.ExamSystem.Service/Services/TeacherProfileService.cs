using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.DTOs;
using JelleSmart.ExamSystem.Core.Interfaces.Repositories;
using JelleSmart.ExamSystem.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace JelleSmart.ExamSystem.Service.Services
{
    public class TeacherProfileService : ITeacherProfileService
    {
        private readonly ITeacherProfileRepository _teacherProfileRepository;
        private readonly ISubjectRepository _subjectRepository;
        private readonly ILogger<TeacherProfileService> _logger;

        public TeacherProfileService(
            ITeacherProfileRepository teacherProfileRepository,
            ISubjectRepository subjectRepository,
            ILogger<TeacherProfileService> logger)
        {
            _teacherProfileRepository = teacherProfileRepository;
            _subjectRepository = subjectRepository;
            _logger = logger;
        }

        public async Task<TeacherProfile?> GetByUserIdAsync(string userId)
        {
            return await _teacherProfileRepository.GetByUserIdAsync(userId);
        }

        public async Task<TeacherProfile?> GetWithSubjectsAsync(string userId)
        {
            return await _teacherProfileRepository.GetWithSubjectsAsync(userId);
        }

        public async Task<bool> CreateTeacherProfileAsync(string userId, CreateTeacherProfileDto dto)
        {
            try
            {
                var profile = new TeacherProfile
                {
                    UserId = userId,
                    Title = dto.Title,
                    Department = dto.Department,
                    HireDate = DateOnly.FromDateTime(DateTime.UtcNow)
                };

                await _teacherProfileRepository.AddAsync(profile);
                await _teacherProfileRepository.SaveChangesAsync();

                // Add subjects
                foreach (var subjectId in dto.SubjectIds)
                {
                    var subject = await _subjectRepository.GetByIdAsync(subjectId);
                    if (subject != null && !await _teacherProfileRepository.HasSubjectAsync(profile.Id!, subjectId))
                    {
                        var teacherSubject = new TeacherSubject
                        {
                            TeacherProfileId = profile.Id,
                            SubjectId = subjectId
                        };

                        await _teacherProfileRepository.AddSubjectAsync(teacherSubject);
                    }
                }

                await _teacherProfileRepository.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating teacher profile for user: {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> UpdateTeacherProfileAsync(string userId, UpdateTeacherProfileDto dto)
        {
            try
            {
                var profile = await _teacherProfileRepository.GetByUserIdAsync(userId);
                if (profile == null)
                    return false;

                if (dto.Title != null)
                    profile.Title = dto.Title;

                if (dto.Department != null)
                    profile.Department = dto.Department;

                await _teacherProfileRepository.SaveChangesAsync();

                // Update subjects if provided
                if (dto.SubjectIds != null)
                {
                    // Get current subjects
                    var currentProfile = await _teacherProfileRepository.GetWithSubjectsAsync(userId);
                    if (currentProfile != null)
                    {
                        // Remove subjects that are not in the new list
                        var currentSubjectIds = currentProfile.Subjects.Select(ts => ts.SubjectId).ToList();
                        foreach (var currentSubjectId in currentSubjectIds)
                        {
                            if (!dto.SubjectIds.Contains(currentSubjectId!))
                            {
                                await _teacherProfileRepository.RemoveSubjectAsync(profile.Id!, currentSubjectId!);
                            }
                        }

                        // Add new subjects
                        foreach (var subjectId in dto.SubjectIds)
                        {
                            if (!currentSubjectIds.Contains(subjectId))
                            {
                                var subject = await _subjectRepository.GetByIdAsync(subjectId);
                                if (subject != null)
                                {
                                    var teacherSubject = new TeacherSubject
                                    {
                                        TeacherProfileId = profile.Id,
                                        SubjectId = subjectId
                                    };

                                    await _teacherProfileRepository.AddSubjectAsync(teacherSubject);
                                }
                            }
                        }

                        await _teacherProfileRepository.SaveChangesAsync();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating teacher profile for user: {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> AddSubjectAsync(string userId, string subjectId)
        {
            try
            {
                var profile = await _teacherProfileRepository.GetByUserIdAsync(userId);
                if (profile == null)
                    return false;

                // Verify subject exists
                var subject = await _subjectRepository.GetByIdAsync(subjectId);
                if (subject == null)
                {
                    _logger.LogWarning("Subject {SubjectId} not found", subjectId);
                    return false;
                }

                if (await _teacherProfileRepository.HasSubjectAsync(profile.Id!, subjectId))
                    return true; // Already has the subject

                var teacherSubject = new TeacherSubject
                {
                    TeacherProfileId = profile.Id,
                    SubjectId = subjectId
                };

                await _teacherProfileRepository.AddSubjectAsync(teacherSubject);
                await _teacherProfileRepository.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding subject {SubjectId} for user: {UserId}", subjectId, userId);
                return false;
            }
        }

        public async Task<bool> RemoveSubjectAsync(string userId, string subjectId)
        {
            try
            {
                var profile = await _teacherProfileRepository.GetByUserIdAsync(userId);
                if (profile == null)
                    return false;

                await _teacherProfileRepository.RemoveSubjectAsync(profile.Id!, subjectId);
                await _teacherProfileRepository.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing subject {SubjectId} for user: {UserId}", subjectId, userId);
                return false;
            }
        }

        public async Task<bool> HasSubjectAsync(string userId, string subjectId)
        {
            var profile = await _teacherProfileRepository.GetByUserIdAsync(userId);
            if (profile == null)
                return false;

            return await _teacherProfileRepository.HasSubjectAsync(profile.Id!, subjectId);
        }
    }
}
