using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.Interfaces.Repositories;
using JelleSmart.ExamSystem.Core.Interfaces.Services;

namespace JelleSmart.ExamSystem.Service.Services
{
    public class TopicService : ITopicService
    {
        private readonly ITopicRepository _topicRepository;

        public TopicService(ITopicRepository topicRepository)
        {
            _topicRepository = topicRepository;
        }

        public async Task<IEnumerable<Topic>> GetAllAsync()
        {
            return await _topicRepository.GetAllAsync();
        }

        public async Task<Topic?> GetByIdAsync(int id)
        {
            return await _topicRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Topic>> GetByUnitAsync(int unitId)
        {
            return await _topicRepository.GetByUnitAsync(unitId);
        }

        public async Task<Topic> CreateAsync(Topic topic)
        {
            return await _topicRepository.CreateAsync(topic);
        }

        public async Task UpdateAsync(Topic topic)
        {
            await _topicRepository.UpdateAsync(topic);
        }

        public async Task DeleteAsync(int id)
        {
            await _topicRepository.DeleteAsync(id);
        }
    }
}
