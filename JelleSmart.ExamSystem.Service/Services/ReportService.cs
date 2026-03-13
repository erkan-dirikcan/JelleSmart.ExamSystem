using JelleSmart.ExamSystem.Core.DTOs;
using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.Interfaces.Repositories;
using JelleSmart.ExamSystem.Core.Interfaces.Services;

namespace JelleSmart.ExamSystem.Service.Services
{
    public class ReportService : IReportService
    {
        private readonly IStudentExamRepository _studentExamRepository;
        private readonly IExamRepository _examRepository;
        private readonly IQuestionRepository _questionRepository;

        public ReportService(
            IStudentExamRepository studentExamRepository,
            IExamRepository examRepository,
            IQuestionRepository questionRepository)
        {
            _studentExamRepository = studentExamRepository;
            _examRepository = examRepository;
            _questionRepository = questionRepository;
        }

        public async Task<StudentReportDto> GetStudentReportAsync(string studentId)
        {
            var studentExams = await _studentExamRepository.GetByStudentAsync(studentId);
            var completedExams = studentExams.Where(se => se.Status == Core.Enums.ExamStatus.Completed).ToList();

            var report = new StudentReportDto
            {
                StudentName = completedExams.FirstOrDefault()?.Student.FullName ?? "",
                GradeName = completedExams.FirstOrDefault()?.Exam?.Grade?.Name ?? "",
                TotalExams = studentExams.Count(),
                CompletedExams = completedExams.Count,
                AverageScore = completedExams.Any() ? completedExams.Average(se => se.Score) : 0,
                AveragePercentage = completedExams.Any() ? completedExams.Average(se =>
                {
                    var totalPoints = se.Exam?.TotalPoints ?? 1;
                    return totalPoints > 0 ? (se.Score / totalPoints) * 100 : 0;
                }) : 0
            };

            // Ders bazlı performans
            var subjectGroups = completedExams
                .Where(se => se.Exam?.SubjectId != null)
                .GroupBy(se => se.Exam!.SubjectId!)
                .Select(g => new SubjectPerformanceDto
                {
                    SubjectId = g.Key,
                    SubjectName = g.FirstOrDefault()?.Exam?.Subject?.Name ?? "",
                    AverageScore = g.Average(se => se.Score),
                    ExamCount = g.Count(),
                    AveragePercentage = g.Average(se =>
                    {
                        var totalPoints = se.Exam?.TotalPoints ?? 1;
                        return totalPoints > 0 ? (se.Score / totalPoints) * 100 : 0;
                    })
                }).ToList();

            report.SubjectPerformances = subjectGroups;

            // Tüm sınav sonuçları
            report.ExamResults = completedExams.Select(se => new ExamResultDto
            {
                ExamId = se.ExamId,
                ExamName = se.Exam?.Name ?? "",
                SubjectName = se.Exam?.Subject?.Name ?? "",
                Score = se.Score,
                TotalPoints = se.Exam?.TotalPoints ?? 0,
                Percentage = se.Exam?.TotalPoints > 0 ? (se.Score / (se.Exam?.TotalPoints ?? 1)) * 100 : 0,
                CorrectCount = se.CorrectCount,
                WrongCount = se.WrongCount,
                EmptyCount = se.EmptyCount,
                CompletedAt = se.CompletedAt
            }).ToList();

            return report;
        }

        public async Task<IEnumerable<ExamResultDto>> GetStudentExamResultsAsync(string studentId)
        {
            var studentExams = await _studentExamRepository.GetByStudentAsync(studentId);
            var completedExams = studentExams.Where(se => se.Status == Core.Enums.ExamStatus.Completed).ToList();

            return completedExams.Select(se => new ExamResultDto
            {
                ExamId = se.ExamId,
                ExamName = se.Exam?.Name ?? "",
                SubjectName = se.Exam?.Subject?.Name ?? "",
                Score = se.Score,
                TotalPoints = se.Exam?.TotalPoints ?? 0,
                Percentage = se.Exam?.TotalPoints > 0 ? (se.Score / (se.Exam?.TotalPoints ?? 1)) * 100 : 0,
                CorrectCount = se.CorrectCount,
                WrongCount = se.WrongCount,
                EmptyCount = se.EmptyCount,
                CompletedAt = se.CompletedAt
            }).ToList();
        }

        public async Task<IEnumerable<ExamResultDto>> GetExamResultsByExamAsync(string examId)
        {
            var studentExams = await _studentExamRepository.GetByExamAsync(examId);
            var completedExams = studentExams.Where(se => se.Status == Core.Enums.ExamStatus.Completed).ToList();

            return completedExams.Select(se => new ExamResultDto
            {
                ExamId = se.ExamId,
                ExamName = se.Exam?.Name ?? "",
                SubjectName = se.Exam?.Subject?.Name ?? "",
                Score = se.Score,
                TotalPoints = se.Exam?.TotalPoints ?? 0,
                Percentage = se.Exam?.TotalPoints > 0 ? (se.Score / (se.Exam?.TotalPoints ?? 1)) * 100 : 0,
                CorrectCount = se.CorrectCount,
                WrongCount = se.WrongCount,
                EmptyCount = se.EmptyCount,
                CompletedAt = se.CompletedAt
            }).ToList();
        }

        public async Task<StudentPerformanceDto> GetStudentPerformanceAsync(string studentId, string? subjectId)
        {
            var studentExams = await _studentExamRepository.GetByStudentAsync(studentId);
            var subjectExams = studentExams
                .Where(se => se.Exam?.SubjectId == subjectId && se.Status == Core.Enums.ExamStatus.Completed)
                .ToList();

            var performance = new StudentPerformanceDto
            {
                StudentName = studentExams.FirstOrDefault()?.Student.FullName ?? "",
                SubjectName = subjectExams.FirstOrDefault()?.Exam?.Subject?.Name ?? "",
                TotalExams = subjectExams.Count,
                CompletedExams = subjectExams.Count,
                AverageScore = subjectExams.Any() ? subjectExams.Average(se => se.Score) : 0,
                AveragePercentage = subjectExams.Any() ? subjectExams.Average(se =>
                {
                    var totalPoints = se.Exam?.TotalPoints ?? 1;
                    return totalPoints > 0 ? (se.Score / totalPoints) * 100 : 0;
                }) : 0
            };

            // Kazanım bazlı performans
            var studentExamIds = subjectExams.Select(se => se.Id).ToList();
            // Burada StudentAnswer'ları getirip kazanım bazlı gruplama yapılabilir
            // Şimdilik boş liste dönüyoruz
            performance.TopicPerformances = new List<TopicPerformanceDto>();

            return performance;
        }
    }
}
