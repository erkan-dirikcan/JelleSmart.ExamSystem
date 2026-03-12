using System.ComponentModel.DataAnnotations;

namespace JelleSmart.ExamSystem.Core.DTOs
{
    public class ForgotPasswordDto
    {
        [Required(ErrorMessage = "E-posta adresi gereklidir")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        public string Email { get; set; } = string.Empty;
    }
}
