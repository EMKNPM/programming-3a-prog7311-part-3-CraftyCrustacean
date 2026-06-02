namespace GLMS.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        private const long MaxFileSizeBytes = 10 * 1024 * 1024;
        private static readonly string[] AllowedExtensions = { ".pdf" };
        private readonly IWebHostEnvironment _environment;
         public LocalFileStorageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<(string? fileName, string? filepath)> SaveAsync(IFormFile? file, string subfolder)
        { 
            if (file == null || file.Length == 0)
            {
                return (null, null);
            }

            ValidateFile(file);

            string uniqueName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", subfolder);
            Directory.CreateDirectory(uploadsFolder);

            string fullPath = Path.Combine(uploadsFolder, uniqueName);
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            string relativePath = $"/uploads/{subfolder}/{uniqueName}";
            return (file.FileName, relativePath);
        }

        public void Delete(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return;

            string fullPath = Path.Combine(_environment.WebRootPath, relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        private static void ValidateFile(IFormFile file)
        {
            string extension = Path.GetExtension(file.FileName);

            if (!AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            {
                throw new FileValidationException($"Only {string.Join(", ", AllowedExtensions)} files are allowed");
            }

            if (file.Length > MaxFileSizeBytes)
            {
                throw new FileValidationException($"File exceeds the maximum size of {MaxFileSizeBytes / (1024 * 1024)} MB.");
            }
        }
    }
}
