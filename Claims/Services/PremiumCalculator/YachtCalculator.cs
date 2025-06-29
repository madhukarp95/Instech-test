namespace Claims.Services.PremiumCalculator
{
    public class YachtCalculator : BaseCalculator
    {
        public override decimal Multiplier => 1.10m;

        public override decimal CalculatePremiumForCoverType(DateOnly startDate, DateOnly endDate)
        {
            (int first30Days, int next150Days, int remaining) = GetDuration(startDate, endDate);

            decimal totalPremium = 0;

            // First 30 days
            totalPremium += first30Days * baseDayRate * Multiplier;

            // Next 150 days with 5% discount
            totalPremium += next150Days * baseDayRate * Multiplier * 0.95m;

            // Remaining days with Additional 8% discount
            totalPremium += remaining * baseDayRate * Multiplier * 0.92m;

            return totalPremium;
        }
    }
}
