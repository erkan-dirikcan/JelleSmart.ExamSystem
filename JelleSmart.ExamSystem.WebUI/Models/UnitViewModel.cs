using System.ComponentModel.DataAnnotations;

namespace JelleSmart.ExamSystem.WebUI.Models
{
    public class UnitViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ünite adı gereklidir")]
        [StringLength(200, ErrorMessage = "Ünite adı en fazla 200 karakter olabilir")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ders seçimi gereklidir")]
        public int SubjectId { get; set; }

        [Required(ErrorMessage = "Sınıf seçimi gereklidir")]
        public int GradeId { get; set; }

        [StringLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir")]
        public string? Description { get; set; }

        // For display purposes
        public string? SubjectName { get; set; }
        public string? GradeName { get; set; }
    }
}
