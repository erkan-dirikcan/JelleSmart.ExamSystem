using JelleSmart.ExamSystem.Core.DTOs;
using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.Entities.Identity;
using JelleSmart.ExamSystem.Core.Enums;
using JelleSmart.ExamSystem.Core.Interfaces.Services;
using JelleSmart.ExamSystem.Core.ViewModels;
using JelleSmart.ExamSystem.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JelleSmart.ExamSystem.WebUI.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly ISubjectService _subjectService;
        private readonly IGradeService _gradeService;
        private readonly IEmailService _emailService;

        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<AppRole> roleManager,
            ISubjectService subjectService,
            IGradeService gradeService,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _subjectService = subjectService;
            _gradeService = gradeService;
            _emailService = emailService;
        }

        private async Task PopulateRegistrationDropdownsAsync(RegisterViewModelUI model)
        {
            // DbContext is NOT thread-safe - run queries sequentially, not in parallel
            var subjects = await _subjectService.GetAllAsync();
            var grades = await _gradeService.GetAllAsync();

            model.AvailableSubjects = subjects
                .Select(s => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = s.Id,
                    Text = s.Name
                }).ToList();
            model.AvailableGrades = grades
                .Select(g => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = g.Id,
                    Text = g.Name
                }).ToList();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = "/")
        {
            var model = (new LoginViewModel(), new ForgotPasswordViewModel());
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind(Prefix = "Item1")] LoginViewModel loginModel, [Bind(Prefix = "Item2")] ForgotPasswordViewModel forgetModel, string returnUrl = "/")
        {
            var model = (loginModel, forgetModel);
            var user = await _userManager.FindByEmailAsync(loginModel.Email ?? "");
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Kullanıcı Bilgilerinizi kontrol ederek tekrar deneyiniz");
                return View(model);
            }

            var res = await _signInManager.PasswordSignInAsync(user, loginModel.Password, loginModel.RememberMe, true);
            if (res.IsLockedOut)
            {
                ModelState.AddModelError("", "Hesabınız Kilitli, 15 dakika boyunca giriş yapamazsınız. Lütfen daha sonra tekrar deneyiniz.");
                return View(model);
            }

            if (res.IsNotAllowed)
            {
                ModelState.AddModelError("", "Erişim Yetkiniz bulunmamaktadır.");
                return View(model);
            }

            if (res.RequiresTwoFactor)
            {
                return RedirectToAction("Login");
            }

            if (res.Succeeded)
            {
                return Redirect(returnUrl);
            }

            ModelState.AddModelError(string.Empty, "Kullanıcı Bilgilerinizi kontrol ederek tekrar deneyiniz");
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ForgetPassword([Bind(Prefix = "Item1")] LoginViewModel loginModel, [Bind(Prefix = "Item2")] ForgotPasswordViewModel forgetModel)
        {
            var url = Request.Host;
            var model = (loginModel, forgetModel);

            if (string.IsNullOrEmpty(forgetModel.Email))
            {
                ModelState.AddModelError(nameof(forgetModel.Email), "E-Posta Adresi Boş Bırakılamaz");
                return View("Error");
            }

            try
            {
                var user = await _userManager.FindByEmailAsync(forgetModel.Email);
                if (user == null)
                {
                    TempData["Eposta"] = forgetModel.Email;
                    return RedirectToAction("ForgotConfirm", "Account");
                }

                var passwordResetTokenstring = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetUrl = "https://" + url.Value + Url.Action("ResetPassword", "Account", new { userId = user.Id, token = passwordResetTokenstring });
                TempData["Eposta"] = forgetModel.Email;

                await _emailService.SendResetPasswordEmailAsync(new ResetPasswordEmailDto
                {
                    Email = user.Email,
                    Name = user.FirstName ?? "",
                    Lastname = user.LastName ?? "",
                    ResetLink = resetUrl
                });

                return RedirectToAction("ForgotConfirm", "Account");
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("", "E-posta gönderilirken bir hata oluştu: " + ex.Message);
                return View("Error");
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotConfirm()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }
            
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new ResetPasswordDto 
            { 
                Token = token, 
                Email = user.Email 
            };
            
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                TempData["Success"] = "Şifreniz başarıyla sıfırlandı. Lütfen giriş yapınız.";
                return RedirectToAction("Login", "Account");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (result.Succeeded)
            {
                TempData["Success"] = "Şifreniz başarıyla sıfırlandı. Yeni şifrenizle giriş yapabilirsiniz.";
                return RedirectToAction("Login", "Account");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var model = new RegisterViewModelUI();
            await PopulateRegistrationDropdownsAsync(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModelUI model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateRegistrationDropdownsAsync(model);
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
                await PopulateRegistrationDropdownsAsync(model);
                return View(model);
            }

            if (!await _roleManager.RoleExistsAsync(model.Role))
            {
                await _roleManager.CreateAsync(new AppRole { Name = model.Role });
            }
            await _userManager.AddToRoleAsync(user, model.Role);

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
