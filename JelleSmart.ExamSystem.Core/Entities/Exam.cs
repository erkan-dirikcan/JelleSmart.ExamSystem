using JelleSmart.ExamSystem.Core.Entities.Identity;
using JelleSmart.ExamSystem.Core.Enums;

namespace JelleSmart.ExamSystem.Core.Entities
{
    /// <summary>
    /// Sınav
    /// </summary>
    public class Exam : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Duration { get; set; } // Dakika cinsinden
        public int QuestionCount { get; set; } // Toplam soru sayısı
        public double TotalPoints { get; set; } // Toplam puan
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsActive { get; set; } = false;
        public ExamStatus Status { get; set; } = ExamStatus.NotStarted;

        // Hangi sınıf için?
        public string? GradeId { get; set; }

        // Hangi ders için?
        public string? SubjectId { get; set; }

        // Hangi kazanımlardan? (JSON olarak saklanabilir)
        public string? TopicIds { get; set; }

        // Sınavı oluşturan öğretmen
        public string CreatedByUserId { get; set; } = null!;

        // Navigation properties
        public Grade? Grade { get; set; }
        public Subject? Subject { get; set; }
        public AppUser CreatedByUser { get; set; } = null!;
        public ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();
        public ICollection<StudentExam> StudentExams { get; set; } = new List<StudentExam>();
    }
}
