using GLMS.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Text;
using Xunit;

namespace GLMS.Tests.Services
{
    public class LocalFileStorageServiceTests : IDisposable
    {
        private readonly string _tempRoot;
        private readonly LocalFileStorageService _service;

        public LocalFileStorageServiceTests()
        {

            _tempRoot = Path.Combine(Path.GetTempPath(), $"GLMS_Tests_{Guid.NewGuid()}");
            Directory.CreateDirectory(_tempRoot);

            var envMock = new Mock<IWebHostEnvironment>();
            envMock.SetupGet(e => e.WebRootPath).Returns(_tempRoot);

            _service = new LocalFileStorageService(envMock.Object);
        }
        public void Dispose()
        {
            if (Directory.Exists(_tempRoot))
            {
                Directory.Delete(_tempRoot, recursive: true);
            }
        }

        private static IFormFile MakeFormFile(string fileName, long sizeBytes, string contentType = "application/pdf")
        {
            var content = Encoding.UTF8.GetBytes(new string('A', (int)Math.Min(sizeBytes, int.MaxValue)));
            var stream = new MemoryStream(content);
            var file = new FormFile(stream, 0, content.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };
            return file;
        }

        //Test it works when a valid file is submitted

        [Fact]
        public async Task SaveAsync_ValidPdf_ReturnsFileInfoAndWritesFile()
        {
            var file = MakeFormFile("Agreement.pdf", sizeBytes: 1024);

            var (fileName, filePath) = await _service.SaveAsync(file, "contracts");
            Assert.Equal("Agreement.pdf", fileName);
            Assert.NotNull(filePath);
            Assert.StartsWith("/uploads/contracts/", filePath);

            var fullPath = Path.Combine(_tempRoot, filePath!.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            Assert.True(File.Exists(fullPath));
        }

        //Test it properly stops invalid file types (.exe)

        [Fact]
        public async Task SaveAsync_ExeFile_ThrowsFileValidationException()
        {
            var file = MakeFormFile("Agreement.exe", sizeBytes: 1024);
            var ex = await Assert.ThrowsAsync<FileValidationException>(
                () => _service.SaveAsync(file, "contracts"));
            Assert.Contains("Only", ex.Message);
        }

        [Theory]
        [InlineData("document.docx")]
        [InlineData("spreadsheet.xlsx")]
        [InlineData("script.js")]
        [InlineData("image.png")]
        [InlineData("archive.zip")]
        [InlineData("executable.exe")]
        [InlineData("no_extension")]
        public async Task SaveAsync_NonPdfExtensions_Throws(string fileName)
        {
            var file = MakeFormFile(fileName, sizeBytes: 1024);

            await Assert.ThrowsAsync<FileValidationException>(
                () => _service.SaveAsync(file, "contracts"));
        }

        // Test it stops files that are too big from being uploaded

        [Fact]
        public async Task SaveAsync_OversizedPdf_ThrowsFileValidationException()
        {
            var file = MakeFormFile("huge.pdf", sizeBytes: 11 * 1024 * 1024);
            var ex = await Assert.ThrowsAsync<FileValidationException>(
                () => _service.SaveAsync(file, "contracts"));
            Assert.Contains("maximum size", ex.Message);
        }

        // Test it doesnt break when there isnt a file

        [Fact]
        public async Task SaveAsync_NullFile_ReturnsEmptyTuple()
        {
            var (fileName, filePath) = await _service.SaveAsync(null, "contracts");
            Assert.Null(fileName);
            Assert.Null(filePath);
        }

        [Fact]
        public async Task SaveAsync_ZeroByteFile_ReturnsEmptyTuple()
        {
            var file = MakeFormFile("empty.pdf", sizeBytes: 0);

            // Act
            var (fileName, filePath) = await _service.SaveAsync(file, "contracts");

            Assert.Null(fileName);
            Assert.Null(filePath);
        }

        // Test deleting works

        [Fact]
        public void Delete_ExistingFile_RemovesFromDisk()
        {
            var subFolder = Path.Combine(_tempRoot, "uploads", "contracts");
            Directory.CreateDirectory(subFolder);
            var filePath = Path.Combine(subFolder, "to_delete.pdf");
            File.WriteAllText(filePath, "test");
            Assert.True(File.Exists(filePath));

            _service.Delete("/uploads/contracts/to_delete.pdf");
            Assert.False(File.Exists(filePath));
        }

        [Fact]
        public void Delete_NullPath_DoesNotThrow()
        {
            _service.Delete(null);
            _service.Delete(string.Empty);
            _service.Delete("   ");
        }

        [Fact]
        public void Delete_NonExistentFile_DoesNotThrow()
        {
            _service.Delete("/uploads/contracts/never_existed.pdf");
        }
    }
}