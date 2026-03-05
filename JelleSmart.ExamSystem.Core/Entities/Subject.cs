using JelleSmart.ExamSystem.Core.Entities.Identity;

namespace JelleSmart.ExamSystem.Core.Entities
{
    /// <summary>
    /// Ders (Örn: Matematik, Türkçe, Fen Bilimleri)
    /// </summary>
    public class Subject : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? IconClass { get; set; } // CSS icon class (FontAwesome vb.)

        // Navigation properties
        public ICollection<Unit> Units { get; set; } = new List<Unit>();
        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}
