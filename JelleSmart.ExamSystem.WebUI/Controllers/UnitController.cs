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
            var units = await _unitService.GetAllAsync();
            var viewModels = units.Select(u => u.ToViewModel()).ToList();
            return View(viewModels);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["ActivePage"] = ManageNavPages.Units;
            ViewData["Title"] = "Yeni Ünite";
            ViewData["PageDescription"] = "Yeni ünite ekleyin";
            ViewBag.Subjects = await _subjectService.GetAllAsync();
            ViewBag.Grades = await _gradeService.GetAllAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UnitViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Subjects = await _subjectService.GetAllAsync();
                ViewBag.Grades = await _gradeService.GetAllAsync();
                return View(viewModel);
            }

            var entity = viewModel.ToEntity();
            await _unitService.CreateAsync(entity);
            TempData["Success"] = "Ünite başarıyla eklendi";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewData["ActivePage"] = ManageNavPages.Units;
            ViewData["Title"] = "Ünite Düzenle";
            ViewData["PageDescription"] = "Ünite bilgilerini düzenleyin";

            var unit = await _unitService.GetByIdAsync(id);
            if (unit == null)
                return NotFound();

            ViewBag.Subjects = await _subjectService.GetAllAsync();
            ViewBag.Grades = await _gradeService.GetAllAsync();
            return View(unit.ToViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UnitViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Subjects = await _subjectService.GetAllAsync();
                ViewBag.Grades = await _gradeService.GetAllAsync();
                return View(viewModel);
            }

            var unit = await _unitService.GetByIdAsync(viewModel.Id);
            if (unit == null)
                return NotFound();

            viewModel.UpdateEntity(unit);
            await _unitService.UpdateAsync(unit);
            TempData["Success"] = "Ünite başarıyla güncellendi";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            ViewData["ActivePage"] = ManageNavPages.Units;
            ViewData["Title"] = "Ünite Sil";
            ViewData["PageDescription"] = "Ünite silme onayı";

            var unit = await _unitService.GetByIdAsync(id);
            if (unit == null)
                return NotFound();

            return View(unit.ToViewModel());
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _unitService.DeleteAsync(id);
            TempData["Success"] = "Ünite başarıyla silindi";
            return RedirectToAction("Index");
        }
    }
}
