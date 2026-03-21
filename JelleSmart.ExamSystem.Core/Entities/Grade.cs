using JelleSmart.ExamSystem.Core.Entities.Identity;

namespace JelleSmart.ExamSystem.Core.Entities
{
    /// <summary>
    /// Sınıf (1, 2, 3, 4)
    /// </summary>
    public class Grade : BaseEntity
    {
        public int Level { get; set; }
        public string Name { get; set; } = string.Empty;

        // Navigation properties
        public ICollection<SubjectGrade> SubjectGrades { get; set; } = new List<SubjectGrade>();
        public ICollection<Unit> Units { get; set; } = new List<Unit>();
        public ICollection<AppUser> Students { get; set; } = new List<AppUser>();
    }
}
