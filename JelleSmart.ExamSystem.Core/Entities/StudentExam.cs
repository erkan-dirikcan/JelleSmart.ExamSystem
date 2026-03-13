using JelleSmart.ExamSystem.Core.Entities.Identity;
using JelleSmart.ExamSystem.Core.Enums;

namespace JelleSmart.ExamSystem.Core.Entities
{
    /// <summary>
    /// Öğrenci Sınav (Öğrencinin sınavı)
    /// </summary>
    public class StudentExam : BaseEntity
    {
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public double Score { get; set; } = 0.0;
        public double CorrectCount { get; set; } = 0.0;
        public double WrongCount { get; set; } = 0.0;
        public double EmptyCount { get; set; } = 0.0;
        public ExamStatus Status { get; set; } = ExamStatus.NotStarted;
        public int? RemainingTime { get; set; } // Kalan süre (saniye)

        // Foreign keys
        public string StudentUserId { get; set; } = null!;
        public string? ExamId { get; set; }

        // Navigation properties
        public AppUser Student { get; set; } = null!;
        public Exam? Exam { get; set; }
        public ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();
    }
}
