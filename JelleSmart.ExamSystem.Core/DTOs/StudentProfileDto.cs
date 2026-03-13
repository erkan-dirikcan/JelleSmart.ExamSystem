using JelleSmart.ExamSystem.Core.Enums;

namespace JelleSmart.ExamSystem.Core.DTOs
{
    public class StudentProfileDto
    {
        public string? Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string? GradeId { get; set; }
        public string GradeName { get; set; } = string.Empty;
        public string? StudentNumber { get; set; }
        public DateOnly? EnrollmentDate { get; set; }
        public List<ParentDto> Parents { get; set; } = new();
    }

    public class ParentDto
    {
        public string? Id { get; set; }
        public ParentType ParentType { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? FullName => $"{FirstName} {LastName}".Trim();
    }

    public class CreateStudentProfileDto
    {
        public string? GradeId { get; set; }
        public string? StudentNumber { get; set; }
        public List<CreateParentDto>? Parents { get; set; }
    }

    public class UpdateStudentProfileDto
    {
        public string? GradeId { get; set; }
        public string? StudentNumber { get; set; }
    }

    public class CreateParentDto
    {
        public ParentType ParentType { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
    }

    public class UpdateParentDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
    }
}
