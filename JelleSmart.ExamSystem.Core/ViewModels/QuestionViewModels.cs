using Microsoft.AspNetCore.Http;

namespace JelleSmart.ExamSystem.Core.ViewModels
{
    public class QuestionViewModel
    {
        public string? Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int Difficulty { get; set; } = 1;
        public string? SubjectId { get; set; }
        public string? UnitId { get; set; }
        public string? TopicId { get; set; }
        public string? GradeId { get; set; }
        public IFormFile? ImageFile { get; set; }
        public List<ChoiceViewModel> Choices { get; set; } = new();
    }

    public class ChoiceViewModel
    {
        public string? Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }
}
