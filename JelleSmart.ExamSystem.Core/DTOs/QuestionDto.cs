namespace JelleSmart.ExamSystem.Core.DTOs
{
    public class QuestionDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public int? UnitId { get; set; }
        public string? UnitName { get; set; }
        public int? TopicId { get; set; }
        public string? TopicName { get; set; }
        public int Difficulty { get; set; }
        public List<ChoiceDto> Choices { get; set; } = new();
    }
}
