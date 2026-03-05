namespace JelleSmart.ExamSystem.Core.DTOs
{
    public class ExamResultDto
    {
        public int ExamId { get; set; }
        public string ExamName { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public double Score { get; set; }
        public double TotalPoints { get; set; }
        public double Percentage { get; set; }
        public double CorrectCount { get; set; }
        public double WrongCount { get; set; }
        public double EmptyCount { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
