using SourceHelpers = FolderSynchronization.Validators;
using FolderSynchronization.Exceptions;

namespace FolderSynchronization.Tests.Validators
{
    public class PathValidatorTests
    {
        [Theory]
        [InlineData("C:\\folder")]
        [InlineData("C:\\folder\\subfolder")]
        [InlineData(" C:\\folder ")]
        [InlineData("..\\relative\\path")]
        [InlineData(".\\current\\path")]
        public void ValidatePath_WithValidPath_ReturnsFullPath(string path)
        {
            string result = SourceHelpers.PathValidator.ValidatePath(path, "Test path", false);

            Assert.NotNull(result);
            Assert.Equal(Path.GetFullPath(path.Trim()), result);
        }

        [Theory]
        [InlineData("C:\\")]
        [InlineData("C:/")]
        public void ValidatePath_WithDriveRoot_WhenNotAllowed_ThrowsValidationException(string path)
        {
            void TestAction() => SourceHelpers.PathValidator.ValidatePath(path, "Test path", false);
            string expectedError = "cannot be a drive root";

            var exception = Assert.Throws<ValidationException>(TestAction);

            Assert.Contains(expectedError, exception.Message);
        }

        [Theory]
        [InlineData("C:\\", "C:\\")]
        [InlineData("C:/", "C:\\")]
        public void ValidatePath_WithDriveRoot_WhenAllowed_ReturnsFullPath(string inputPath, string expectedPath)
        {
            string result = SourceHelpers.PathValidator.ValidatePath(inputPath, "Test path", true);

            Assert.Equal(expectedPath, result);
        }

        [Theory]
        [InlineData("C:\\folder//subfolder")]
        [InlineData("C:/folder/subfolder")]
        [InlineData("C://folder//subfolder")]
        [InlineData("C:\\folder\\\\subfolder")]
        public void ValidatePath_WithMixedSlashes_ReturnsFullPath(string path)
        {
            string expectedPath = Path.GetFullPath(path);

            string result = SourceHelpers.PathValidator.ValidatePath(path, "Test path", false);

            Assert.Equal(expectedPath, result);
            Assert.Contains(Path.DirectorySeparatorChar, result);
            Assert.DoesNotContain("/", result);
            Assert.DoesNotContain("\\\\", result);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("\t")]
        [InlineData("\n")]
        [InlineData("\r")]
        [InlineData(null)]
        public void ValidatePath_WithEmptyOrWhitespace_ThrowsValidationException(string path)
        {
            void TestAction() => SourceHelpers.PathValidator.ValidatePath(path, "Test path", false);
            string expectedError = "cannot be empty or whitespace";

            var exception = Assert.Throws<ValidationException>(TestAction);

            Assert.Contains(expectedError, exception.Message);
        }

        [Theory]
        [InlineData("C:")]
        [InlineData("C")]
        [InlineData(" D: ")]
        [InlineData(" d ")]
        public void ValidatePath_WithDriveLetter_ThrowsValidationException(string path)
        {
            void TestAction() => SourceHelpers.PathValidator.ValidatePath(path, "Test path", false);
            string expectedError = "cannot be just a drive letter";

            var exception = Assert.Throws<ValidationException>(TestAction);

            Assert.Contains(expectedError, exception.Message);
        }

        [Theory]
        [InlineData("C:\\folder<\\subfolder")]
        [InlineData("C:\\folder>\\subfolder")]
        [InlineData("C:\\folder|\\subfolder")]
        [InlineData("C:\\folder\"\\subfolder")]
        [InlineData("C:\\folder?\\subfolder")]
        [InlineData("C:\\folder*\\subfolder")]
        public void ValidatePath_WithInvalidCharacters_ThrowsValidationException(string path)
        {
            void TestAction() => SourceHelpers.PathValidator.ValidatePath(path, "Test path", false);

            var exception = Assert.Throws<ValidationException>(TestAction);

            Assert.Contains("Invalid Test path", exception.Message);
        }

        [Theory]
        [InlineData("\\\\")]  
        [InlineData("C:\\invalid\\path\\\0")]  
        public void ValidatePath_WithInvalidPathFormat_ThrowsValidationException(string path)
        {
            void TestAction() => SourceHelpers.PathValidator.ValidatePath(path, "Test path", false);

            var exception = Assert.Throws<ValidationException>(TestAction);
            Assert.Contains("Invalid Test path", exception.Message);
        }
    }
}
