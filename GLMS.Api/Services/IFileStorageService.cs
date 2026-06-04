using Microsoft.AspNetCore.Http;

namespace GLMS.Services
{
    public interface IFileStorageService
    {
        Task<(string? fileName, string? filepath)> SaveAsync(IFormFile? file, string subfolder);
        void Delete(string? relativePath);
    }
}
