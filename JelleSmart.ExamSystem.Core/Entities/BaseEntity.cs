namespace JelleSmart.ExamSystem.Core.Entities
{
    public abstract class BaseEntity
    {
        public string? Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
