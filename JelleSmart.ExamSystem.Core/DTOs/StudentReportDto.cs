namespace JelleSmart.ExamSystem.Core.DTOs
{
    public class StudentReportDto
    {
        public string StudentName { get; set; } = string.Empty;
        public string GradeName { get; set; } = string.Empty;
        public int TotalExams { get; set; }
        public int CompletedExams { get; set; }
        public double AverageScore { get; set; }
        public double AveragePercentage { get; set; }
        public List<SubjectPerformanceDto> SubjectPerformances { get; set; } = new();
        public List<ExamResultDto> ExamResults { get; set; } = new();
    }

    public class SubjectPerformanceDto
    {
        public string? SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public double AverageScore { get; set; }
        public int ExamCount { get; set; }
        public double AveragePercentage { get; set; }
    }
}
