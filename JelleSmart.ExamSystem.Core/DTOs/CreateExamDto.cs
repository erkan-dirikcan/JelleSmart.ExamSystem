namespace JelleSmart.ExamSystem.Core.DTOs
{
    public class CreateExamDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Duration { get; set; }
        public int GradeId { get; set; }
        public int SubjectId { get; set; }
        public List<int> TopicIds { get; set; } = new();
        public int QuestionCount { get; set; }
        public double TotalPoints { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string CreatedByUserId { get; set; } = null!;
    }
}
