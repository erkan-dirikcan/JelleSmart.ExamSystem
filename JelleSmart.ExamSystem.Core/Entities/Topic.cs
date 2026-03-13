namespace JelleSmart.ExamSystem.Core.Entities
{
    /// <summary>
    /// Konu (Ünitenin alt başlıkları)
    /// Öğretmenler soruları konulara göre oluşturur
    /// Sınav oluştururken konu seçilir ve rastgele sorular çekilir
    /// </summary>
    public class Topic : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Code { get; set; } // Konu kodu (Örn: M.1.1.1)

        // Foreign key
        public string? UnitId { get; set; }

        // Navigation properties
        public Unit? Unit { get; set; }
        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}
