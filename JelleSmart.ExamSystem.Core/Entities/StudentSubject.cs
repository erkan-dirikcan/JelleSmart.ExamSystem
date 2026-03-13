using JelleSmart.ExamSystem.Core.Entities.Identity;

namespace JelleSmart.ExamSystem.Core.Entities
{
    /// <summary>
    /// Öğrenci Ders İlişkisi (Hangi öğrencinin hangi dersleri seçtiği)
    /// </summary>
    public class StudentSubject : BaseEntity
    {
        // Foreign keys
        public string StudentUserId { get; set; } = null!;
        public string? SubjectId { get; set; }

        // Navigation properties
        public AppUser Student { get; set; } = null!;
        public Subject? Subject { get; set; }
    }
}
