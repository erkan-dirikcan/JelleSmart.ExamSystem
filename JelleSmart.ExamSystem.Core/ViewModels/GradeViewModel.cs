using System.ComponentModel.DataAnnotations;

namespace JelleSmart.ExamSystem.Core.ViewModels
{
    public class GradeViewModel
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Sınıf adı gereklidir")]
        [StringLength(50, ErrorMessage = "Sınıf adı en fazla 50 karakter olabilir")]
        public string Name { get; set; } = string.Empty;

        [Range(1, 12, ErrorMessage = "Sınıf seviyesi 1-12 arasında olmalıdır")]
        public int Level { get; set; }
    }
}
