using FolderSynchronization.Validators;
using FolderSynchronization.Exceptions;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;


namespace FolderSynchronization.Tests.Validators
{
    public class AccessValidatorTests : IDisposable
    {
        private readonly MockFileSystem _mockFileSystem;
        private readonly string _testDirectory;

        public AccessValidatorTests()
        {
            _mockFileSystem = new MockFileSystem();
            AccessValidator.SetFileSystem(_mockFileSystem);
            _testDirectory = Path.Combine(Path.GetTempPath(), $"AccessValidatorTests_{Guid.NewGuid()}");
            _mockFileSystem.AddDirectory(_testDirectory);
        }

        public void Dispose()
        {
            if (_mockFileSystem.Directory.Exists(_testDirectory))
            {
                try
                {
                    _mockFileSystem.Directory.Delete(_testDirectory, recursive: true);
                }
                catch (IOException)
                {
                    foreach (var file in _mockFileSystem.Directory.GetFiles(_testDirectory, "*.*", SearchOption.AllDirectories))
                    {
                        _mockFileSystem.File.Delete(file);
                    }
                    foreach (var dir in _mockFileSystem.Directory.GetDirectories(_testDirectory, "*", SearchOption.AllDirectories))
                    {
                        _mockFileSystem.Directory.Delete(dir);
                    }
                }
            }
            AccessValidator.ResetFileSystem();
        }

        [Fact]
        public void ValidateReadAccess_WithValidPath_DoesNotThrowException()
        {
            _mockFileSystem.AddFile(Path.Combine(_testDirectory, "test.txt"), new MockFileData(""));

            var exception = Record.Exception(() => AccessValidator.ValidateReadAccess(_testDirectory));

            Assert.Null(exception);
        }

        [Fact]
        public void ValidateReadAccess_WithInvalidPath_ThrowsValidationException()
        {
            string nonExistentPath = Path.Combine(_testDirectory, "NonExistetntFolder");
            string expectedError = "No read access";

            var exception = Assert.Throws<ValidationException>(() => AccessValidator.ValidateReadAccess(nonExistentPath));

            Assert.Contains(expectedError, exception.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void ValidateReadAccess_WithInvalidInput_ThrowsValidationException(string path)
        {
            string expectedError = "No read access";

            var exception = Assert.Throws<ValidationException>(() => AccessValidator.ValidateReadAccess(path));

            Assert.Contains(expectedError, exception.Message);

        }

        [Fact]
        public void ValidateWriteAccess_WithValidPath_DoesNotThrowException()
        {
            var exception = Record.Exception(() => AccessValidator.ValidateWriteAccess(_testDirectory));

            Assert.Null(exception);
        }

        [Fact]
        public void ValidateWriteAccess_WithNonExistentPath_ThrowsValidationException()
        {
            string nonExistentPath = Path.Combine(_testDirectory, "NonExistetntFolder");
            string expectedError = "No write access";

            var exception = Assert.Throws<ValidationException>(() => AccessValidator.ValidateWriteAccess(nonExistentPath));

            Assert.Contains(expectedError, exception.Message);
        }

        [Fact]
        public void ValidateWriteAccess_WithReadOnlyDirectory_ThrowsValidationException()
        {
            string readOnlyDirectory = Path.Combine(_testDirectory, "ReadOnlyDirectory");
            _mockFileSystem.AddDirectory(readOnlyDirectory);
            _mockFileSystem.File.SetAttributes(readOnlyDirectory, FileAttributes.ReadOnly);
            string expectedError = "No write access";

            try
            {
                var exception = Assert.Throws<ValidationException>(() => AccessValidator.ValidateWriteAccess(readOnlyDirectory));

                Assert.Contains(expectedError, exception.Message);
            }
            finally
            {
                _mockFileSystem.File.SetAttributes(readOnlyDirectory, FileAttributes.Normal);
            }
        }
    }
}
