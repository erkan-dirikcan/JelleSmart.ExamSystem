using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace JelleSmart.ExamSystem.WebUI.Models
{
    public class QuestionViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Soru metni gereklidir")]
        public string Text { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        [Range(1, 3, ErrorMessage = "Zorluk seviyesi 1-3 arasında olmalıdır")]
        public int Difficulty { get; set; } = 1;

        [Required(ErrorMessage = "Ders seçimi gereklidir")]
        public int SubjectId { get; set; }

        public int? UnitId { get; set; }

        public int? TopicId { get; set; }

        [Required(ErrorMessage = "Sınıf seçimi gereklidir")]
        public int GradeId { get; set; }

        public IFormFile? ImageFile { get; set; }

        public List<ChoiceViewModel> Choices { get; set; } = new List<ChoiceViewModel>
        {
            new ChoiceViewModel { Label = "A" },
            new ChoiceViewModel { Label = "B" },
            new ChoiceViewModel { Label = "C" },
            new ChoiceViewModel { Label = "D" }
        };
    }

    public class ChoiceViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Şık metni gereklidir")]
        public string Text { get; set; } = string.Empty;

        public string Label { get; set; } = string.Empty;

        public bool IsCorrect { get; set; }
    }
}
