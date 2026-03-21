namespace JelleSmart.ExamSystem.Core.Entities
{
    /// <summary>
    /// Konu (Ünitenin alt başlıkları)
    /// </summary>
    public class Topic : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Code { get; set; }
        public int Order { get; set; } = 1;

        // Foreign keys
        public string UnitId { get; set; } = string.Empty;
        public string GradeId { get; set; } = string.Empty;

        // Navigation properties
        public Unit? Unit { get; set; }
        public Grade? Grade { get; set; }
        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}
