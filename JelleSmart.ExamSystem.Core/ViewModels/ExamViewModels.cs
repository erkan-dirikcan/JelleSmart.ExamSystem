namespace JelleSmart.ExamSystem.Core.ViewModels
{
    public class CreateExamViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Duration { get; set; } = 30;
        public string? GradeId { get; set; }
        public string? SubjectId { get; set; }
        public List<string?>? TopicIds { get; set; }
        public int QuestionCount { get; set; } = 10;
        public double TotalPoints { get; set; } = 100;
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now.AddDays(7);
    }

    public class TakeExamViewModel
    {
        public string StudentExamId { get; set; } = null!;
        public string ExamName { get; set; } = string.Empty;
        public int Duration { get; set; }
        public DateTime StartTime { get; set; }
        public int RemainingTime { get; set; }
        public List<QuestionInExamViewModel> Questions { get; set; } = new();
    }

    public class QuestionInExamViewModel
    {
        public string ExamQuestionId { get; set; } = null!;
        public string QuestionId { get; set; } = null!;
        public int Order { get; set; }
        public string Text { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public List<ChoiceInExamViewModel> Choices { get; set; } = new();
        public string? SelectedChoiceId { get; set; }
    }

    public class ChoiceInExamViewModel
    {
        public string Id { get; set; } = null!;
        public string Label { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
    }

    public class SubmitAnswerRequest
    {
        public string StudentExamId { get; set; } = null!;
        public string QuestionId { get; set; } = null!;
        public string? ChoiceId { get; set; }
    }

    public class ExamResultViewModel
    {
        public DTOs.ExamResultDto Result { get; set; } = null!;
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
