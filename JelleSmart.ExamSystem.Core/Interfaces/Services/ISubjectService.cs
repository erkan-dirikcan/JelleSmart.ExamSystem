using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.ViewModels;

namespace JelleSmart.ExamSystem.Core.Interfaces.Services
{
    public interface ISubjectService
    {
        Task<IEnumerable<Subject>> GetAllAsync();
        Task<Subject?> GetByIdAsync(string id);
        Task<Subject> CreateAsync(Subject subject);
        Task UpdateAsync(Subject subject);
        Task DeleteAsync(string id);

        // ViewModel methods for WebUI
        Task<IEnumerable<SubjectViewModel>> GetAllViewModelAsync();
        Task<SubjectViewModel?> GetViewModelByIdAsync(string id);
        Task<string> CreateViewModelAsync(SubjectViewModel viewModel);
        Task UpdateViewModelAsync(SubjectViewModel viewModel);
    }
}
