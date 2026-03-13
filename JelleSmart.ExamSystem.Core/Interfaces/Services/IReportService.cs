using JelleSmart.ExamSystem.Core.DTOs;

namespace JelleSmart.ExamSystem.Core.Interfaces.Services
{
    public interface IReportService
    {
        Task<StudentReportDto> GetStudentReportAsync(string studentId);
        Task<IEnumerable<ExamResultDto>> GetStudentExamResultsAsync(string studentId);
        Task<IEnumerable<ExamResultDto>> GetExamResultsByExamAsync(string examId);
        Task<StudentPerformanceDto> GetStudentPerformanceAsync(string studentId, string? subjectId);
    }
}
