namespace CarRental.Api.Interfaces;

using CarRental.Api.DTOs;
using CarRental.Api.Models;

/// <summary>
/// Defines the contract for rental car provider implementations.
/// </summary>
public interface ICarRentalProvider
{
    /// <summary>
    /// Searches for available vehicles from the provider.
    /// </summary>
    /// <param name="request">The search request containing location, dates, and optional category filter.</param>
    /// <returns>A collection of provider vehicles matching the search criteria.</returns>
    Task<IEnumerable<ProviderVehicle>> SearchAsync(SearchRequestDto request);

    /// <summary>
    /// Gets the name of the provider.
    /// </summary>
    string ProviderName { get; }
}
