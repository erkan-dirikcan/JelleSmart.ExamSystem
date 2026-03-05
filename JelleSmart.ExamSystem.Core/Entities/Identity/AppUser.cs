using Microsoft.AspNetCore.Identity;

namespace JelleSmart.ExamSystem.Core.Entities.Identity
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Öğretmen için branş dersi
        public int? SubjectId { get; set; }
        public Subject? Subject { get; set; }

        // Hangi sınıfa ait?
        public int? GradeId { get; set; }
        public Grade? Grade { get; set; }

        // Navigation properties
        public ICollection<StudentSubject> StudentSubjects { get; set; } = new List<StudentSubject>();
        public ICollection<StudentExam> StudentExams { get; set; } = new List<StudentExam>();
        public ICollection<Question> Questions { get; set; } = new List<Question>();
        public ICollection<Exam> CreatedExams { get; set; } = new List<Exam>();
        public ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();
    }
}
