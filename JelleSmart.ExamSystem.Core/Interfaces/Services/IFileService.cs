namespace JelleSmart.ExamSystem.Core.Interfaces.Services
{
    public interface IFileService
    {
        Task<string> UploadFileAsync(string fileName, string contentType, Stream stream, string folder);
        Task DeleteFileAsync(string filePath);
        bool FileExists(string filePath);
        string GetUniqueFileName(string fileName);
    }
}
