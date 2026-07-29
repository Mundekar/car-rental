namespace CarRental.Api.Interfaces;

/// <summary>
/// Defines the contract for provider-specific pricing calculation strategies.
/// </summary>
public interface IPricingStrategy
{
    /// <summary>
    /// Calculates the total rental price for the given parameters.
    /// </summary>
    /// <param name="dailyRate">The base daily rate.</param>
    /// <param name="fromDate">The rental start date.</param>
    /// <param name="toDate">The rental end date.</param>
    /// <returns>The calculated total price.</returns>
    decimal CalculateTotalPrice(decimal dailyRate, DateTime fromDate, DateTime toDate);

    /// <summary>
    /// Gets a day-by-day or night-by-night price breakdown.
    /// </summary>
    /// <param name="dailyRate">The base daily rate.</param>
    /// <param name="fromDate">The rental start date.</param>
    /// <param name="toDate">The rental end date.</param>
    /// <returns>A dictionary mapping date strings to prices.</returns>
    Dictionary<string, decimal> GetPriceBreakdown(decimal dailyRate, DateTime fromDate, DateTime toDate);
}
