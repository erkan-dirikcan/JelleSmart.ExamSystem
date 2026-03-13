using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.Interfaces.Repositories;
using JelleSmart.ExamSystem.Core.Interfaces.Services;
using JelleSmart.ExamSystem.Core.ViewModels;

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

        public async Task<Topic?> GetByIdAsync(string id)
        {
            return await _topicRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Topic>> GetByUnitAsync(string unitId)
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

        public async Task DeleteAsync(string id)
        {
            await _topicRepository.DeleteAsync(id);
        }

        // ViewModel methods for WebUI
        public async Task<IEnumerable<TopicViewModel>> GetAllViewModelAsync()
        {
            var entities = await _topicRepository.GetAllWithIncludesAsync();
            return entities.Select(e => new TopicViewModel
            {
                Id = e.Id,
                Name = e.Name,
                UnitId = e.UnitId,
                Code = e.Code,
                Description = e.Description,
                UnitName = e.Unit?.Name,
                SubjectName = e.Unit?.Subject?.Name
            }).ToList();
        }

        public async Task<TopicViewModel?> GetViewModelByIdAsync(string id)
        {
            var entity = await _topicRepository.GetByIdWithIncludesAsync(id);
            if (entity == null)
                return null;

            return new TopicViewModel
            {
                Id = entity.Id,
                Name = entity.Name,
                UnitId = entity.UnitId,
                Code = entity.Code,
                Description = entity.Description,
                UnitName = entity.Unit?.Name,
                SubjectName = entity.Unit?.Subject?.Name
            };
        }

        public async Task<string> CreateViewModelAsync(TopicViewModel viewModel)
        {
            var entity = new Topic
            {
                Name = viewModel.Name,
                UnitId = viewModel.UnitId,
                Code = viewModel.Code,
                Description = viewModel.Description
            };
            var result = await _topicRepository.CreateAsync(entity);
            return result.Id!;
        }

        public async Task UpdateViewModelAsync(TopicViewModel viewModel)
        {
            var entity = await _topicRepository.GetByIdAsync(viewModel.Id!);
            if (entity == null)
                throw new Exception("Topic not found");

            entity.Name = viewModel.Name;
            entity.UnitId = viewModel.UnitId;
            entity.Code = viewModel.Code;
            entity.Description = viewModel.Description;

            await _topicRepository.UpdateAsync(entity);
        }
    }
}
