using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace Claims.Tests.PremiumCalculator
{
    [ExcludeFromCodeCoverage]
    public class YachtCalculatorTest
    {
        public readonly static TheoryData<DateOnly, DateOnly, decimal> PremiumInput = new()
        {
            { DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddDays(30)), 41250 },
            { DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddDays(61)), 81743.75m },
            { DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddDays(183)), 240982.5m }
        };

        [Theory, MemberData(nameof(PremiumInput))]
        public void YachtCalculator_ShouldReturnCorrectPremium(DateOnly startDate, DateOnly endDate, decimal expected)
        {
            // Arrange
            var calculator = new Services.PremiumCalculator.YachtCalculator();

            // Act
            decimal premiumAmount = calculator.CalculatePremiumForCoverType(startDate, endDate);

            // Assert
            Assert.True(premiumAmount > 0);
            Assert.Equal(expected, premiumAmount);
        }
    }
}
