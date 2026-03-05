namespace JelleSmart.ExamSystem.Core.DTOs
{
    public class SubmitAnswerDto
    {
        public int StudentExamId { get; set; }
        public int QuestionId { get; set; }
        public int? ChoiceId { get; set; }
        public string StudentUserId { get; set; } = null!;
    }
}
