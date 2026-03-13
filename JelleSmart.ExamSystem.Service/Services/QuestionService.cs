using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.Interfaces.Repositories;
using JelleSmart.ExamSystem.Core.Interfaces.Services;

namespace JelleSmart.ExamSystem.Service.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IFileService _fileService;

        public QuestionService(IQuestionRepository questionRepository, IFileService fileService)
        {
            _questionRepository = questionRepository;
            _fileService = fileService;
        }

        public async Task<IEnumerable<Question>> GetAllAsync()
        {
            return await _questionRepository.GetAllAsync();
        }

        public async Task<Question?> GetByIdAsync(string id)
        {
            return await _questionRepository.GetByIdAsync(id);
        }

        public async Task<Question?> GetWithChoicesAsync(string id)
        {
            return await _questionRepository.GetWithChoicesAsync(id);
        }

        public async Task<IEnumerable<Question>> GetBySubjectAsync(string subjectId)
        {
            return await _questionRepository.GetBySubjectAsync(subjectId);
        }

        public async Task<IEnumerable<Question>> GetByTeacherAsync(string teacherId)
        {
            return await _questionRepository.GetByTeacherAsync(teacherId);
        }

        public async Task<Question> CreateAsync(Question question)
        {
            return await _questionRepository.CreateAsync(question);
        }

        public async Task UpdateAsync(Question question)
        {
            await _questionRepository.UpdateAsync(question);
        }

        public async Task DeleteAsync(string id)
        {
            var question = await _questionRepository.GetWithChoicesAsync(id);
            if (question != null)
            {
                // Görsel varsa sil
                if (!string.IsNullOrEmpty(question.ImageUrl))
                {
                    await _fileService.DeleteFileAsync(question.ImageUrl);
                }

                await _questionRepository.DeleteAsync(id);
            }
        }

        public async Task<string> UploadImageAsync(string questionId, string fileName, string contentType, Stream stream)
        {
            var question = await _questionRepository.GetByIdAsync(questionId);
            if (question == null)
                throw new ArgumentException("Question not found");

            // Eski görseli sil
            if (!string.IsNullOrEmpty(question.ImageUrl))
            {
                await _fileService.DeleteFileAsync(question.ImageUrl);
            }

            var imagePath = await _fileService.UploadFileAsync(fileName, contentType, stream, "questions");
            question.ImageUrl = imagePath;
            await _questionRepository.UpdateAsync(question);

            return imagePath;
        }

        public async Task DeleteImageAsync(string questionId, string imagePath)
        {
            var question = await _questionRepository.GetByIdAsync(questionId);
            if (question == null)
                throw new ArgumentException("Question not found");

            await _fileService.DeleteFileAsync(imagePath);
            question.ImageUrl = null;
            await _questionRepository.UpdateAsync(question);
        }
    }
}
