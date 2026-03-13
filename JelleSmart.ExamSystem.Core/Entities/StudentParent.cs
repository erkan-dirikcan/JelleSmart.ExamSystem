using JelleSmart.ExamSystem.Core.Enums;

namespace JelleSmart.ExamSystem.Core.Entities
{
    public class StudentParent : BaseEntity
    {
        public string? StudentProfileId { get; set; }
        public StudentProfile? StudentProfile { get; set; }

        public ParentType ParentType { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
    }
}
