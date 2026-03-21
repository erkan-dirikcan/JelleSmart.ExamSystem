using JelleSmart.ExamSystem.Core.Entities;

namespace JelleSmart.ExamSystem.Core.Interfaces.Repositories
{
    public interface ISubjectGradeRepository
    {
        Task<IEnumerable<SubjectGrade>> GetAllAsync();
        Task<SubjectGrade?> GetByIdAsync(string subjectId, string gradeId);
        Task<SubjectGrade?> GetRelationAsync(string subjectId, string gradeId);
        Task AddAsync(SubjectGrade subjectGrade);
        Task DeleteAsync(SubjectGrade subjectGrade);
        Task<bool> ExistsAsync(string subjectId, string gradeId);
        Task<IEnumerable<Grade>> GetGradesBySubjectIdAsync(string subjectId);
        Task<IEnumerable<Subject>> GetSubjectsByGradeIdAsync(string gradeId);
    }
}
