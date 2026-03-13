using JelleSmart.ExamSystem.Core.ViewModels;
using JelleSmart.ExamSystem.Core.Interfaces.Services;
using JelleSmart.ExamSystem.Core.Enums;
using JelleSmart.ExamSystem.WebUI.ViewComponents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JelleSmart.ExamSystem.WebUI.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    public class SubjectController : Controller
    {
        private readonly ISubjectService _subjectService;

        public SubjectController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["ActivePage"] = ManageNavPages.Subjects;
            ViewData["Title"] = "Dersler";
            ViewData["PageDescription"] = "Sistem derslerini yönetin";
            var viewModels = await _subjectService.GetAllViewModelAsync();
            return View(viewModels);
        }

        public IActionResult Create()
        {
            ViewData["ActivePage"] = ManageNavPages.Subjects;
            ViewData["Title"] = "Yeni Ders";
            ViewData["PageDescription"] = "Yeni ders ekleyin";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubjectViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            await _subjectService.CreateViewModelAsync(viewModel);
            TempData["Success"] = "Ders başarıyla eklendi";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(string id)
        {
            ViewData["ActivePage"] = ManageNavPages.Subjects;
            ViewData["Title"] = "Ders Düzenle";
            ViewData["PageDescription"] = "Ders bilgilerini düzenleyin";

            var viewModel = await _subjectService.GetViewModelByIdAsync(id);
            if (viewModel == null)
                return NotFound();

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubjectViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            await _subjectService.UpdateViewModelAsync(viewModel);
            TempData["Success"] = "Ders başarıyla güncellendi";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string id)
        {
            ViewData["ActivePage"] = ManageNavPages.Subjects;
            ViewData["Title"] = "Ders Sil";
            ViewData["PageDescription"] = "Ders silme onayı";

            var viewModel = await _subjectService.GetViewModelByIdAsync(id);
            if (viewModel == null)
                return NotFound();

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _subjectService.DeleteAsync(id);
            TempData["Success"] = "Ders başarıyla silindi";
            return RedirectToAction("Index");
        }
    }
}
