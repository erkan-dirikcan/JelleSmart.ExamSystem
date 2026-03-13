using JelleSmart.ExamSystem.Core.Entities.Identity;

namespace JelleSmart.ExamSystem.Core.Entities
{
    /// <summary>
    /// Soru
    /// </summary>
    public class Question : BaseEntity
    {
        public string Text { get; set; } = string.Empty;
        public string? ImageUrl { get; set; } // Soru görseli
        public string? Explanation { get; set; } // Açıklama/Cevap anahtarı notu
        public int Difficulty { get; set; } = 1; // 1: Kolay, 2: Orta, 3: Zor
        public int Order { get; set; } = 0; // Sıralama

        // Foreign keys
        public string? SubjectId { get; set; }
        public string? UnitId { get; set; }
        public string? TopicId { get; set; }
        public string? GradeId { get; set; }
        public string CreatedByUserId { get; set; } = null!;

        // Navigation properties
        public Subject? Subject { get; set; }
        public Unit? Unit { get; set; }
        public Topic? Topic { get; set; }
        public Grade? Grade { get; set; }
        public AppUser CreatedByUser { get; set; } = null!;
        public ICollection<Choice> Choices { get; set; } = new List<Choice>();
        public ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();
    }
}
