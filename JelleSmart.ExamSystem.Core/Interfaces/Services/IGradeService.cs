using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.ViewModels;

namespace JelleSmart.ExamSystem.Core.Interfaces.Services
{
    public interface IGradeService
    {
        Task<IEnumerable<Grade>> GetAllAsync();
        Task<Grade?> GetByIdAsync(string id);
        Task<Grade> CreateAsync(Grade grade);
        Task UpdateAsync(Grade grade);
        Task DeleteAsync(string id);

        // ViewModel methods for WebUI
        Task<IEnumerable<GradeViewModel>> GetAllViewModelAsync();
        Task<GradeViewModel?> GetViewModelByIdAsync(string id);
        Task<string> CreateViewModelAsync(GradeViewModel viewModel);
        Task UpdateViewModelAsync(GradeViewModel viewModel);
    }
}
