using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.ViewModels;

namespace JelleSmart.ExamSystem.Core.Interfaces.Services
{
    public interface IUnitService
    {
        Task<IEnumerable<Unit>> GetAllAsync();
        Task<Unit?> GetByIdAsync(string id);
        Task<IEnumerable<Unit>> GetBySubjectAsync(string subjectId);
        Task<Unit> CreateAsync(Unit unit);
        Task UpdateAsync(Unit unit);
        Task DeleteAsync(string id);

        // ViewModel methods for WebUI
        Task<IEnumerable<UnitViewModel>> GetAllViewModelAsync();
        Task<UnitViewModel?> GetViewModelByIdAsync(string id);
        Task<string> CreateViewModelAsync(UnitViewModel viewModel);
        Task UpdateViewModelAsync(UnitViewModel viewModel);
    }
}
