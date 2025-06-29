namespace Claims.Services.PremiumCalculator;

public interface IPremiumCalculator
{
    /// <summary>
    /// Multiplier for the premium calculation.
    /// </summary>
    public decimal Multiplier { get; }

    /// <summary>
    /// Calculates the premium for a specific cover type based on the provided start and end dates.
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    decimal CalculatePremiumForCoverType(DateOnly startDate, DateOnly endDate);
}
