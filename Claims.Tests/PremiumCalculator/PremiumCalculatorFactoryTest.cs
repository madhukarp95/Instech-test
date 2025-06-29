using Claims.Models.Cover;
using Claims.Services.PremiumCalculator;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Claims.Tests.PremiumCalculator;

[ExcludeFromCodeCoverage]
public class PremiumCalculatorFactoryTest
{
    private readonly PremiumCalculatorFactory _premiumCalculatorFactory;
    private readonly IServiceProvider _serviceProvider;

    public PremiumCalculatorFactoryTest()
    {
        var services = new ServiceCollection();
        services.AddTransient<YachtCalculator>();
        services.AddTransient<PassengerShipCalculator>();
        services.AddTransient<TankerCalculator>();
        services.AddTransient<DefaultCalculator>();
        _serviceProvider = services.BuildServiceProvider();

        _premiumCalculatorFactory = new PremiumCalculatorFactory(_serviceProvider);
    }

    [Theory]
    [InlineData(CoverType.Yacht, typeof(YachtCalculator))]
    [InlineData(CoverType.PassengerShip, typeof(PassengerShipCalculator))]
    [InlineData(CoverType.Tanker, typeof(TankerCalculator))]
    [InlineData(CoverType.ContainerShip, typeof(DefaultCalculator))]
    [InlineData(CoverType.BulkCarrier, typeof(DefaultCalculator))]
    public void GetCalculator_ShouldReturnCorrectType(CoverType coverType, Type expectedType)
    {
        var calculator = _premiumCalculatorFactory.GetCalculator(coverType);
        Assert.IsType(expectedType, calculator);
    }

    [Theory]
    [InlineData(CoverType.Yacht, 1.10)]
    [InlineData(CoverType.PassengerShip, 1.20)]
    [InlineData(CoverType.Tanker, 1.50)]
    [InlineData(CoverType.ContainerShip, 1.3)] // DefaultCalculator
    public void Calculator_ShouldReturnCorrectMultiplier(CoverType coverType, decimal expectedMultiplier)
    {
        var calculator = _premiumCalculatorFactory.GetCalculator(coverType);
        Assert.Equal(expectedMultiplier, calculator.Multiplier);
    }
}
