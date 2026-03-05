using JelleSmart.ExamSystem.Core.DTOs;
using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.Enums;
using JelleSmart.ExamSystem.Core.Interfaces.Repositories;
using JelleSmart.ExamSystem.Core.Interfaces.Services;

namespace JelleSmart.ExamSystem.Service.Services
{
    public class StudentExamService : IStudentExamService
    {
        private readonly IStudentExamRepository _studentExamRepository;
        private readonly IExamRepository _examRepository;
        private readonly IQuestionRepository _questionRepository;

        public StudentExamService(
            IStudentExamRepository studentExamRepository,
            IExamRepository examRepository,
            IQuestionRepository questionRepository)
        {
            _studentExamRepository = studentExamRepository;
            _examRepository = examRepository;
            _questionRepository = questionRepository;
        }

        public async Task<StudentExam?> GetByIdAsync(int id)
        {
            return await _studentExamRepository.GetByIdAsync(id);
        }

        public async Task<StudentExam?> GetWithAnswersAsync(int id)
        {
            return await _studentExamRepository.GetWithAnswersAsync(id);
        }

        public async Task<StudentExam?> GetByStudentAndExamAsync(string studentId, int examId)
        {
            return await _studentExamRepository.GetByStudentAndExamAsync(studentId, examId);
        }

        public async Task<IEnumerable<StudentExam>> GetByStudentAsync(string studentId)
        {
            return await _studentExamRepository.GetByStudentAsync(studentId);
        }

        public async Task<StudentExam> StartExamAsync(int examId, string studentId)
        {
            // Sınavı getir
            var exam = await _examRepository.GetWithQuestionsAsync(examId);
            if (exam == null)
                throw new ArgumentException("Sınav bulunamadı");

            if (!exam.IsActive)
                throw new InvalidOperationException("Sınav aktif değil");

            // Zaman kontrolü
            var now = DateTime.UtcNow;
            if (now < exam.StartTime)
                throw new InvalidOperationException("Sınav henüz başlamadı");
            if (now > exam.EndTime)
                throw new InvalidOperationException("Sınav süresi doldu");

            // Daha önce başlatılmış mı?
            var existingStudentExam = await _studentExamRepository.GetByStudentAndExamAsync(studentId, examId);
            if (existingStudentExam != null)
            {
                if (existingStudentExam.Status == ExamStatus.Completed)
                    throw new InvalidOperationException("Bu sınavı daha önce tamamladınız");

                if (existingStudentExam.Status == ExamStatus.InProgress)
                    return existingStudentExam; // Devam et
            }

            // Yeni StudentExam oluştur
            var studentExam = existingStudentExam ?? new StudentExam
            {
                ExamId = examId,
                StudentUserId = studentId,
                StartedAt = now,
                Status = ExamStatus.InProgress,
                RemainingTime = exam.Duration * 60 // Dakika -> saniye
            };

            if (existingStudentExam == null)
            {
                await _studentExamRepository.CreateAsync(studentExam);
            }
            else
            {
                await _studentExamRepository.UpdateAsync(studentExam);
            }

            return studentExam;
        }

        public async Task SubmitAnswerAsync(SubmitAnswerDto dto)
        {
            var studentExam = await _studentExamRepository.GetWithAnswersAsync(dto.StudentExamId);
            if (studentExam == null)
                throw new ArgumentException("Sınav bulunamadı");

            if (studentExam.Status != ExamStatus.InProgress)
                throw new InvalidOperationException("Sınav aktif değil");

            // Süre kontrolü
            var exam = await _examRepository.GetByIdAsync(studentExam.ExamId);
            if (exam == null)
                throw new ArgumentException("Sınav bulunamadı");

            var now = DateTime.UtcNow;
            if (now > exam.EndTime)
                throw new InvalidOperationException("Sınav süresi doldu");

            // Soruyu getir
            var question = await _questionRepository.GetWithChoicesAsync(dto.QuestionId);
            if (question == null)
                throw new ArgumentException("Soru bulunamadı");

            // Daha önce cevaplandı mı?
            var existingAnswer = studentExam.StudentAnswers
                .FirstOrDefault(sa => sa.QuestionId == dto.QuestionId);

            if (existingAnswer != null)
            {
                // Mevcut cevabı güncelle
                existingAnswer.ChoiceId = dto.ChoiceId;

                if (dto.ChoiceId.HasValue)
                {
                    var selectedChoice = question.Choices.FirstOrDefault(c => c.Id == dto.ChoiceId.Value);
                    existingAnswer.IsCorrect = selectedChoice?.IsCorrect ?? false;
                    existingAnswer.Points = existingAnswer.IsCorrect ? (exam.TotalPoints / exam.QuestionCount) : 0;
                }
                else
                {
                    existingAnswer.IsCorrect = false;
                    existingAnswer.Points = 0;
                }

                await _studentExamRepository.UpdateAsync(studentExam);
            }
            else
            {
                // Yeni cevap oluştur
                var studentAnswer = new StudentAnswer
                {
                    StudentExamId = dto.StudentExamId,
                    QuestionId = dto.QuestionId,
                    ChoiceId = dto.ChoiceId,
                    StudentUserId = dto.StudentUserId
                };

                if (dto.ChoiceId.HasValue)
                {
                    var selectedChoice = question.Choices.FirstOrDefault(c => c.Id == dto.ChoiceId.Value);
                    studentAnswer.IsCorrect = selectedChoice?.IsCorrect ?? false;
                    studentAnswer.Points = studentAnswer.IsCorrect ? (exam.TotalPoints / exam.QuestionCount) : 0;
                }

                studentExam.StudentAnswers.Add(studentAnswer);
                await _studentExamRepository.UpdateAsync(studentExam);
            }
        }

        public async Task<StudentExam> CompleteExamAsync(int studentExamId)
        {
            var studentExam = await _studentExamRepository.GetWithAnswersAsync(studentExamId);
            if (studentExam == null)
                throw new ArgumentException("Sınav bulunamadı");

            if (studentExam.Status != ExamStatus.InProgress)
                throw new InvalidOperationException("Sınav zaten tamamlandı");

            var exam = await _examRepository.GetWithQuestionsAsync(studentExam.ExamId);
            if (exam == null)
                throw new ArgumentException("Sınav bulunamadı");

            // Sonuçları hesapla
            studentExam.CompletedAt = DateTime.UtcNow;
            studentExam.Status = ExamStatus.Completed;

            var correctCount = studentExam.StudentAnswers.Count(sa => sa.IsCorrect);
            var wrongCount = studentExam.StudentAnswers.Count(sa => !sa.IsCorrect && sa.ChoiceId.HasValue);
            var answeredCount = studentExam.StudentAnswers.Count;
            var emptyCount = exam.QuestionCount - answeredCount;

            studentExam.CorrectCount = correctCount;
            studentExam.WrongCount = wrongCount;
            studentExam.EmptyCount = emptyCount;
            studentExam.Score = studentExam.StudentAnswers.Sum(sa => sa.Points);

            await _studentExamRepository.UpdateAsync(studentExam);

            return studentExam;
        }

        public async Task<ExamResultDto> GetExamResultAsync(int studentExamId)
        {
            var studentExam = await _studentExamRepository.GetWithAnswersAsync(studentExamId);
            if (studentExam == null)
                throw new ArgumentException("Sınav bulunamadı");

            var exam = await _examRepository.GetWithQuestionsAsync(studentExam.ExamId);
            if (exam == null)
                throw new ArgumentException("Sınav bulunamadı");

            return new ExamResultDto
            {
                ExamId = exam.Id,
                ExamName = exam.Name,
                SubjectName = exam.Subject.Name,
                Score = studentExam.Score,
                TotalPoints = exam.TotalPoints,
                Percentage = exam.TotalPoints > 0 ? (studentExam.Score / exam.TotalPoints) * 100 : 0,
                CorrectCount = studentExam.CorrectCount,
                WrongCount = studentExam.WrongCount,
                EmptyCount = studentExam.EmptyCount,
                CompletedAt = studentExam.CompletedAt
            };
        }
    }
}
