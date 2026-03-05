namespace JelleSmart.ExamSystem.Core.Entities
{
    /// <summary>
    /// Soru Şıkkı (A, B, C, D)
    /// </summary>
    public class Choice : BaseEntity
    {
        public string Label { get; set; } = string.Empty; // A, B, C, D
        public string Text { get; set; } = string.Empty;
        public string? ImageUrl { get; set; } // Şıkta görsel olabilir
        public bool IsCorrect { get; set; } = false;

        // Foreign key
        public int QuestionId { get; set; }

        // Navigation properties
        public Question Question { get; set; } = null!;
        public ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();
    }
}
