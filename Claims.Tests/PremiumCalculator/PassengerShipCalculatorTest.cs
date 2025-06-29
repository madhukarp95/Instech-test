using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace Claims.Tests.PremiumCalculator
{
    [ExcludeFromCodeCoverage]
    public class PassengerShipCalculatorTest
    {
        public readonly static TheoryData<DateOnly, DateOnly, decimal> PremiumInput = new()
        {
            { DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddDays(30)), 45000 },
            { DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddDays(61)), 90570 },
            { DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddDays(183)), 269865 }
        };

        [Theory, MemberData(nameof(PremiumInput))]
        public void PassengerShipCalculator_ShouldReturnCorrectPremium(DateOnly startDate, DateOnly endDate, decimal expected)
        {
            // Arrange
            var calculator = new Services.PremiumCalculator.PassengerShipCalculator();

            // Act
            decimal premiumAmount = calculator.CalculatePremiumForCoverType(startDate, endDate);

            // Assert
            Assert.True(premiumAmount > 0);
            Assert.Equal(expected, premiumAmount);
        }
    }
}
