using JelleSmart.ExamSystem.Core.Interfaces.Services;
using JelleSmart.ExamSystem.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JelleSmart.ExamSystem.WebUI.Controllers
{
    [Authorize(Roles = UserRoles.Student)]
    public class StudentReportController : Controller
    {
        private readonly IReportService _reportService;
        private readonly IExamService _examService;

        public StudentReportController(IReportService reportService, IExamService examService)
        {
            _reportService = reportService;
            _examService = examService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var report = await _reportService.GetStudentReportAsync(userId!);
            return View(report);
        }

        public async Task<IActionResult> ExamResults()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var results = await _reportService.GetStudentExamResultsAsync(userId!);
            return View(results);
        }

        public async Task<IActionResult> SubjectPerformance(int subjectId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var performance = await _reportService.GetStudentPerformanceAsync(userId!, subjectId);
            return View(performance);
        }
    }
}
