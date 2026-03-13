using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.Entities.Identity;
using JelleSmart.ExamSystem.Core.Interfaces.Services;
using JelleSmart.ExamSystem.Core.Enums;
using JelleSmart.ExamSystem.Core.ViewModels;
using JelleSmart.ExamSystem.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JelleSmart.ExamSystem.WebUI.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    public class UserManagementController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly ISubjectService _subjectService;
        private readonly IGradeService _gradeService;

        public UserManagementController(
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            ISubjectService subjectService,
            IGradeService gradeService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _subjectService = subjectService;
            _gradeService = gradeService;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault() ?? "";

                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    FullName = user.FullName,
                    Email = user.Email!,
                    Role = role,
                    IsActive = user.IsActive,
                    GradeId = user.GradeId,
                    SubjectId = user.SubjectId
                });
            }

            return View(userViewModels);
        }

        public async Task<IActionResult> ToggleActive(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            user.IsActive = !user.IsActive;
            await _userManager.UpdateAsync(user);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "Kullanıcı başarıyla silindi";
            }
            else
            {
                TempData["Error"] = "Kullanıcı silinirken hata oluştu";
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            var viewModel = new UserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                Email = user.Email!,
                Role = roles.FirstOrDefault() ?? "",
                IsActive = user.IsActive,
                GradeId = user.GradeId,
                SubjectId = user.SubjectId
            };

            if (!string.IsNullOrEmpty(user.GradeId))
            {
                var grade = await _gradeService.GetByIdAsync(user.GradeId);
                viewModel.GradeName = grade?.Name;
            }

            if (!string.IsNullOrEmpty(user.SubjectId))
            {
                var subject = await _subjectService.GetByIdAsync(user.SubjectId);
                viewModel.SubjectName = subject?.Name;
            }

            return View(viewModel);
        }
    }
}
