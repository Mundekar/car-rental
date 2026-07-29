namespace CarRental.Api.Providers;

using CarRental.Api.Interfaces;
using CarRental.Api.Models;

/// <summary>
/// Provider implementation for BudgetWheels rental service.
/// </summary>
public class BudgetWheelsProvider : ICarRentalProvider
{
    /// <summary>
    /// Gets the name of the provider.
    /// </summary>
    public string ProviderName => "BudgetWheels";

    /// <summary>
    /// Initializes a new instance of the <see cref="BudgetWheelsProvider"/> class.
    /// </summary>
    public BudgetWheelsProvider()
    {
    }

    /// <summary>
    /// Searches for available vehicles from BudgetWheels.
    /// </summary>
    /// <param name="pickup">The pickup location.</param>
    /// <param name="from">The rental start date.</param>
    /// <param name="to">The rental end date.</param>
    /// <param name="category">The optional vehicle category filter.</param>
    /// <returns>A collection of available provider vehicles.</returns>
    public Task<IEnumerable<ProviderVehicle>> SearchAvailableVehiclesAsync(
        string pickup,
        DateTime from,
        DateTime to,
        string? category = null)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Retrieves detailed information about a specific vehicle from BudgetWheels.
    /// </summary>
    /// <param name="vehicleId">The provider's vehicle identifier.</param>
    /// <returns>The vehicle details.</returns>
    public Task<ProviderVehicle?> GetVehicleDetailsAsync(string vehicleId)
    {
        throw new NotImplementedException();
    }
}
