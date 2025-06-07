using SourceHelpers = FolderSynchronization.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchronization.Tests.Validators
{
    public class ValidateSynchronizationIntervalTests
    {
        [Theory]
        [InlineData("1", 1)]
        [InlineData("60", 60)]
        [InlineData("3600", 3600)]
        public void ValidateSynchronizationInterval_WithValidInput_ReturnsTrue(string intervalString, int expectedIntervalInSeconds)
        {
            bool result = SourceHelpers.SynchronizationIntervalValidator.ValidateSynchronizationInterval(intervalString, out int actualIntervalInSeconds);

            Assert.True(result);
            Assert.Equal(expectedIntervalInSeconds, actualIntervalInSeconds);
        }

        [Fact]
        public void ValidateSynchronizationInterval_WithMaxIntValue_ReturnsTrue()
        {
            int expectedIntervalInSeconds = int.MaxValue;
            string intervalString = expectedIntervalInSeconds.ToString();

            bool result = SourceHelpers.SynchronizationIntervalValidator.ValidateSynchronizationInterval(intervalString, out int actualIntervalInSeconds);

            Assert.True(result);
            Assert.Equal(expectedIntervalInSeconds, actualIntervalInSeconds);
        }

        [Fact]
        public void ValidateSynchronizationInterval_ExceedingMaxIntValue_ReturnsFalse()
        {
            string intervalString = "2147483648";

            bool result = SourceHelpers.SynchronizationIntervalValidator.ValidateSynchronizationInterval(intervalString, out int actualIntervalInSeconds);

            Assert.False(result);
            Assert.Equal(0, actualIntervalInSeconds);
        }

        [Fact]
        public void ValidateSynchronizationInterval_WithZero_ReturnsFalse()
        {
            int expectedIntervalInSeconds = 0;
            string intervalString = expectedIntervalInSeconds.ToString();

            bool result = SourceHelpers.SynchronizationIntervalValidator.ValidateSynchronizationInterval(intervalString, out int actualIntervalInSeconds);

            Assert.False(result);
            Assert.Equal(0, actualIntervalInSeconds);

        }

        [Fact]
        public void ValidateSynchronizationInterval_WithNull_ReturnsFalse()
        {
            bool result = SourceHelpers.SynchronizationIntervalValidator.ValidateSynchronizationInterval(null, out int actualIntervalInSeconds);

            Assert.False(result);
            Assert.Equal(0, actualIntervalInSeconds);
        }

        [Theory]
        [InlineData("-1")]
        [InlineData("-100")]
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
        public void ValidateSynchronizationInterval_WithInvalidInput_ReturnsFalse(string intervalString)
        {
            bool result = SourceHelpers.SynchronizationIntervalValidator.ValidateSynchronizationInterval(intervalString, out _);

            Assert.False(result);
        }

        [Fact]
        public void ValidateSynchronizationInterval_WithInvalidInput_SetOutputToZero()
        {
            string intervalString = "invalid";

            SourceHelpers.SynchronizationIntervalValidator.ValidateSynchronizationInterval(intervalString, out int actualIntervalInSeconds);

            Assert.Equal(0, actualIntervalInSeconds);
        }

        [Theory]
        [InlineData(" 60 ", 60)]
        [InlineData("\t60", 60)]
        [InlineData("60\n", 60)]
        [InlineData("060", 60)]
        [InlineData("00060", 60)]
        public void ValidateSynchronizationInterval_WithPaddedValidInput_ReturnsTrue(string intervalString, int expectedIntervalInSeconds)
        {
            bool result = SourceHelpers.SynchronizationIntervalValidator.ValidateSynchronizationInterval(intervalString, out int actualIntervalInSeconds);

            Assert.True(result);
            Assert.Equal(expectedIntervalInSeconds, actualIntervalInSeconds);
        }
    }
}
