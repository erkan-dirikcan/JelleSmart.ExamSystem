using JelleSmart.ExamSystem.Core.ViewModels;
using JelleSmart.ExamSystem.Core.Interfaces.Services;
using JelleSmart.ExamSystem.Core.Enums;
using JelleSmart.ExamSystem.WebUI.ViewComponents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JelleSmart.ExamSystem.WebUI.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    public class UnitController : Controller
    {
        private readonly IUnitService _unitService;
        private readonly ISubjectService _subjectService;
        private readonly IGradeService _gradeService;

        public UnitController(IUnitService unitService, ISubjectService subjectService, IGradeService gradeService)
        {
            _unitService = unitService;
            _subjectService = subjectService;
            _gradeService = gradeService;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["ActivePage"] = ManageNavPages.Units;
            ViewData["Title"] = "Üniteler";
            ViewData["PageDescription"] = "Sistem ünitelerini yönetin";
            var viewModels = await _unitService.GetAllViewModelAsync();
            return View(viewModels);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["ActivePage"] = ManageNavPages.Units;
            ViewData["Title"] = "Yeni Ünite";
            ViewData["PageDescription"] = "Yeni ünite ekleyin";
            ViewBag.Subjects = await _subjectService.GetAllViewModelAsync();
            ViewBag.Grades = await _gradeService.GetAllViewModelAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UnitViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Subjects = await _subjectService.GetAllViewModelAsync();
                ViewBag.Grades = await _gradeService.GetAllViewModelAsync();
                return View(viewModel);
            }

            await _unitService.CreateViewModelAsync(viewModel);
            TempData["Success"] = "Ünite başarıyla eklendi";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(string id)
        {
            ViewData["ActivePage"] = ManageNavPages.Units;
            ViewData["Title"] = "Ünite Düzenle";
            ViewData["PageDescription"] = "Ünite bilgilerini düzenleyin";

            var viewModel = await _unitService.GetViewModelByIdAsync(id);
            if (viewModel == null)
                return NotFound();

            ViewBag.Subjects = await _subjectService.GetAllViewModelAsync();
            ViewBag.Grades = await _gradeService.GetAllViewModelAsync();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UnitViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Subjects = await _subjectService.GetAllViewModelAsync();
                ViewBag.Grades = await _gradeService.GetAllViewModelAsync();
                return View(viewModel);
            }

            await _unitService.UpdateViewModelAsync(viewModel);
            TempData["Success"] = "Ünite başarıyla güncellendi";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string id)
        {
            ViewData["ActivePage"] = ManageNavPages.Units;
            ViewData["Title"] = "Ünite Sil";
            ViewData["PageDescription"] = "Ünite silme onayı";

            var viewModel = await _unitService.GetViewModelByIdAsync(id);
            if (viewModel == null)
                return NotFound();

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _unitService.DeleteAsync(id);
            TempData["Success"] = "Ünite başarıyla silindi";
            return RedirectToAction("Index");
        }
    }
}
