using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.Interfaces.Services;
using JelleSmart.ExamSystem.Core.Enums;
using JelleSmart.ExamSystem.Core.ViewModels;
using JelleSmart.ExamSystem.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JelleSmart.ExamSystem.WebUI.Controllers
{
    [Authorize(Roles = UserRoles.Teacher)]
    public class TeacherExamController : Controller
    {
        private readonly IExamService _examService;
        private readonly IQuestionService _questionService;
        private readonly ISubjectService _subjectService;
        private readonly IGradeService _gradeService;
        private readonly ITopicService _topicService;
        private readonly IReportService _reportService;

        public TeacherExamController(
            IExamService examService,
            IQuestionService questionService,
            ISubjectService subjectService,
            IGradeService gradeService,
            ITopicService topicService,
            IReportService reportService)
        {
            _examService = examService;
            _questionService = questionService;
            _subjectService = subjectService;
            _gradeService = gradeService;
            _topicService = topicService;
            _reportService = reportService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var exams = await _examService.GetByTeacherAsync(userId!);
            return View(exams);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Subjects = await _subjectService.GetAllAsync();
            ViewBag.Grades = await _gradeService.GetAllAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateExamViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Subjects = await _subjectService.GetAllAsync();
                ViewBag.Grades = await _gradeService.GetAllAsync();
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var dto = new Core.DTOs.CreateExamDto
            {
                Name = model.Name,
                Description = model.Description,
                Duration = model.Duration,
                GradeId = model.GradeId,
                SubjectId = model.SubjectId,
                TopicIds = model.TopicIds ?? new List<string?>(),
                QuestionCount = model.QuestionCount,
                TotalPoints = model.TotalPoints,
                StartTime = model.StartDate,
                EndTime = model.EndDate,
                CreatedByUserId = userId!
            };

            try
            {
                await _examService.CreateAsync(dto);
                TempData["Success"] = "Sinav basariyla olusturuldu";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.Subjects = await _subjectService.GetAllAsync();
                ViewBag.Grades = await _gradeService.GetAllAsync();
                return View(model);
            }
        }

        public async Task<IActionResult> Details(string id)
        {
            var exam = await _examService.GetWithQuestionsAsync(id);
            if (exam == null)
                return NotFound();

            return View(exam);
        }

        public async Task<IActionResult> Activate(string id)
        {
            await _examService.ActivateExamAsync(id);
            TempData["Success"] = "Sinav aktiflestirildi";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Deactivate(string id)
        {
            await _examService.DeactivateExamAsync(id);
            TempData["Success"] = "Sinav pasife alindi";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string id)
        {
            var exam = await _examService.GetByIdAsync(id);
            if (exam == null)
                return NotFound();

            return View(exam);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _examService.DeleteAsync(id);
            TempData["Success"] = "Sinav basariyla silindi";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Results(string id)
        {
            var results = await _reportService.GetExamResultsByExamAsync(id);
            var exam = await _examService.GetByIdAsync(id);

            ViewBag.ExamName = exam?.Name;
            return View(results);
        }

        [HttpPost]
        public async Task<JsonResult> GetTopicsByGradeSubject(string gradeId, string subjectId)
        {
            // Get all topics and filter by grade and subject through the unit hierarchy
            var allTopics = await _topicService.GetAllAsync();
            var topics = allTopics.Where(t => t.GradeId == gradeId);
            return Json(topics.Select(t => new { id = t.Id, name = $"{t.Unit?.Name} - {t.Name}", code = t.Code }));
        }

        [HttpPost]
        public async Task<JsonResult> GetQuestionCount(List<string?>? topicIds)
        {
            if (topicIds == null || !topicIds.Any())
                return Json(new { count = 0 });

            var allQuestions = await _questionService.GetAllAsync();
            var questions = allQuestions.Where(q => !string.IsNullOrEmpty(q.TopicId) && topicIds.Contains(q.TopicId));

            return Json(new { count = questions.Count() });
        }
    }
}
