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
            var subjects = await _subjectService.GetAllAsync();
            var viewModels = subjects.Select(s => s.ToViewModel()).ToList();
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

            var entity = viewModel.ToEntity();
            await _subjectService.CreateAsync(entity);
            TempData["Success"] = "Ders başarıyla eklendi";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewData["ActivePage"] = ManageNavPages.Subjects;
            ViewData["Title"] = "Ders Düzenle";
            ViewData["PageDescription"] = "Ders bilgilerini düzenleyin";

            var subject = await _subjectService.GetByIdAsync(id);
            if (subject == null)
                return NotFound();

            return View(subject.ToViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubjectViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var subject = await _subjectService.GetByIdAsync(viewModel.Id);
            if (subject == null)
                return NotFound();

            viewModel.UpdateEntity(subject);
            await _subjectService.UpdateAsync(subject);
            TempData["Success"] = "Ders başarıyla güncellendi";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            ViewData["ActivePage"] = ManageNavPages.Subjects;
            ViewData["Title"] = "Ders Sil";
            ViewData["PageDescription"] = "Ders silme onayı";

            var subject = await _subjectService.GetByIdAsync(id);
            if (subject == null)
                return NotFound();

            return View(subject.ToViewModel());
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _subjectService.DeleteAsync(id);
            TempData["Success"] = "Ders başarıyla silindi";
            return RedirectToAction("Index");
        }
    }
}
