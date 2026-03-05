using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.Interfaces.Services;
using JelleSmart.ExamSystem.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JelleSmart.ExamSystem.WebUI.ViewComponents;

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
            return View(subjects);
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
        public async Task<IActionResult> Create(Subject subject)
        {
            if (!ModelState.IsValid)
                return View(subject);

            await _subjectService.CreateAsync(subject);
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

            return View(subject);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Subject subject)
        {
            if (!ModelState.IsValid)
                return View(subject);

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

            return View(subject);
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
