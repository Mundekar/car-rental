namespace CarRental.Api.Interfaces;

/// <summary>
/// Defines the contract for provider-specific pricing calculation strategies.
/// </summary>
public interface IPricingStrategy
{
    /// <summary>
    /// Calculates the total rental price for the given period.
    /// </summary>
    /// <param name="dailyRate">The base daily rate for the vehicle.</param>
    /// <param name="from">The rental start date (inclusive).</param>
    /// <param name="to">The rental end date (exclusive).</param>
    /// <returns>The total rental price for the period.</returns>
    decimal CalculateTotalPrice(decimal dailyRate, DateOnly from, DateOnly to);
}
