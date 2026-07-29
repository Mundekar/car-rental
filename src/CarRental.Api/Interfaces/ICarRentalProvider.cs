namespace CarRental.Api.Interfaces;

using CarRental.Api.Models;

/// <summary>
/// Defines the contract for rental car provider implementations.
/// </summary>
public interface ICarRentalProvider
{
    /// <summary>
    /// Searches for available vehicles from the provider.
    /// </summary>
    /// <param name="pickup">The pickup location.</param>
    /// <param name="from">The rental start date.</param>
    /// <param name="to">The rental end date.</param>
    /// <param name="category">The optional vehicle category filter.</param>
    /// <returns>A collection of available provider vehicles.</returns>
    Task<IEnumerable<ProviderVehicle>> SearchAvailableVehiclesAsync(
        string pickup,
        DateTime from,
        DateTime to,
        string? category = null);

    /// <summary>
    /// Retrieves detailed information about a specific vehicle.
    /// </summary>
    /// <param name="vehicleId">The provider's vehicle identifier.</param>
    /// <returns>The vehicle details.</returns>
    Task<ProviderVehicle?> GetVehicleDetailsAsync(string vehicleId);

    /// <summary>
    /// Gets the name of the provider.
    /// </summary>
    string ProviderName { get; }
}
