using SourceHelpers = FolderSynchronization.Validators;

namespace FolderSynchronization.Tests.Validators
{
    public class SynchronizationIntervalValidatorTests
    {
        [Theory]
        [InlineData("1")]
        [InlineData("60")]
        [InlineData("3600")]
        [InlineData(" 60 ")]
        [InlineData("\t60")]
        [InlineData("60\n")]
        [InlineData("060")]
        [InlineData("00060")]
        public void ValidateSynchronizationInterval_WithValidInput_ReturnsInterval(string intervalString)
        {
            int result = SourceHelpers.SynchronizationIntervalValidator.ValidateSynchronizationInterval(intervalString);

            Assert.Equal(int.Parse(intervalString.Trim()), result);
        }

        [Fact]
        public void ValidateSynchronizationInterval_WithMaxIntValue_ReturnsInterval()
        {
            string intervalString = int.MaxValue.ToString();

            int result = SourceHelpers.SynchronizationIntervalValidator.ValidateSynchronizationInterval(intervalString);

            Assert.Equal(int.MaxValue, result);
        }

        [Theory]
        [InlineData("0")]
        [InlineData("-1")]
        [InlineData("-100")]
        public void ValidateSynchronizationInterval_WithNonPositiveInteger_ThrowsValidationError(string intervalString)
        {
            void TestAction() => SourceHelpers.SynchronizationIntervalValidator.ValidateSynchronizationInterval(intervalString);
            string expectedErrorMessage = "must be a positive integer";

            Exceptions.ValidationException exception = Assert.Throws<Exceptions.ValidationException>(TestAction);
            
            Assert.Contains(expectedErrorMessage, exception.Message);
        }

        [Theory]
        [InlineData("2147483648")] // int.Max + 1
        [InlineData("abc")]
        [InlineData("123abc")]
        [InlineData("3.14")]
        [InlineData("60.0")]
        [InlineData("\n")]
        [InlineData("\t")]
        [InlineData("\r")]
        [InlineData("\r\n")]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void ValidateSynchronizationInterval_WithInvalidInput_ReturnsFalse(string intervalString)
        {
            void TestAction() => SourceHelpers.SynchronizationIntervalValidator.ValidateSynchronizationInterval(intervalString);
            string expectedErrorMessage = "must be a valid integer";

            Exceptions.ValidationException exception = Assert.Throws<Exceptions.ValidationException>(TestAction);

            Assert.Contains(expectedErrorMessage, exception.Message);
        }
    }
}
