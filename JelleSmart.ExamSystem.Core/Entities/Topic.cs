namespace JelleSmart.ExamSystem.Core.Entities
{
    /// <summary>
    /// Kazanım/Konu (Ünitenin alt başlıkları)
    /// </summary>
    public class Topic : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Code { get; set; } // Kazanım kodu (Örn: M.1.1.1)

        // Foreign key
        public int UnitId { get; set; }

        // Navigation properties
        public Unit Unit { get; set; } = null!;
        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}
