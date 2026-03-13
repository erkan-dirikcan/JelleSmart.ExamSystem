using JelleSmart.ExamSystem.Core.Entities;

namespace JelleSmart.ExamSystem.Core.Interfaces.Repositories
{
    public interface ITopicRepository : IRepository<Topic>
    {
        Task<IEnumerable<Topic>> GetByUnitAsync(string unitId);
        Task<Topic?> GetByIdWithIncludesAsync(string id);
        Task<IEnumerable<Topic>> GetAllWithIncludesAsync();
    }
}
