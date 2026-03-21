using System.ComponentModel.DataAnnotations;

namespace JelleSmart.ExamSystem.Core.ViewModels
{
    public class UnitViewModel
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Ünite adı gereklidir")]
        [StringLength(200, ErrorMessage = "Ünite adı en fazla 200 karakter olabilir")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sınıf seçimi gereklidir")]
        public string? GradeId { get; set; }

        [StringLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir")]
        public string? Description { get; set; }

        public int Order { get; set; } = 1;

        // For display purposes
        public string? GradeName { get; set; }
    }
}
