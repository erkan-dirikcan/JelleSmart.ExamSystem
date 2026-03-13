using System.ComponentModel.DataAnnotations;

namespace JelleSmart.ExamSystem.Core.RequestModels
{
    public class CreateQuestionRequestModel
    {
        [Required(ErrorMessage = "Soru metni gereklidir")]
        public string Text { get; set; } = string.Empty;

        [Range(1, 3, ErrorMessage = "Zorluk seviyesi 1-3 arasında olmalıdır")]
        public int Difficulty { get; set; } = 1;

        [Required(ErrorMessage = "Ders seçimi gereklidir")]
        public string? SubjectId { get; set; }

        public string? UnitId { get; set; }
        public string? TopicId { get; set; }

        [Required(ErrorMessage = "Sınıf seçimi gereklidir")]
        public string? GradeId { get; set; }

        public string CreatedByUserId { get; set; } = string.Empty;

        public List<CreateChoiceRequestModel>? Choices { get; set; }
    }

    public class UpdateQuestionRequestModel
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Soru metni gereklidir")]
        public string Text { get; set; } = string.Empty;

        [Range(1, 3, ErrorMessage = "Zorluk seviyesi 1-3 arasında olmalıdır")]
        public int Difficulty { get; set; } = 1;

        [Required(ErrorMessage = "Ders seçimi gereklidir")]
        public string? SubjectId { get; set; }

        public string? UnitId { get; set; }
        public string? TopicId { get; set; }

        [Required(ErrorMessage = "Sınıf seçimi gereklidir")]
        public string? GradeId { get; set; }

        public List<CreateChoiceRequestModel>? Choices { get; set; }
    }

    public class CreateChoiceRequestModel
    {
        [Required(ErrorMessage = "Şık metni gereklidir")]
        public string Text { get; set; } = string.Empty;

        public bool IsCorrect { get; set; }
    }

    public class UploadQuestionImageRequestModel
    {
        public string QuestionId { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public Stream Stream { get; set; } = null!;
    }
}
