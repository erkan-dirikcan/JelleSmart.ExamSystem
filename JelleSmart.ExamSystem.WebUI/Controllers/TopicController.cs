using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.Interfaces.Services;
using JelleSmart.ExamSystem.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JelleSmart.ExamSystem.WebUI.ViewComponents;

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
            return View(topics);
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
        public async Task<IActionResult> Create(Topic topic)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Units = await _unitService.GetAllAsync();
                return View(topic);
            }

            await _topicService.CreateAsync(topic);
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
            return View(topic);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Topic topic)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Units = await _unitService.GetAllAsync();
                return View(topic);
            }

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

            return View(topic);
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
