namespace Claims.Services.PremiumCalculator
{
    public class PassengerShipCalculator : BaseCalculator
    {
        public override decimal Multiplier => 1.20m;

        public override decimal CalculatePremiumForCoverType(DateOnly startDate, DateOnly endDate)
        {
            (int first30Days, int next150Days, int remaining) = GetDuration(startDate, endDate);

            decimal totalPremium = 0;

            // First 30 days
            totalPremium += first30Days * baseDayRate * Multiplier;

            totalPremium += next150Days * baseDayRate * Multiplier * 0.98m;

            totalPremium += remaining * baseDayRate * Multiplier * 0.97m;

            return totalPremium;
        }
    }
}
