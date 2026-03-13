using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.Interfaces.Repositories;
using JelleSmart.ExamSystem.Core.Interfaces.Services;
using JelleSmart.ExamSystem.Core.ViewModels;

namespace JelleSmart.ExamSystem.Service.Services
{
    public class GradeService : IGradeService
    {
        private readonly IGradeRepository _gradeRepository;

        public GradeService(IGradeRepository gradeRepository)
        {
            _gradeRepository = gradeRepository;
        }

        public async Task<IEnumerable<Grade>> GetAllAsync()
        {
            return await _gradeRepository.GetAllAsync();
        }

        public async Task<Grade?> GetByIdAsync(string id)
        {
            return await _gradeRepository.GetByIdAsync(id);
        }

        public async Task<Grade> CreateAsync(Grade grade)
        {
            return await _gradeRepository.CreateAsync(grade);
        }

        public async Task UpdateAsync(Grade grade)
        {
            await _gradeRepository.UpdateAsync(grade);
        }

        public async Task DeleteAsync(string id)
        {
            await _gradeRepository.DeleteAsync(id);
        }

        // ViewModel methods for WebUI
        public async Task<IEnumerable<GradeViewModel>> GetAllViewModelAsync()
        {
            var entities = await _gradeRepository.GetAllAsync();
            return entities.Select(e => new GradeViewModel
            {
                Id = e.Id,
                Name = e.Name,
                Level = e.Level
            }).ToList();
        }

        public async Task<GradeViewModel?> GetViewModelByIdAsync(string id)
        {
            var entity = await _gradeRepository.GetByIdAsync(id);
            if (entity == null)
                return null;

            return new GradeViewModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Level = entity.Level
            };
        }

        public async Task<string> CreateViewModelAsync(GradeViewModel viewModel)
        {
            var entity = new Grade
            {
                Name = viewModel.Name,
                Level = viewModel.Level
            };
            var result = await _gradeRepository.CreateAsync(entity);
            return result.Id!;
        }

        public async Task UpdateViewModelAsync(GradeViewModel viewModel)
        {
            var entity = await _gradeRepository.GetByIdAsync(viewModel.Id!);
            if (entity == null)
                throw new Exception("Grade not found");

            entity.Name = viewModel.Name;
            entity.Level = viewModel.Level;

            await _gradeRepository.UpdateAsync(entity);
        }
    }
}
