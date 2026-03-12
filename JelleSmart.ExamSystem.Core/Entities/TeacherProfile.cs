using JelleSmart.ExamSystem.Core.Entities.Identity;

namespace JelleSmart.ExamSystem.Core.Entities
{
    public class TeacherProfile : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public AppUser User { get; set; } = null!;

        public ICollection<TeacherSubject> Subjects { get; set; } = new List<TeacherSubject>();
        public string? Title { get; set; }
        public string? Department { get; set; }
        public DateOnly? HireDate { get; set; }
    }
}
