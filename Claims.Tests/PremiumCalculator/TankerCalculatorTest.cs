using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace Claims.Tests.PremiumCalculator
{
    [ExcludeFromCodeCoverage]
    public class TankerCalculatorTest
    {
        public readonly static TheoryData<DateOnly, DateOnly, decimal> PremiumInput = new()
        {
            { DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddDays(30)), 56250 },
            { DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddDays(61)), 113212.50m },
            { DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddDays(183)), 337331.25m }
        };

        [Theory, MemberData(nameof(PremiumInput))]
        public void TankerCalculator_ShouldReturnCorrectPremium(DateOnly startDate, DateOnly endDate, decimal expected)
        {
            // Arrange
            var calculator = new Services.PremiumCalculator.TankerCalculator();

            // Act
            decimal premiumAmount = calculator.CalculatePremiumForCoverType(startDate, endDate);

            // Assert
            Assert.True(premiumAmount > 0);
            Assert.Equal(expected, premiumAmount);
        }
    }
}
