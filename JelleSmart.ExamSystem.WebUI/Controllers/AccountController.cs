using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.Entities.Identity;
using JelleSmart.ExamSystem.Core.Interfaces.Services;
using JelleSmart.ExamSystem.Core.Enums;
using JelleSmart.ExamSystem.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JelleSmart.ExamSystem.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly ISubjectService _subjectService;
        private readonly IGradeService _gradeService;

        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<AppRole> roleManager,
            ISubjectService subjectService,
            IGradeService gradeService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _subjectService = subjectService;
            _gradeService = gradeService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Geçersiz e-posta veya şifre");
                return View(model);
            }

            if (!user.IsActive)
            {
                ModelState.AddModelError("", "Hesabınız pasif durumda. Lütfen yönetici ile iletişime geçin.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "Hesabınız çok fazla başarısız girişten dolayı kilitlendi. Lütfen bekleyin.");
                return View(model);
            }

            ModelState.AddModelError("", "Geçersiz e-posta veya şifre");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var model = new RegisterViewModel
            {
                AvailableSubjects = (await _subjectService.GetAllAsync())
                    .Select(s => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Value = s.Id.ToString(),
                        Text = s.Name
                    }).ToList(),
                AvailableGrades = (await _gradeService.GetAllAsync())
                    .Select(g => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Value = g.Id.ToString(),
                        Text = g.Name
                    }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableSubjects = (await _subjectService.GetAllAsync())
                    .Select(s => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Value = s.Id.ToString(),
                        Text = s.Name
                    }).ToList();
                model.AvailableGrades = (await _gradeService.GetAllAsync())
                    .Select(g => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Value = g.Id.ToString(),
                        Text = g.Name
                    }).ToList();
                return View(model);
            }

            var user = new AppUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Email,
                IsActive = true,
                SubjectId = model.Role == UserRoles.Teacher ? model.SubjectId : null,
                GradeId = model.Role == UserRoles.Student ? model.GradeId : null
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                model.AvailableSubjects = (await _subjectService.GetAllAsync())
                    .Select(s => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Value = s.Id.ToString(),
                        Text = s.Name
                    }).ToList();
                model.AvailableGrades = (await _gradeService.GetAllAsync())
                    .Select(g => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Value = g.Id.ToString(),
                        Text = g.Name
                    }).ToList();
                return View(model);
            }

            // Rol ata
            if (!await _roleManager.RoleExistsAsync(model.Role))
            {
                await _roleManager.CreateAsync(new AppRole { Name = model.Role });
            }
            await _userManager.AddToRoleAsync(user, model.Role);

            // Öğrenci için ders seçimi
            if (model.Role == UserRoles.Student && model.SubjectId.HasValue)
            {
                // StudentSubject ilişkisi eklenebilir
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
