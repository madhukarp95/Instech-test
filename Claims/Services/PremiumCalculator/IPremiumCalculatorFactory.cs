using Claims.Models;

namespace Claims.Services.PremiumCalculator
{
    public interface IPremiumCalculatorFactory
    {
        IPremiumCalculator GetCalculator(CoverType coverType);
    }
}
