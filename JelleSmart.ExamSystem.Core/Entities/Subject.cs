namespace JelleSmart.ExamSystem.Core.Entities
{
    /// <summary>
    /// Ders (Örn: Matematik, Türkçe, Fen Bilimleri)
    /// </summary>
    public class Subject : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? IconClass { get; set; }

        // Navigation properties
        public ICollection<SubjectGrade> SubjectGrades { get; set; } = new List<SubjectGrade>();
    }
}
