using System.ComponentModel.DataAnnotations;

namespace JelleSmart.ExamSystem.WebUI.Models
{
    public class CreateExamViewModel
    {
        [Required(ErrorMessage = "Sınav adı gereklidir")]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Süre gereklidir")]
        [Range(1, 180, ErrorMessage = "Süre 1-180 dakika arasında olmalıdır")]
        public int Duration { get; set; } = 30;

        [Required(ErrorMessage = "Sınıf seçimi gereklidir")]
        public int GradeId { get; set; }

        [Required(ErrorMessage = "Ders seçimi gereklidir")]
        public int SubjectId { get; set; }

        public List<int>? TopicIds { get; set; }

        [Required(ErrorMessage = "Soru sayısı gereklidir")]
        [Range(1, 100, ErrorMessage = "Soru sayısı 1-100 arasında olmalıdır")]
        public int QuestionCount { get; set; } = 10;

        [Required(ErrorMessage = "Toplam puan gereklidir")]
        [Range(1, 1000, ErrorMessage = "Toplam puan 1-1000 arasında olmalıdır")]
        public double TotalPoints { get; set; } = 100;

        [Required(ErrorMessage = "Başlangıç tarihi gereklidir")]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Bitiş tarihi gereklidir")]
        public DateTime EndDate { get; set; } = DateTime.Now.AddDays(7);
    }
}
