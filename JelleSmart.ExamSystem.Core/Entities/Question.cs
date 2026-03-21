using JelleSmart.ExamSystem.Core.Entities.Identity;

namespace JelleSmart.ExamSystem.Core.Entities
{
    /// <summary>
    /// Soru
    /// Her soru mutlaka bir konuya aittir
    /// </summary>
    public class Question : BaseEntity
    {
        public string Text { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string? Explanation { get; set; }
        public int Difficulty { get; set; } = 1;

        // Foreign keys
        public string TopicId { get; set; } = string.Empty;
        public string CreatedByUserId { get; set; } = string.Empty;

        // Navigation properties
        public Topic? Topic { get; set; }
        public AppUser CreatedByUser { get; set; } = null!;
        public ICollection<Choice> Choices { get; set; } = new List<Choice>();
        public ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();
    }
}
