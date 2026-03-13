using JelleSmart.ExamSystem.Core.ViewModels;
using JelleSmart.ExamSystem.Core.Interfaces.Services;
using JelleSmart.ExamSystem.Core.Enums;
using JelleSmart.ExamSystem.WebUI.ViewComponents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JelleSmart.ExamSystem.WebUI.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    public class TopicController : Controller
    {
        private readonly ITopicService _topicService;
        private readonly IUnitService _unitService;

        public TopicController(ITopicService topicService, IUnitService unitService)
        {
            _topicService = topicService;
            _unitService = unitService;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["ActivePage"] = ManageNavPages.Topics;
            ViewData["Title"] = "Konular";
            ViewData["PageDescription"] = "Ünite konularını yönetin";
            var viewModels = await _topicService.GetAllViewModelAsync();
            return View(viewModels);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["ActivePage"] = ManageNavPages.Topics;
            ViewData["Title"] = "Yeni Konu";
            ViewData["PageDescription"] = "Yeni konu ekleyin";
            ViewBag.Units = await _unitService.GetAllViewModelAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TopicViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Units = await _unitService.GetAllViewModelAsync();
                return View(viewModel);
            }

            await _topicService.CreateViewModelAsync(viewModel);
            TempData["Success"] = "Konu başarıyla eklendi";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(string id)
        {
            ViewData["ActivePage"] = ManageNavPages.Topics;
            ViewData["Title"] = "Konu Düzenle";
            ViewData["PageDescription"] = "Konu bilgilerini düzenleyin";

            var viewModel = await _topicService.GetViewModelByIdAsync(id);
            if (viewModel == null)
                return NotFound();

            ViewBag.Units = await _unitService.GetAllViewModelAsync();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TopicViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Units = await _unitService.GetAllViewModelAsync();
                return View(viewModel);
            }

            await _topicService.UpdateViewModelAsync(viewModel);
            TempData["Success"] = "Konu başarıyla güncellendi";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string id)
        {
            ViewData["ActivePage"] = ManageNavPages.Topics;
            ViewData["Title"] = "Konu Sil";
            ViewData["PageDescription"] = "Konu silme onayı";

            var viewModel = await _topicService.GetViewModelByIdAsync(id);
            if (viewModel == null)
                return NotFound();

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _topicService.DeleteAsync(id);
            TempData["Success"] = "Konu başarıyla silindi";
            return RedirectToAction("Index");
        }
    }
}
