namespace CarRental.Api.Interfaces;

using CarRental.Api.DTOs;

/// <summary>
/// Defines the contract for car rental search service.
/// </summary>
public interface ICarRentalService
{
    /// <summary>
    /// Searches for available vehicles across configured providers.
    /// </summary>
    /// <param name="request">The search request parameters.</param>
    /// <returns>Aggregated search results from all providers.</returns>
    Task<SearchResponseDto> SearchCarsAsync(SearchRequestDto request);
}
