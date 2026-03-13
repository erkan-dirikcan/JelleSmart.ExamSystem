using JelleSmart.ExamSystem.Core.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace JelleSmart.ExamSystem.WebUI.Models
{
    public class RegisterViewModelUI : RegisterViewModel
    {
        public List<SelectListItem>? AvailableSubjects { get; set; }
        public List<SelectListItem>? AvailableGrades { get; set; }
    }
}
