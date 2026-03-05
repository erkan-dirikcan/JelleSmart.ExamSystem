namespace JelleSmart.ExamSystem.Core.DTOs
{
    public class ChoiceDto
    {
        public int Id { get; set; }
        public string Label { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
    }
}
