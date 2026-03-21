using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.Interfaces.Repositories;
using JelleSmart.ExamSystem.Core.Interfaces.Services;
using JelleSmart.ExamSystem.Core.ViewModels;

namespace JelleSmart.ExamSystem.Service.Services
{
    public class UnitService : IUnitService
    {
        private readonly IUnitRepository _unitRepository;

        public UnitService(IUnitRepository unitRepository)
        {
            _unitRepository = unitRepository;
        }

        public async Task<IEnumerable<Unit>> GetAllAsync()
        {
            return await _unitRepository.GetAllAsync();
        }

        public async Task<Unit?> GetByIdAsync(string id)
        {
            return await _unitRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Unit>> GetByGradeAsync(string gradeId)
        {
            return await _unitRepository.GetByGradeAsync(gradeId);
        }

        public async Task<Unit> CreateAsync(Unit unit)
        {
            return await _unitRepository.CreateAsync(unit);
        }

        public async Task UpdateAsync(Unit unit)
        {
            await _unitRepository.UpdateAsync(unit);
        }

        public async Task DeleteAsync(string id)
        {
            await _unitRepository.DeleteAsync(id);
        }

        // ViewModel methods for WebUI
        public async Task<IEnumerable<UnitViewModel>> GetAllViewModelAsync()
        {
            var entities = await _unitRepository.GetAllWithIncludesAsync();
            return entities.Select(e => new UnitViewModel
            {
                Id = e.Id,
                Name = e.Name,
                GradeId = e.GradeId,
                Description = e.Description,
                GradeName = e.Grade?.Name
            }).ToList();
        }

        public async Task<UnitViewModel?> GetViewModelByIdAsync(string id)
        {
            var entity = await _unitRepository.GetByIdWithIncludesAsync(id);
            if (entity == null)
                return null;

            return new UnitViewModel
            {
                Id = entity.Id,
                Name = entity.Name,
                GradeId = entity.GradeId,
                Description = entity.Description,
                GradeName = entity.Grade?.Name
            };
        }

        public async Task<string> CreateViewModelAsync(UnitViewModel viewModel)
        {
            var entity = new Unit
            {
                Name = viewModel.Name,
                GradeId = viewModel.GradeId,
                Description = viewModel.Description
            };
            var result = await _unitRepository.CreateAsync(entity);
            return result.Id!;
        }

        public async Task UpdateViewModelAsync(UnitViewModel viewModel)
        {
            var entity = await _unitRepository.GetByIdAsync(viewModel.Id!);
            if (entity == null)
                throw new Exception("Unit not found");

            entity.Name = viewModel.Name;
            entity.GradeId = viewModel.GradeId;
            entity.Description = viewModel.Description;

            await _unitRepository.UpdateAsync(entity);
        }
    }
}
