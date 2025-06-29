using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace Claims.Tests.PremiumCalculator;

[ExcludeFromCodeCoverage]
public class DefaultCalculatorTest
{
    public readonly static TheoryData<DateOnly, DateOnly, decimal> PremiumInput = new()
    {
        { DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddDays(30)), 48750 },
        { DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddDays(61)), 98117.50M },
        { DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddDays(183)), 292353.75m }
    };

    [Theory, MemberData(nameof(PremiumInput))]
    public void DefaultCalculator_ShouldReturnCorrectPremium(DateOnly startDate, DateOnly endDate, decimal expected)
    {
        // Arrange
        var calculator = new Services.PremiumCalculator.DefaultCalculator();

        // Act
        decimal premiumAmount = calculator.CalculatePremiumForCoverType(startDate, endDate);

        // Assert
        Assert.True(premiumAmount > 0);
        Assert.Equal(expected, premiumAmount);
    }
}
