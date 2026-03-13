namespace JelleSmart.ExamSystem.Core.DTOs
{
    public class StudentPerformanceDto
    {
        public string StudentName { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public int TotalExams { get; set; }
        public int CompletedExams { get; set; }
        public double AverageScore { get; set; }
        public double AveragePercentage { get; set; }
        public List<TopicPerformanceDto> TopicPerformances { get; set; } = new();
    }

    public class TopicPerformanceDto
    {
        public string? TopicId { get; set; }
        public string TopicName { get; set; } = string.Empty;
        public int QuestionCount { get; set; }
        public int CorrectCount { get; set; }
        public double SuccessRate { get; set; }
    }
}
