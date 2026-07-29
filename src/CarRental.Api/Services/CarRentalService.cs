namespace CarRental.Api.Services;

using CarRental.Api.DTOs;
using CarRental.Api.Interfaces;

/// <summary>
/// Service for searching available rental cars across providers.
/// </summary>
public class CarRentalService : ICarRentalService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CarRentalService"/> class.
    /// </summary>
    public CarRentalService()
    {
    }

    /// <summary>
    /// Searches for available vehicles across configured providers.
    /// </summary>
    /// <param name="request">The search request parameters.</param>
    /// <returns>Aggregated search results from all providers.</returns>
    public Task<SearchResponseDto> SearchCarsAsync(SearchRequestDto request)
    {
        throw new NotImplementedException();
    }
}
