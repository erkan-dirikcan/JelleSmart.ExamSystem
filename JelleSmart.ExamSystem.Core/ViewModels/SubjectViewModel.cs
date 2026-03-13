using System.ComponentModel.DataAnnotations;

namespace JelleSmart.ExamSystem.Core.ViewModels
{
    public class SubjectViewModel
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Ders adı gereklidir")]
        [StringLength(100, ErrorMessage = "Ders adı en fazla 100 karakter olabilir")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
        public string? Description { get; set; }

        [StringLength(100, ErrorMessage = "İkon sınıfı en fazla 100 karakter olabilir")]
        public string? IconClass { get; set; }
    }
}
