using FolderSynchronization.Validators;
using FolderSynchronization.Exceptions;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;


namespace FolderSynchronization.Tests.Validators
{
    public class AccessValidatorTests
    {
        private MockFileSystem _mockFileSystem;
        private AccessValidator _accessValidator;
        private string _testDirectory;

        public AccessValidatorTests()
        {
            _mockFileSystem = new MockFileSystem();
            _accessValidator = new AccessValidator(_mockFileSystem);
            _testDirectory = Path.Combine(Path.GetTempPath(), $"AccessValidatorTests_{Guid.NewGuid()}");
            _mockFileSystem.AddDirectory(_testDirectory);
        }

        [Fact]
        public void ValidateReadAccess_WithValidPath_DoesNotThrowException()
        {
            // Arrange
            _mockFileSystem.AddFile(Path.Combine(_testDirectory, "test.txt"), new MockFileData(""));

            // Act
            var exception = Record.Exception(() => _accessValidator.ValidateReadAccess(_testDirectory));
           
            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void ValidateReadAccess_WithInvalidPath_ThrowsValidationException()
        {
            // Arrange
            string nonExistentPath = Path.Combine(_testDirectory, "NonExistetntFolder");
            string expectedError = "No read access";

            // Act
            var exception = Assert.Throws<ValidationException>(() => _accessValidator.ValidateReadAccess(nonExistentPath));

            // Assert
            Assert.Contains(expectedError, exception.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void ValidateReadAccess_WithInvalidInput_ThrowsValidationException(string path)
        {
            // Arrange
            string expectedError = "No read access";

            // Act
            var exception = Assert.Throws<ValidationException>(() => _accessValidator.ValidateReadAccess(path));

            // Assert
            Assert.Contains(expectedError, exception.Message);
        }

        [Fact]
        public void ValidateWriteAccess_WithValidPath_DoesNotThrowException()
        {
            // Act
            var exception = Record.Exception(() => _accessValidator.ValidateWriteAccess(_testDirectory));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void ValidateWriteAccess_WithNonExistentPath_ThrowsValidationException()
        {
            // Arrange
            string nonExistentPath = Path.Combine(_testDirectory, "NonExistentFolder");
            string expectedError = "No write access";

            // Act
            var exception = Assert.Throws<ValidationException>(() => _accessValidator.ValidateWriteAccess(nonExistentPath));

            // Assert
            Assert.Contains(expectedError, exception.Message);
        }

        [Fact]
        public void ValidateWriteAccess_WithReadOnlyDirectory_ThrowsValidationException()
        {
            // Arrange
            string readOnlyDirectory = Path.Combine(_testDirectory, "ReadOnlyDirectory");
            _mockFileSystem.AddDirectory(readOnlyDirectory);
            _mockFileSystem.File.SetAttributes(readOnlyDirectory, FileAttributes.ReadOnly);
            string expectedError = "No write access";

            // Act & assert
            try
            {
                var exception = Assert.Throws<ValidationException>(() => _accessValidator.ValidateWriteAccess(readOnlyDirectory));

                Assert.Contains(expectedError, exception.Message);
            }
            finally
            {
                _mockFileSystem.File.SetAttributes(readOnlyDirectory, FileAttributes.Normal);
            }
        }
    }
}
