using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.Interfaces.Repositories;
using JelleSmart.ExamSystem.Core.Interfaces.Services;

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

        public async Task<Grade?> GetByIdAsync(int id)
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

        public async Task DeleteAsync(int id)
        {
            await _gradeRepository.DeleteAsync(id);
        }
    }
}
