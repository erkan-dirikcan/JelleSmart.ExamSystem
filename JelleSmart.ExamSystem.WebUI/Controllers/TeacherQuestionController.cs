using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.Helpers;
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
    public class TeacherQuestionController : Controller
    {
        private readonly IQuestionService _questionService;
        private readonly ISubjectService _subjectService;
        private readonly IUnitService _unitService;
        private readonly ITopicService _topicService;
        private readonly IGradeService _gradeService;

        public TeacherQuestionController(
            IQuestionService questionService,
            ISubjectService subjectService,
            IUnitService unitService,
            ITopicService topicService,
            IGradeService gradeService)
        {
            _questionService = questionService;
            _subjectService = subjectService;
            _unitService = unitService;
            _topicService = topicService;
            _gradeService = gradeService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var questions = await _questionService.GetByTeacherAsync(userId!);
            return View(questions);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Subjects = await _subjectService.GetAllAsync();
            ViewBag.Grades = await _gradeService.GetAllAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QuestionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Subjects = await _subjectService.GetAllAsync();
                ViewBag.Grades = await _gradeService.GetAllAsync();
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var question = new Question
            {
                Text = model.Text,
                Difficulty = model.Difficulty,
                TopicId = model.TopicId,
                CreatedByUserId = userId!
            };

            // Once soruyu kaydet
            question = await _questionService.CreateAsync(question);

            // Siklari ekle
            if (model.Choices != null && model.Choices.Count >= 2)
            {
                for (int i = 0; i < model.Choices.Count; i++)
                {
                    var choiceDto = model.Choices[i];
                    if (!string.IsNullOrWhiteSpace(choiceDto.Text))
                    {
                        question.Choices.Add(new Choice
                        {
                            Label = ChoiceHelper.GenerateLabel(i),
                            Text = choiceDto.Text,
                            IsCorrect = choiceDto.IsCorrect,
                            QuestionId = question.Id
                        });
                    }
                }
            }

            await _questionService.UpdateAsync(question);

            // Gorsel varsa yukle
            if (model.ImageFile != null)
            {
                using (var stream = model.ImageFile.OpenReadStream())
                {
                    await _questionService.UploadImageAsync(question.Id, model.ImageFile.FileName, model.ImageFile.ContentType, stream);
                }
            }

            TempData["Success"] = "Soru basariyla eklendi";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(string id)
        {
            var question = await _questionService.GetWithChoicesAsync(id);
            if (question == null)
                return NotFound();

            var viewModel = new QuestionViewModel
            {
                Id = question.Id,
                Text = question.Text,
                ImageUrl = question.ImageUrl,
                Difficulty = question.Difficulty,
                TopicId = question.TopicId,
                Choices = question.Choices.Select(c => new ChoiceViewModel
                {
                    Id = c.Id,
                    Text = c.Text,
                    IsCorrect = c.IsCorrect
                }).ToList()
            };

            ViewBag.Subjects = await _subjectService.GetAllAsync();
            ViewBag.Grades = await _gradeService.GetAllAsync();

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(QuestionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Subjects = await _subjectService.GetAllAsync();
                ViewBag.Grades = await _gradeService.GetAllAsync();
                return View(model);
            }

            var question = await _questionService.GetWithChoicesAsync(model.Id);
            if (question == null)
                return NotFound();

            question.Text = model.Text;
            question.Difficulty = model.Difficulty;
            question.TopicId = model.TopicId;

            // Siklari guncelle
            question.Choices.Clear();
            if (model.Choices != null && model.Choices.Count >= 2)
            {
                for (int i = 0; i < model.Choices.Count; i++)
                {
                    var choiceDto = model.Choices[i];
                    if (!string.IsNullOrWhiteSpace(choiceDto.Text))
                    {
                        question.Choices.Add(new Choice
                        {
                            Label = ChoiceHelper.GenerateLabel(i),
                            Text = choiceDto.Text,
                            IsCorrect = choiceDto.IsCorrect,
                            QuestionId = question.Id
                        });
                    }
                }
            }

            await _questionService.UpdateAsync(question);

            // Yeni gorsel varsa yukle
            if (model.ImageFile != null)
            {
                using (var stream = model.ImageFile.OpenReadStream())
                {
                    await _questionService.UploadImageAsync(question.Id, model.ImageFile.FileName, model.ImageFile.ContentType, stream);
                }
            }

            TempData["Success"] = "Soru basariyla guncellendi";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string id)
        {
            var question = await _questionService.GetByIdAsync(id);
            if (question == null)
                return NotFound();

            return View(question);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _questionService.DeleteAsync(id);
            TempData["Success"] = "Soru basariyla silindi";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<JsonResult> GetUnitsByGrade(string gradeId)
        {
            var units = await _unitService.GetByGradeAsync(gradeId);
            return Json(units.Select(u => new { id = u.Id, name = u.Name }));
        }

        [HttpPost]
        public async Task<JsonResult> GetTopicsByUnit(string unitId)
        {
            var topics = await _topicService.GetByUnitAsync(unitId);
            return Json(topics.Select(t => new { id = t.Id, name = t.Name }));
        }
    }
}
