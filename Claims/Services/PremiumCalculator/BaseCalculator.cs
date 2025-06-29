namespace Claims.Services.PremiumCalculator;

public abstract class BaseCalculator : IPremiumCalculator
{
    public const decimal baseDayRate = 1250m;

    // </inheritdoc>
    public virtual decimal Multiplier => 1.3m;

    // </inheritdoc>
    public abstract decimal CalculatePremiumForCoverType(DateOnly startDate, DateOnly endDate);

    private protected static (int first30Days, int next150Days, int remainingDays) GetDuration(DateOnly startDate, DateOnly endDate)
    {
        int totalDays = endDate.DayNumber - startDate.DayNumber;

        int first30Days = Math.Min(30, totalDays);
        int next150Days = Math.Min(150, Math.Max(0, totalDays - 30));
        int remaining = Math.Max(0, totalDays - 180);

        return (first30Days, next150Days, remaining);
    }
}
