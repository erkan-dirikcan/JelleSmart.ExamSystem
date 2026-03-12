using JelleSmart.ExamSystem.Core.Enums;

namespace JelleSmart.ExamSystem.Core.DTOs
{
    public class CreateAppUserDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        // Teacher için
        public List<int>? SubjectIds { get; set; }
        public string? Title { get; set; }
        public string? Department { get; set; }

        // Student için
        public int? GradeId { get; set; }
        public string? StudentNumber { get; set; }
        public List<CreateParentDto>? Parents { get; set; }
    }
}
