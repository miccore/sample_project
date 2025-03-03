using FluentAssertions;
using Miccore.Clean.Sample.Core.Helpers;

namespace Miccore.Clean.Sample.Core.Tests.Helpers
{
    public class DateHelperTests
    {
        [Fact]
        public void GetCurrentTimestamp_ShouldReturnCurrentUnixTimeSeconds()
        {
            // Arrange
            var expectedTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            // Act
            var actualTimestamp = DateHelper.GetCurrentTimestamp();

            // Assert
            actualTimestamp.Should().BeCloseTo(expectedTimestamp, 2, "the timestamps should be within 2 seconds of each other.");
        }
    }
}