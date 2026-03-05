using JelleSmart.ExamSystem.Core.Entities.Identity;

namespace JelleSmart.ExamSystem.Core.Entities
{
    /// <summary>
    /// Sınıf (1, 2, 3, 4)
    /// </summary>
    public class Grade : BaseEntity
    {
        public int Level { get; set; } // 1, 2, 3, 4
        public string Name { get; set; } = string.Empty; // "1. Sınıf", "2. Sınıf" vb.

        // Navigation properties
        public ICollection<Unit> Units { get; set; } = new List<Unit>();
        public ICollection<AppUser> Students { get; set; } = new List<AppUser>();
    }
}
