using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.Interfaces.Repositories;
using JelleSmart.ExamSystem.Core.Interfaces.Services;

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

        public async Task<Subject?> GetByIdAsync(int id)
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

        public async Task DeleteAsync(int id)
        {
            await _subjectRepository.DeleteAsync(id);
        }
    }
}
