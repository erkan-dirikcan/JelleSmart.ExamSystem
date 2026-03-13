using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.Interfaces.Repositories;
using JelleSmart.ExamSystem.Core.Interfaces.Services;
using JelleSmart.ExamSystem.Core.ViewModels;

namespace JelleSmart.ExamSystem.Service.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly ISubjectRepository _subjectRepository;

        public SubjectService(ISubjectRepository subjectRepository)
        {
            _subjectRepository = subjectRepository;
        }

        public async Task<IEnumerable<Subject>> GetAllAsync()
        {
            return await _subjectRepository.GetAllAsync();
        }

        public async Task<Subject?> GetByIdAsync(string id)
        {
            return await _subjectRepository.GetByIdAsync(id);
        }

        public async Task<Subject> CreateAsync(Subject subject)
        {
            return await _subjectRepository.CreateAsync(subject);
        }

        public async Task UpdateAsync(Subject subject)
        {
            await _subjectRepository.UpdateAsync(subject);
        }

        public async Task DeleteAsync(string id)
        {
            await _subjectRepository.DeleteAsync(id);
        }

        // ViewModel methods for WebUI
        public async Task<IEnumerable<SubjectViewModel>> GetAllViewModelAsync()
        {
            var entities = await _subjectRepository.GetAllAsync();
            return entities.Select(e => new SubjectViewModel
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                IconClass = e.IconClass
            }).ToList();
        }

        public async Task<SubjectViewModel?> GetViewModelByIdAsync(string id)
        {
            var entity = await _subjectRepository.GetByIdAsync(id);
            if (entity == null)
                return null;

            return new SubjectViewModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                IconClass = entity.IconClass
            };
        }

        public async Task<string> CreateViewModelAsync(SubjectViewModel viewModel)
        {
            var entity = new Subject
            {
                Name = viewModel.Name,
                Description = viewModel.Description,
                IconClass = viewModel.IconClass
            };
            var result = await _subjectRepository.CreateAsync(entity);
            return result.Id!;
        }

        public async Task UpdateViewModelAsync(SubjectViewModel viewModel)
        {
            var entity = await _subjectRepository.GetByIdAsync(viewModel.Id!);
            if (entity == null)
                throw new Exception("Subject not found");

            entity.Name = viewModel.Name;
            entity.Description = viewModel.Description;
            entity.IconClass = viewModel.IconClass;

            await _subjectRepository.UpdateAsync(entity);
        }
    }
}
