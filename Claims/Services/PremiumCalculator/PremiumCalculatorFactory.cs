using Claims.Models;

namespace Claims.Services.PremiumCalculator
{
    public class PremiumCalculatorFactory : IPremiumCalculatorFactory
    {
        private readonly IServiceProvider _provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="PremiumCalculatorFactory"/> class.
        /// </summary>
        /// <param name="provider"></param>
        public PremiumCalculatorFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        /// <summary>
        /// Gets the appropriate premium calculator based on the cover type.
        /// </summary>
        /// <param name="coverType"></param>
        /// <returns></returns>
        public IPremiumCalculator GetCalculator(CoverType coverType)
        {
            return coverType switch
            {
                CoverType.Yacht => _provider.GetRequiredService<YachtCalculator>(),
                CoverType.PassengerShip => _provider.GetRequiredService<PassengerShipCalculator>(),
                CoverType.Tanker => _provider.GetRequiredService<TankerCalculator>(),
                _ => _provider.GetRequiredService<DefaultCalculator>()
            };
        }
    }
}
