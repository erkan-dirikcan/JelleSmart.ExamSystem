namespace JelleSmart.ExamSystem.WebUI.Models
{
    public class TakeExamViewModel
    {
        public int StudentExamId { get; set; }
        public string ExamName { get; set; } = string.Empty;
        public int Duration { get; set; }
        public DateTime StartTime { get; set; }
        public int RemainingTime { get; set; }
        public List<QuestionInExamViewModel> Questions { get; set; } = new();
    }

    public class QuestionInExamViewModel
    {
        public int ExamQuestionId { get; set; }
        public int QuestionId { get; set; }
        public int Order { get; set; }
        public string Text { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public List<ChoiceInExamViewModel> Choices { get; set; } = new();
        public int? SelectedChoiceId { get; set; }
    }

    public class ChoiceInExamViewModel
    {
        public int Id { get; set; }
        public string Label { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
    }

    public class SubmitAnswerRequest
    {
        public int StudentExamId { get; set; }
        public int QuestionId { get; set; }
        public int? ChoiceId { get; set; }
    }

    public class ExamResultViewModel
    {
        public Core.DTOs.ExamResultDto Result { get; set; } = null!;
        public List<AnswerDetailViewModel> Answers { get; set; } = new();
    }

    public class AnswerDetailViewModel
    {
        public string QuestionText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public string SelectedChoice { get; set; } = string.Empty;
        public string CorrectChoice { get; set; } = string.Empty;
    }
}
