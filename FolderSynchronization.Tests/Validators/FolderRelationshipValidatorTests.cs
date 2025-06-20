using FolderSynchronization.Exceptions;
using FolderSynchronization.Validators;

namespace FolderSynchronization.Tests.Validators
{
    public class FolderRelationshipValidatorTests
    {
        [Fact]
        public void ValidateSourceAndDestinationRelationship_WhenValidPaths_DoesNotThrowException()
        {
            // Arrange
            string sourcePath = @"C:\SourceFolder";
            string destinationPath = @"C:\DestinationFolder\SourceFolder_copy";
            void TestAction() => FolderRelationshipValidator.ValidateSourceAndDestinationRelationship(sourcePath, destinationPath);

            // Act
            var exception = Record.Exception(TestAction);

            // Assert
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(@"C:\TestFolder\TestFolder_copy")]
        [InlineData(@"C:\testfolder\testfolder_copy")]
        public void ValidateSourceAndDestinationRelationship_WhenSourceAndDestinationAreSame_ThrowsValidationException(string destinationPath)
        {
            // Arrange
            string sourcePath = @"C:\TestFolder";
            string expectedError = "Source and Destination folders cannot be the same";
            void TestAction () => FolderRelationshipValidator.ValidateSourceAndDestinationRelationship (sourcePath, destinationPath);

            // Act
            var exception = Assert.Throws<ValidationException>(TestAction);

            // Assert
            Assert.Contains(expectedError, exception.Message);
        }

        [Theory]
        [InlineData(@"C:\TestFolder\subFolder\TestFolder_copy")]
        [InlineData(@"C:\TestFolder\TestFolder_copy\TestFolder_copy")]
        public void ValidateSourceAndDestinationRelationship_WhenDestinationIsNestedInSource_ThrowsValidationException(string destinationPath)
        {
            // Arrange
            string sourcePath = @"C:\TestFolder";
            string expectedError = "Destination folder cannot be inside Source folder";
            void TestAction() => FolderRelationshipValidator.ValidateSourceAndDestinationRelationship(sourcePath, destinationPath);

            // Act
            var exception = Assert.Throws<ValidationException>(TestAction);

            // Assert
            Assert.Contains(expectedError, exception.Message);
        }

        [Theory]
        [InlineData(@"C:\TestFolder_copy\TestFolder", @"C:\TestFolder_copy")]
        [InlineData(@"C:\TestFolder_copy\subFolder\TestFolder", @"C:\TestFolder_copy")]
        [InlineData(@"C:\Folder\TestFolder_copy\TestFolder", @"C:\Folder\TestFolder_copy")]
        public void ValidateSourceAndDestinationRelationship_WhenSourceIsNestedInDestination_ThrowsValidationException(string sourcePath, string destinationPath)
        {
            // Arrange
            string expectedError = "Source folder cannot be inside Destination folder";
            void TestAction() => FolderRelationshipValidator.ValidateSourceAndDestinationRelationship(sourcePath, destinationPath);

            // Act
            var exception = Assert.Throws<ValidationException>(TestAction);

            // Assert
            Assert.Contains(expectedError, exception.Message);
        }

        [Theory]
        [InlineData(@"C:\TestFolder\", @"C:\Destination\TestFolder_copy\")]
        [InlineData(@"C:\TestFolder", @"C:\Destination\TestFolder_copy\")]
        [InlineData(@"C:\TestFolder\", @"C:\Destination\TestFolder_copy")]
        public void ValidateSourceAndDestinationRelationship_WithTrailingSlashes_DoesNotThrowException(string sourcePath, string destinationPath)
        {
            // Arrange
            void TestAction() => FolderRelationshipValidator.ValidateSourceAndDestinationRelationship(sourcePath, destinationPath);
            
            // Act
            var exception = Record.Exception(TestAction);
            
            // Assert
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(@".\TestFolder", @"C:\Destination\TestFolder_copy")]
        [InlineData(@"..\TestFolder", @"C:\Destination\TestFolder_copy")]
        public void ValidateSourceAndDestinationRelationship_WithRelativePaths_DoesNotThrowException(string sourcePath, string destinationPath)
        {
            // Arrange
            void TestAction() => FolderRelationshipValidator.ValidateSourceAndDestinationRelationship(sourcePath, destinationPath);
            
            // Act
            var exception = Record.Exception(TestAction);
            
            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void ValidateLogFolderRelationship_WhenValidPaths_DoesNotThrowException()
        {
            // Arrange
            string logPath = @"C:\Logs";
            string sourcePath = @"C:\SourceFolder";
            string destinationPath = @"C:\DestinationFolder\SourceFolder_copy";
            void TestAction() => FolderRelationshipValidator.ValidateLogFolderRelationship(logPath, sourcePath, destinationPath);

            // Act
            var exception = Record.Exception(TestAction);

            // Assert
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(@"C:\SourceFolder")]
        [InlineData(@"C:\SourceFolder\Logs")]
        public void ValidateLogFolderRelationship_WhenLogIsInSource_ThrowsValidationException(string logPath)
        {
            // Arrange
            string sourcePath = @"C:\SourceFolder";
            string destinationPath = @"C:\DestinationFolder\SourceFolder_copy";
            string expectedError = "Log folder cannot be inside Source folder";
            void TestAction() => FolderRelationshipValidator.ValidateLogFolderRelationship(logPath, sourcePath, destinationPath);

            // Act
            var exception = Assert.Throws<ValidationException>(TestAction);

            // Assert
            Assert.Contains(expectedError, exception.Message);
        }

        [Theory]
        [InlineData(@"C:\DestinationFolder\SourceFolder_copy")]
        [InlineData(@"C:\DestinationFolder\SourceFolder_copy\Logs")]
        public void ValidateLogFolderRelationship_WhenLogIsInDestination_ThrowsValidationException(string logPath)
        {
            // Arrange
            string sourcePath = @"C:\SourceFolder";
            string destinationPath = @"C:\DestinationFolder\SourceFolder_copy";
            string expectedError = "Log folder cannot be inside Destination folder";
            void TestAction() => FolderRelationshipValidator.ValidateLogFolderRelationship(logPath, sourcePath, destinationPath);

            // Act
            var exception = Assert.Throws<ValidationException>(TestAction);

            // Assert
            Assert.Contains(expectedError, exception.Message);
        }
    }
}
