namespace CarRental.Api.Strategies;

using CarRental.Api.Interfaces;

/// <summary>
/// Pricing strategy for PremiumDrive provider.
/// Implements flat daily rate pricing with no surcharges.
/// </summary>
public class PremiumDrivePricingStrategy : IPricingStrategy
{
    /// <summary>
    /// Calculates total price using flat daily rate.
    /// Formula: totalPrice = dailyRate × numberOfNights
    /// </summary>
    /// <param name="dailyRate">The flat daily rate.</param>
    /// <param name="from">The rental start date.</param>
    /// <param name="to">The rental end date.</param>
    /// <returns>Total price for the rental period.</returns>
    public decimal CalculateTotalPrice(decimal dailyRate, DateOnly from, DateOnly to)
    {
        var nights = (int)(to.DayNumber - from.DayNumber);
        if (nights <= 0)
        {
            nights = 1;
        }

        return dailyRate * nights;
    }
}
