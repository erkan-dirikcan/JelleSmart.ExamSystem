using JelleSmart.ExamSystem.Core.Entities.Identity;

namespace JelleSmart.ExamSystem.Core.Entities
{
    public class StudentProfile : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public AppUser User { get; set; } = null!;

        public int GradeId { get; set; }
        public Grade Grade { get; set; } = null!;

        public string? StudentNumber { get; set; }
        public DateOnly? EnrollmentDate { get; set; }

        public ICollection<StudentParent> Parents { get; set; } = new List<StudentParent>();
    }
}
