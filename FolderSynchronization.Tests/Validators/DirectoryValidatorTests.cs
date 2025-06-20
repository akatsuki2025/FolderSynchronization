using FolderSynchronization.Exceptions;
using FolderSynchronization.Validators;
using System.IO.Abstractions.TestingHelpers;

namespace FolderSynchronization.Tests.Validators
{
    public class DirectoryValidatorTests
    {
        private const string SourceDirectory = @"C:\Source";
        private const string DestinationDirectory = @"C:\Destination\Source_copy";
        private const string LogDirectory = @"C:\Logs";

        [Fact]
        public void ValidateSourceDirectory_WithValidSource_DoesNotThrowException()
        {
            // Arrange
            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddDirectory(SourceDirectory);
            var accessValidator = new AccessValidator(mockFileSystem);

            // Act
            var exception = Record.Exception(() => DirectoryValidator.ValidateSourceDirectory(SourceDirectory, accessValidator));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void ValidateSourceDirectory_WithInvalidSource_ThrowsValidationException()
        {
            // Arrange
            var mockFileSystem = new MockFileSystem();
            var accessValidator = new AccessValidator(mockFileSystem);
            string invalidSource = @"C:\NonExistent";
            string expectedError = "No read access";

            // Act
            var exception = Assert.Throws<ValidationException>(() => DirectoryValidator.ValidateSourceDirectory(invalidSource, accessValidator));

            // Assert
            Assert.Contains(expectedError, exception.Message);
        }

        [Fact]
        public void ValidateDestinationDirectory_WithValidDestination_DoesNotThrowException()
        {
            // Arrange
            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddDirectory(SourceDirectory);
            mockFileSystem.AddDirectory(DestinationDirectory);
            var accessValidator = new AccessValidator(mockFileSystem);

            // Act
            var exception = Record.Exception(() => DirectoryValidator.ValidateDestinationDirectory(DestinationDirectory, SourceDirectory, accessValidator));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void ValidateDestinationDirectory_WithInvalidParentDirectory_ThrowsValidationException()
        {
            // Arrange
            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddDirectory(SourceDirectory);
            var accessValidator = new AccessValidator(mockFileSystem);
            string invalidDestination = @"C:\NonExistent\Source_copy";
            string expectedError = "No read access";

            // Act
            var exception = Assert.Throws<ValidationException>(() => DirectoryValidator.ValidateDestinationDirectory(invalidDestination, SourceDirectory, accessValidator));

            // Assert
            Assert.Contains(expectedError, exception.Message);
        }

        [Fact]
        public void ValidateLogDirectory_WithExistingLogDirectory_DoesNotThrowException()
        {
            // Arrange
            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddDirectory(LogDirectory);
            var accessValidator = new AccessValidator(mockFileSystem);

            // Act
            var exception = Record.Exception(() => DirectoryValidator.ValidateLogDirectory(LogDirectory, accessValidator, mockFileSystem));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void ValidateLogDirectory_WithNonExistentLogDirectory_CreatesDirectory()
        {
            // Arrange
            var mockFileSystem = new MockFileSystem();
            var accessValidator = new AccessValidator(mockFileSystem);
            string newLogDirectory = @"C:\NewLogs";

            // Act
            var exception = Record.Exception(() => DirectoryValidator.ValidateLogDirectory(newLogDirectory, accessValidator, mockFileSystem));

            // Assert
            Assert.Null(exception);
            Assert.True(mockFileSystem.Directory.Exists(newLogDirectory));
        }

        [Fact]
        public void ValidateLogDirectory_WithReadonlyLogDirectory_ThrowsValidationException()
        {
            // Arrange
            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddDirectory(LogDirectory);
            mockFileSystem.File.SetAttributes(LogDirectory, FileAttributes.ReadOnly);
            var accessValidator = new AccessValidator(mockFileSystem);

            // Act
            var exception = Assert.Throws<ValidationException>(() => DirectoryValidator.ValidateLogDirectory(LogDirectory, accessValidator, mockFileSystem));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ValidationException>(exception);
        }
    }
}
