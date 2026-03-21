namespace JelleSmart.ExamSystem.Core.Entities
{
    /// <summary>
    /// Ders-Sınıf ilişkisi (Many-to-Many)
    /// Her dersin hangi sınıflarda öğretildiğini tanımlar
    /// </summary>
    public class SubjectGrade : BaseEntity
    {
        public string SubjectId { get; set; } = string.Empty;
        public string GradeId { get; set; } = string.Empty;

        // Navigation properties
        public Subject? Subject { get; set; }
        public Grade? Grade { get; set; }
    }
}
