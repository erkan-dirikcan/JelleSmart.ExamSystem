using JelleSmart.ExamSystem.Core.ViewModels;
using JelleSmart.ExamSystem.Core.Interfaces.Services;
using JelleSmart.ExamSystem.Core.Enums;
using JelleSmart.ExamSystem.WebUI.ViewComponents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JelleSmart.ExamSystem.WebUI.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    public class GradeController : Controller
    {
        private readonly IGradeService _gradeService;

        public GradeController(IGradeService gradeService)
        {
            _gradeService = gradeService;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["ActivePage"] = ManageNavPages.Grades;
            ViewData["Title"] = "Sınıflar";
            ViewData["PageDescription"] = "Sistem sınıflarını yönetin";
            var viewModels = await _gradeService.GetAllViewModelAsync();
            return View(viewModels.OrderBy(v => v.Level));
        }

        public IActionResult Create()
        {
            ViewData["ActivePage"] = ManageNavPages.Grades;
            ViewData["Title"] = "Yeni Sınıf";
            ViewData["PageDescription"] = "Yeni sınıf ekleyin";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GradeViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            await _gradeService.CreateViewModelAsync(viewModel);
            TempData["Success"] = "Sınıf başarıyla eklendi";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(string id)
        {
            ViewData["ActivePage"] = ManageNavPages.Grades;
            ViewData["Title"] = "Sınıf Düzenle";
            ViewData["PageDescription"] = "Sınıf bilgilerini düzenleyin";

            var viewModel = await _gradeService.GetViewModelByIdAsync(id);
            if (viewModel == null)
                return NotFound();

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(GradeViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            await _gradeService.UpdateViewModelAsync(viewModel);
            TempData["Success"] = "Sınıf başarıyla güncellendi";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string id)
        {
            ViewData["ActivePage"] = ManageNavPages.Grades;
            ViewData["Title"] = "Sınıf Sil";
            ViewData["PageDescription"] = "Sınıf silme onayı";

            var viewModel = await _gradeService.GetViewModelByIdAsync(id);
            if (viewModel == null)
                return NotFound();

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _gradeService.DeleteAsync(id);
            TempData["Success"] = "Sınıf başarıyla silindi";
            return RedirectToAction("Index");
        }
    }
}
