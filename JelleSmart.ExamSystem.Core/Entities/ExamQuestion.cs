namespace JelleSmart.ExamSystem.Core.Entities
{
    /// <summary>
    /// Sınav Soru İlişkisi (Sınavın hangi soruları içerdiğini belirtir)
    /// </summary>
    public class ExamQuestion : BaseEntity
    {
        public int Order { get; set; } // Sınavdaki sırası
        public double Points { get; set; } = 1.0; // Bu sorunun puanı

        // Foreign keys
        public string? ExamId { get; set; }
        public string? QuestionId { get; set; }

        // Navigation properties
        public Exam? Exam { get; set; }
        public Question? Question { get; set; }
    }
}
