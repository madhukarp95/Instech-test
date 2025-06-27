using Claims.Models;

namespace Claims.Domain;

public static class PremiumCalculator
{
    /// <summary>
    /// Calculates the premium based on the coverage period and cover type. 
    /// </summary>
    /// <param name="startDate">The start date of the coverage</param>
    /// <param name="endDate">The end date of the coverage</param>
    /// <param name="coverType">Type of Cover</param>
    /// <returns>The total premium amount based on duration and cover type</returns>
    public static decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
    {
        const decimal baseDayRate = 1250m;

        decimal multiplier = coverType switch
        {
            CoverType.Yacht => 1.10m,
            CoverType.PassengerShip => 1.20m,
            CoverType.Tanker => 1.50m,
            _ => 1.30m
        };

        int totalDays = (int)(endDate - startDate).TotalDays + 1;

        int first30Days = Math.Min(30, totalDays);
        int next150Days = Math.Min(150, Math.Max(0, totalDays - 30));
        int remaining = Math.Max(0, totalDays - 180);

        decimal totalPremium = 0;

        // First 30 days
        totalPremium += first30Days * baseDayRate * multiplier;

        // Next 150 days
        if (coverType == CoverType.Yacht)
            totalPremium += next150Days * baseDayRate * multiplier * 0.95m;
        else
            totalPremium += first30Days * baseDayRate * multiplier * 0.98m;
        // Remaining days
        if (coverType == CoverType.Yacht)
            totalPremium += remaining * baseDayRate * multiplier * 0.92m;
        else
            totalPremium += remaining * baseDayRate * multiplier * 0.97m;

        return totalPremium;
        
    }
}
