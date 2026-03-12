namespace JelleSmart.ExamSystem.Core.DTOs
{
    public class ResetPasswordEmailDto
    {
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;
        public string ResetLink { get; set; } = string.Empty;
    }
}
