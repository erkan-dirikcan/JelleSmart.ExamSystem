using JelleSmart.ExamSystem.Core.Entities;

namespace JelleSmart.ExamSystem.Core.Interfaces.Services
{
    public interface ITopicService
    {
        Task<IEnumerable<Topic>> GetAllAsync();
        Task<Topic?> GetByIdAsync(int id);
        Task<IEnumerable<Topic>> GetByUnitAsync(int unitId);
        Task<Topic> CreateAsync(Topic topic);
        Task UpdateAsync(Topic topic);
        Task DeleteAsync(int id);
    }
}
