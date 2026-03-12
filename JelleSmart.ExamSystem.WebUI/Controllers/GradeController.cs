using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.Interfaces.Services;
using JelleSmart.ExamSystem.Core.Enums;
using JelleSmart.ExamSystem.WebUI.Models;
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
            var grades = await _gradeService.GetAllAsync();
            var viewModels = grades.OrderBy(g => g.Level).Select(g => g.ToViewModel()).ToList();
            return View(viewModels);
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

            var entity = viewModel.ToEntity();
            await _gradeService.CreateAsync(entity);
            TempData["Success"] = "Sınıf başarıyla eklendi";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewData["ActivePage"] = ManageNavPages.Grades;
            ViewData["Title"] = "Sınıf Düzenle";
            ViewData["PageDescription"] = "Sınıf bilgilerini düzenleyin";

            var grade = await _gradeService.GetByIdAsync(id);
            if (grade == null)
                return NotFound();

            return View(grade.ToViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(GradeViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var grade = await _gradeService.GetByIdAsync(viewModel.Id);
            if (grade == null)
                return NotFound();

            viewModel.UpdateEntity(grade);
            await _gradeService.UpdateAsync(grade);
            TempData["Success"] = "Sınıf başarıyla güncellendi";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            ViewData["ActivePage"] = ManageNavPages.Grades;
            ViewData["Title"] = "Sınıf Sil";
            ViewData["PageDescription"] = "Sınıf silme onayı";

            var grade = await _gradeService.GetByIdAsync(id);
            if (grade == null)
                return NotFound();

            return View(grade.ToViewModel());
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _gradeService.DeleteAsync(id);
            TempData["Success"] = "Sınıf başarıyla silindi";
            return RedirectToAction("Index");
        }
    }
}
