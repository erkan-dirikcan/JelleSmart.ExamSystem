using System.ComponentModel.DataAnnotations;

namespace JelleSmart.ExamSystem.Core.ViewModels
{
    public class TopicViewModel
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Konu adı gereklidir")]
        [StringLength(200, ErrorMessage = "Konu adı en fazla 200 karakter olabilir")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ünite seçimi gereklidir")]
        public string? UnitId { get; set; }

        [Required(ErrorMessage = "Sınıf seçimi gereklidir")]
        public string? GradeId { get; set; }

        [StringLength(50, ErrorMessage = "Kod en fazla 50 karakter olabilir")]
        public string? Code { get; set; }

        [StringLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir")]
        public string? Description { get; set; }

        public int Order { get; set; } = 1;

        // For display purposes
        public string? UnitName { get; set; }
        public string? GradeName { get; set; }
    }
}
