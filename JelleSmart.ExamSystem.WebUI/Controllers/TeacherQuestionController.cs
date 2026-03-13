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
                SubjectId = model.SubjectId,
                UnitId = model.UnitId,
                TopicId = model.TopicId,
                GradeId = model.GradeId,
                CreatedByUserId = userId!
            };

            // Ã–nce soruyu kaydet
            question = await _questionService.CreateAsync(question);

            // ÅÄ±klarÄ± ekle
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

            // GÃ¶rsel varsa yÃ¼kle
            if (model.ImageFile != null)
            {
                using (var stream = model.ImageFile.OpenReadStream())
                {
                    await _questionService.UploadImageAsync(question.Id, model.ImageFile.FileName, model.ImageFile.ContentType, stream);
                }
            }

            TempData["Success"] = "Soru baÅŸarÄ±yla eklendi";
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
                SubjectId = question.SubjectId,
                UnitId = question.UnitId,
                TopicId = question.TopicId,
                GradeId = question.GradeId,
                Choices = question.Choices.Select(c => new ChoiceViewModel
                {
                    Id = c.Id,
                    Text = c.Text,
                    IsCorrect = c.IsCorrect
                }).ToList()
            };

            ViewBag.Subjects = await _subjectService.GetAllAsync();
            ViewBag.Grades = await _gradeService.GetAllAsync();
            if (!string.IsNullOrEmpty(question.UnitId))
                ViewBag.Units = await _unitService.GetBySubjectAsync(question.SubjectId!);
            if (!string.IsNullOrEmpty(question.TopicId))
                ViewBag.Topics = await _topicService.GetByUnitAsync(question.UnitId!);

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
            question.SubjectId = model.SubjectId;
            question.UnitId = model.UnitId;
            question.TopicId = model.TopicId;
            question.GradeId = model.GradeId;

            // ÅÄ±klarÄ± gÃ¼ncelle
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

            // Yeni gÃ¶rsel varsa yÃ¼kle
            if (model.ImageFile != null)
            {
                using (var stream = model.ImageFile.OpenReadStream())
                {
                    await _questionService.UploadImageAsync(question.Id, model.ImageFile.FileName, model.ImageFile.ContentType, stream);
                }
            }

            TempData["Success"] = "Soru baÅŸarÄ±yla gÃ¼ncellendi";
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
            TempData["Success"] = "Soru baÅŸarÄ±yla silindi";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<JsonResult> GetUnitsBySubject(string subjectId)
        {
            var units = await _unitService.GetBySubjectAsync(subjectId);
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
