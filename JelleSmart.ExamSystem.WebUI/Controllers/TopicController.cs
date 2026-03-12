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
            var topics = await _topicService.GetAllAsync();
            var viewModels = topics.Select(t => t.ToViewModel()).ToList();
            return View(viewModels);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["ActivePage"] = ManageNavPages.Topics;
            ViewData["Title"] = "Yeni Konu";
            ViewData["PageDescription"] = "Yeni konu ekleyin";
            ViewBag.Units = await _unitService.GetAllAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TopicViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Units = await _unitService.GetAllAsync();
                return View(viewModel);
            }

            var entity = viewModel.ToEntity();
            await _topicService.CreateAsync(entity);
            TempData["Success"] = "Konu başarıyla eklendi";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewData["ActivePage"] = ManageNavPages.Topics;
            ViewData["Title"] = "Konu Düzenle";
            ViewData["PageDescription"] = "Konu bilgilerini düzenleyin";

            var topic = await _topicService.GetByIdAsync(id);
            if (topic == null)
                return NotFound();

            ViewBag.Units = await _unitService.GetAllAsync();
            return View(topic.ToViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TopicViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Units = await _unitService.GetAllAsync();
                return View(viewModel);
            }

            var topic = await _topicService.GetByIdAsync(viewModel.Id);
            if (topic == null)
                return NotFound();

            viewModel.UpdateEntity(topic);
            await _topicService.UpdateAsync(topic);
            TempData["Success"] = "Konu başarıyla güncellendi";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            ViewData["ActivePage"] = ManageNavPages.Topics;
            ViewData["Title"] = "Konu Sil";
            ViewData["PageDescription"] = "Konu silme onayı";

            var topic = await _topicService.GetByIdAsync(id);
            if (topic == null)
                return NotFound();

            return View(topic.ToViewModel());
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _topicService.DeleteAsync(id);
            TempData["Success"] = "Konu başarıyla silindi";
            return RedirectToAction("Index");
        }
    }
}
