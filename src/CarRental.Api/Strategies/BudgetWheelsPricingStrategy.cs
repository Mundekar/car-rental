namespace CarRental.Api.Strategies;

using CarRental.Api.Interfaces;

/// <summary>
/// Pricing strategy for BudgetWheels provider.
/// Implements base rate with 20% surcharge on Friday, Saturday, and Sunday nights.
/// </summary>
public class BudgetWheelsPricingStrategy : IPricingStrategy
{
    /// <summary>
    /// Calculates total price with weekend surcharge.
    /// Weekend nights (Fri, Sat, Sun) receive 20% surcharge.
    /// Calculates price night-by-night, not as daily rate × days.
    /// </summary>
    /// <param name="dailyRate">The base daily rate.</param>
    /// <param name="from">The rental start date.</param>
    /// <param name="to">The rental end date.</param>
    /// <returns>Total price including weekend surcharges.</returns>
    public decimal CalculateTotalPrice(decimal dailyRate, DateOnly from, DateOnly to)
    {
        var totalPrice = 0m;
        var currentDate = from;

        while (currentDate < to)
        {
            var dayOfWeek = currentDate.DayOfWeek;
            var isWeekendNight = dayOfWeek == DayOfWeek.Friday ||
                                dayOfWeek == DayOfWeek.Saturday ||
                                dayOfWeek == DayOfWeek.Sunday;

            if (isWeekendNight)
            {
                totalPrice += dailyRate * 1.2m; // 20% surcharge
            }
            else
            {
                totalPrice += dailyRate;
            }

            currentDate = currentDate.AddDays(1);
        }

        return totalPrice;
    }
}
