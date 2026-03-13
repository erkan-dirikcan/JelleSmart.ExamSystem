namespace JelleSmart.ExamSystem.Core.Entities
{
    /// <summary>
    /// Ünite (Dersin alt başlıkları)
    /// </summary>
    public class Unit : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        // Foreign keys
        public string? SubjectId { get; set; }
        public string? GradeId { get; set; }

        // Navigation properties
        public Subject? Subject { get; set; }
        public Grade? Grade { get; set; }
        public ICollection<Topic> Topics { get; set; } = new List<Topic>();
        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}
