using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.ViewModels;

namespace JelleSmart.ExamSystem.Core.Interfaces.Services
{
    public interface ITopicService
    {
        Task<IEnumerable<Topic>> GetAllAsync();
        Task<Topic?> GetByIdAsync(string id);
        Task<IEnumerable<Topic>> GetByUnitAsync(string unitId);
        Task<Topic> CreateAsync(Topic topic);
        Task UpdateAsync(Topic topic);
        Task DeleteAsync(string id);

        // ViewModel methods for WebUI
        Task<IEnumerable<TopicViewModel>> GetAllViewModelAsync();
        Task<TopicViewModel?> GetViewModelByIdAsync(string id);
        Task<string> CreateViewModelAsync(TopicViewModel viewModel);
        Task UpdateViewModelAsync(TopicViewModel viewModel);
    }
}
