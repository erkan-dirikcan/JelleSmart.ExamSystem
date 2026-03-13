using JelleSmart.ExamSystem.Core.Entities.Identity;

namespace JelleSmart.ExamSystem.Core.Entities
{
    /// <summary>
    /// Öğrenci Cevap (Öğrencinin verdiği cevaplar)
    /// </summary>
    public class StudentAnswer : BaseEntity
    {
        public bool IsCorrect { get; set; } = false;
        public double Points { get; set; } = 0.0;

        // Foreign keys
        public string StudentUserId { get; set; } = null!;
        public string? StudentExamId { get; set; }
        public string? QuestionId { get; set; }
        public string? ChoiceId { get; set; } // Seçilen şık

        // Navigation properties
        public AppUser Student { get; set; } = null!;
        public StudentExam? StudentExam { get; set; }
        public Question? Question { get; set; }
        public Choice? Choice { get; set; }
    }
}
