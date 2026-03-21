namespace JelleSmart.ExamSystem.Core.ViewModels
{
    public class SubjectGradeViewModel
    {
        public string? Id { get; set; }
        public string SubjectId { get; set; } = string.Empty;
        public string GradeId { get; set; } = string.Empty;

        // For display purposes
        public string? SubjectName { get; set; }
        public string? GradeName { get; set; }
    }
}
