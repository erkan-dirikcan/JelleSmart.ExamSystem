using Microsoft.AspNetCore.Identity;

namespace JelleSmart.ExamSystem.Core.Entities.Identity
{
    public class AppRole : IdentityRole
    {
        public string? Description { get; set; }
    }
}
