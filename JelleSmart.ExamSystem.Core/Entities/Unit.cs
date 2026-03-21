namespace JelleSmart.ExamSystem.Core.Entities
{
    /// <summary>
    /// Ünite (Dersin alt başlıkları)
    /// Her ünite bir sınıfa aittir
    /// </summary>
    public class Unit : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Order { get; set; } = 1;

        // Foreign keys
        public string GradeId { get; set; } = string.Empty;

        // Navigation properties
        public Grade? Grade { get; set; }
        public ICollection<Topic> Topics { get; set; } = new List<Topic>();
        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}
