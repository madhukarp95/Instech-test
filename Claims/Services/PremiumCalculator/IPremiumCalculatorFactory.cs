using Claims.Models.Cover;

namespace Claims.Services.PremiumCalculator
{
    public interface IPremiumCalculatorFactory
    {
        IPremiumCalculator GetCalculator(CoverType coverType);
    }
}
