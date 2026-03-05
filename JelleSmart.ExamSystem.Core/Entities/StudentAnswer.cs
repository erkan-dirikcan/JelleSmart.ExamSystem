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
        public int StudentExamId { get; set; }
        public int QuestionId { get; set; }
        public int? ChoiceId { get; set; } // Seçilen şık

        // Navigation properties
        public AppUser Student { get; set; } = null!;
        public StudentExam StudentExam { get; set; } = null!;
        public Question Question { get; set; } = null!;
        public Choice? Choice { get; set; }
    }
}
