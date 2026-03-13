namespace JelleSmart.ExamSystem.Core.DTOs
{
    public class SubmitAnswerDto
    {
        public string StudentExamId { get; set; } = null!;
        public string QuestionId { get; set; } = null!;
        public string? ChoiceId { get; set; }
        public string StudentUserId { get; set; } = null!;
    }
}
