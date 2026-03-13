using JelleSmart.ExamSystem.Core.DTOs;
using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.Interfaces.Repositories;
using JelleSmart.ExamSystem.Core.Interfaces.Services;

namespace JelleSmart.ExamSystem.Service.Services
{
    public class ExamService : IExamService
    {
        private readonly IExamRepository _examRepository;
        private readonly IQuestionRepository _questionRepository;

        public ExamService(IExamRepository examRepository, IQuestionRepository questionRepository)
        {
            _examRepository = examRepository;
            _questionRepository = questionRepository;
        }

        public async Task<IEnumerable<Exam>> GetAllAsync()
        {
            return await _examRepository.GetAllAsync();
        }

        public async Task<Exam?> GetByIdAsync(string id)
        {
            return await _examRepository.GetByIdAsync(id);
        }

        public async Task<Exam?> GetWithQuestionsAsync(string id)
        {
            return await _examRepository.GetWithQuestionsAsync(id);
        }

        public async Task<IEnumerable<Exam>> GetByTeacherAsync(string teacherId)
        {
            return await _examRepository.GetByTeacherAsync(teacherId);
        }

        public async Task<Exam> CreateAsync(CreateExamDto dto)
        {
            // Seçilen kazanımlardan rastgele sorular seç
            var questions = await _questionRepository.GetRandomQuestionsAsync(
                dto.SubjectId,
                null, // UnitId will be filtered by topics
                null,
                dto.QuestionCount);

            if (!questions.Any())
            {
                throw new InvalidOperationException("Seçilen kriterlere uygun soru bulunamadı.");
            }

            var exam = new Exam
            {
                Name = dto.Name,
                Description = dto.Description,
                Duration = dto.Duration,
                QuestionCount = dto.QuestionCount,
                TotalPoints = dto.TotalPoints,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                IsActive = false,
                Status = Core.Enums.ExamStatus.NotStarted,
                GradeId = dto.GradeId,
                SubjectId = dto.SubjectId,
                TopicIds = string.Join(",", dto.TopicIds),
                CreatedByUserId = dto.CreatedByUserId
            };

            var createdExam = await _examRepository.CreateAsync(exam);

            // Sınav sorularını ekle
            var examQuestions = questions.Select((q, index) => new ExamQuestion
            {
                ExamId = createdExam.Id!,
                QuestionId = q.Id!,
                Order = index + 1,
                Points = dto.TotalPoints / dto.QuestionCount
            }).ToList();

            // ExamQuestion'ları manuel eklemeliyiz
            foreach (var eq in examQuestions)
            {
                createdExam.ExamQuestions.Add(eq);
            }

            await _examRepository.UpdateAsync(createdExam);

            return createdExam;
        }

        public async Task UpdateAsync(Exam exam)
        {
            await _examRepository.UpdateAsync(exam);
        }

        public async Task DeleteAsync(string id)
        {
            await _examRepository.DeleteAsync(id);
        }

        public async Task ActivateExamAsync(string examId)
        {
            var exam = await _examRepository.GetByIdAsync(examId);
            if (exam == null)
                throw new ArgumentException("Sınav bulunamadı");

            exam.IsActive = true;
            await _examRepository.UpdateAsync(exam);
        }

        public async Task DeactivateExamAsync(string examId)
        {
            var exam = await _examRepository.GetByIdAsync(examId);
            if (exam == null)
                throw new ArgumentException("Sınav bulunamadı");

            exam.IsActive = false;
            await _examRepository.UpdateAsync(exam);
        }

        public async Task<Exam?> GetExamForStudentAsync(string examId, string studentId)
        {
            var exam = await _examRepository.GetWithQuestionsAsync(examId);

            if (exam == null || !exam.IsActive)
                return null;

            // Öğrenci daha önce bu sınavı çözdü mü?
            // Bu kontrol StudentExam servisinde yapılacak

            return exam;
        }
    }
}
