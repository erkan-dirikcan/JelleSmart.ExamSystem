using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.Interfaces.Services;
using JelleSmart.ExamSystem.Core.Enums;
using JelleSmart.ExamSystem.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JelleSmart.ExamSystem.WebUI.Controllers
{
    [Authorize(Roles = UserRoles.Student)]
    public class StudentExamController : Controller
    {
        private readonly IExamService _examService;
        private readonly IStudentExamService _studentExamService;
        private readonly ISubjectService _subjectService;

        public StudentExamController(
            IExamService examService,
            IStudentExamService studentExamService,
            ISubjectService subjectService)
        {
            _examService = examService;
            _studentExamService = studentExamService;
            _subjectService = subjectService;
        }

        public async Task<IActionResult> Index()
        {
            var exams = await _examService.GetAllAsync();
            var activeExams = exams.Where(e => e.IsActive && !e.IsDeleted).ToList();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var studentExams = await _studentExamService.GetByStudentAsync(userId!);

            ViewBag.StudentExams = studentExams;
            return View(activeExams);
        }

        public async Task<IActionResult> Details(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var exam = await _examService.GetWithQuestionsAsync(id);

            if (exam == null || !exam.IsActive)
                return NotFound();

            // Ã–ÄŸrenci bu sÄ±nava daha Ã¶nce girdi mi?
            var studentExam = await _studentExamService.GetByStudentAndExamAsync(userId!, id);
            ViewBag.StudentExam = studentExam;

            return View(exam);
        }

        public async Task<IActionResult> Start(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                var studentExam = await _studentExamService.StartExamAsync(id, userId!);
                return RedirectToAction("Take", new { id = studentExam.Id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Details", new { id });
            }
        }

        public async Task<IActionResult> Take(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var studentExam = await _studentExamService.GetWithAnswersAsync(id);

            if (studentExam == null)
                return NotFound();

            if (studentExam.StudentUserId != userId)
                return Forbid();

            if (studentExam.Status == ExamStatus.Completed)
                return RedirectToAction("Result", new { id });

            var exam = await _examService.GetWithQuestionsAsync(studentExam.ExamId);

            var viewModel = new TakeExamViewModel
            {
                StudentExamId = studentExam.Id,
                ExamName = exam!.Name,
                Duration = exam.Duration,
                StartTime = studentExam.StartedAt ?? DateTime.UtcNow,
                RemainingTime = studentExam.RemainingTime ?? (exam.Duration * 60),
                Questions = exam.ExamQuestions
                    .OrderBy(eq => eq.Order)
                    .Select(eq => new QuestionInExamViewModel
                    {
                        ExamQuestionId = eq.Id,
                        QuestionId = eq.QuestionId,
                        Order = eq.Order,
                        Text = eq.Question!.Text,
                        ImageUrl = eq.Question.ImageUrl,
                        Choices = eq.Question.Choices.OrderBy(c => c.Label).Select(c => new ChoiceInExamViewModel
                        {
                            Id = c.Id,
                            Label = c.Label,
                            Text = c.Text,
                            ImageUrl = c.ImageUrl
                        }).ToList(),
                        SelectedChoiceId = studentExam.StudentAnswers
                            .FirstOrDefault(sa => sa.QuestionId == eq.QuestionId)?.ChoiceId
                    }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitAnswer([FromBody] SubmitAnswerRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var dto = new Core.DTOs.SubmitAnswerDto
            {
                StudentExamId = request.StudentExamId,
                QuestionId = request.QuestionId,
                ChoiceId = request.ChoiceId,
                StudentUserId = userId!
            };

            await _studentExamService.SubmitAnswerAsync(dto);
            return Json(new { success = true });
        }

        public async Task<IActionResult> Complete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var studentExam = await _studentExamService.GetByIdAsync(id);

            if (studentExam == null || studentExam.StudentUserId != userId)
                return Forbid();

            try
            {
                await _studentExamService.CompleteExamAsync(id);
                return RedirectToAction("Result", new { id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Take", new { id });
            }
        }

        public async Task<IActionResult> Result(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var studentExam = await _studentExamService.GetByIdAsync(id);

            if (studentExam == null || studentExam.StudentUserId != userId)
                return Forbid();

            if (studentExam.Status != ExamStatus.Completed)
                return RedirectToAction("Take", new { id });

            var result = await _studentExamService.GetExamResultAsync(id);

            var studentExamWithAnswers = await _studentExamService.GetWithAnswersAsync(id);
            var exam = await _examService.GetWithQuestionsAsync(studentExam.ExamId);

            var viewModel = new ExamResultViewModel
            {
                Result = result,
                Answers = studentExamWithAnswers.StudentAnswers.Select(sa => new AnswerDetailViewModel
                {
                    QuestionText = exam!.ExamQuestions.FirstOrDefault(eq => eq.QuestionId == sa.QuestionId)?.Question?.Text ?? "",
                    IsCorrect = sa.IsCorrect,
                    SelectedChoice = sa.Choice?.Text ?? "BoÅŸ",
                    CorrectChoice = exam.ExamQuestions
                        .FirstOrDefault(eq => eq.QuestionId == sa.QuestionId)?
                        .Question?.Choices.FirstOrDefault(c => c.IsCorrect)?.Text ?? ""
                }).ToList()
            };

            return View(viewModel);
        }
    }
}
