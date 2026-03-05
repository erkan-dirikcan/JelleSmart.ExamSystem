namespace JelleSmart.ExamSystem.Core.DTOs
{
    public class ExamDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Duration { get; set; }
        public int QuestionCount { get; set; }
        public double TotalPoints { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsActive { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public string GradeName { get; set; } = string.Empty;
    }
}
