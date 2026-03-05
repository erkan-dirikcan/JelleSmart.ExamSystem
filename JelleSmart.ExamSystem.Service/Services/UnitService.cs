using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.Interfaces.Repositories;
using JelleSmart.ExamSystem.Core.Interfaces.Services;

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

        public async Task<Unit?> GetByIdAsync(int id)
        {
            return await _unitRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Unit>> GetBySubjectAsync(int subjectId)
        {
            return await _unitRepository.GetBySubjectAsync(subjectId);
        }

        public async Task<Unit> CreateAsync(Unit unit)
        {
            return await _unitRepository.CreateAsync(unit);
        }

        public async Task UpdateAsync(Unit unit)
        {
            await _unitRepository.UpdateAsync(unit);
        }

        public async Task DeleteAsync(int id)
        {
            await _unitRepository.DeleteAsync(id);
        }
    }
}
