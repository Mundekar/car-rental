namespace CarRental.Api.Services;

using System.Security.Cryptography;
using System.Text;
using CarRental.Api.Common;
using CarRental.Api.DTOs;
using CarRental.Api.Interfaces;
using CarRental.Api.Models;
using CarRental.Api.Strategies;
using CarRental.Api.Validators;
using Microsoft.Extensions.Logging;

/// <summary>
/// Service for searching available rental cars across multiple providers.
/// Orchestrates provider queries, pricing calculations, result normalization, and sorting.
/// </summary>
public class CarRentalService : ICarRentalService
{
    private readonly IEnumerable<ICarRentalProvider> _providers;
    private readonly SearchRequestValidator _validator;
    private readonly IPricingStrategyRegistry _strategyRegistry;
    private readonly ILogger<CarRentalService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CarRentalService"/> class.
    /// </summary>
    /// <param name="providers">The collection of rental providers.</param>
    /// <param name="validator">The search request validator.</param>
    /// <param name="strategyRegistry">The registry for pricing strategies by provider type.</param>
    /// <param name="logger">The logger for recording search operations.</param>
    public CarRentalService(
        IEnumerable<ICarRentalProvider> providers,
        SearchRequestValidator validator,
        IPricingStrategyRegistry strategyRegistry,
        ILogger<CarRentalService> logger)
    {
        _providers = providers ?? throw new ArgumentNullException(nameof(providers));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _strategyRegistry = strategyRegistry ?? throw new ArgumentNullException(nameof(strategyRegistry));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Searches for available vehicles across configured providers.
    /// </summary>
    /// <param name="request">The search request parameters.</param>
    /// <returns>Aggregated and sorted search results from all providers.</returns>
    /// <exception cref="ArgumentNullException">Thrown when request is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when validation fails.</exception>
    public async Task<SearchResponseDto> SearchCarsAsync(SearchRequestDto request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        try
        {
            _logger.LogInformation("Search initiated: Pickup={Pickup} From={From} To={To}", 
                request.Pickup, request.From, request.To);

            // Validate request
            var validationErrors = _validator.Validate(request);
            if (validationErrors.Any())
            {
                _logger.LogWarning("Search validation failed: {Errors}", string.Join("; ", validationErrors));
                throw new InvalidOperationException(string.Join("; ", validationErrors));
            }

            var searchId = Guid.NewGuid();
            var allVehicles = new List<ProviderVehicle>();

            // Query all providers
            _logger.LogInformation("Querying {ProviderCount} providers", _providers.Count());
            var providerTasks = _providers.Select(provider => provider.SearchAsync(request));
            var providerResults = await Task.WhenAll(providerTasks);

            // Collect results from all providers
            foreach (var result in providerResults)
            {
                allVehicles.AddRange(result);
            }

            _logger.LogInformation("Received {TotalVehicles} vehicles from all providers", allVehicles.Count);

            // Filter out unavailable vehicles (BudgetWheels may return unavailable)
            var availableVehicles = allVehicles.Where(v => v.IsAvailable).ToList();
            
            _logger.LogInformation("Filtered to {AvailableVehicles} available vehicles", availableVehicles.Count);

            // Normalize and calculate pricing
            var normalizedVehicles = new List<ProviderVehicleDto>();
            var fromDate = DateOnly.FromDateTime(request.From);
            var toDate = DateOnly.FromDateTime(request.To);

            foreach (var vehicle in availableVehicles)
            {
                var pricing = _strategyRegistry.GetStrategy(vehicle.ProviderType);
                var totalPrice = pricing.CalculateTotalPrice(vehicle.DailyRate, fromDate, toDate);

                var dto = new ProviderVehicleDto
                {
                    VehicleId = DeterministicVehicleId(vehicle.ProviderType.ToString(), vehicle.ProviderVehicleId),
                    Provider = vehicle.ProviderType.ToString(),
                    Category = vehicle.Category,
                    Make = vehicle.Make,
                    Model = vehicle.Model,
                    DailyRate = vehicle.DailyRate,
                    TotalPrice = totalPrice,
                    InsuranceType = vehicle.InsuranceType,
                    CancellationPolicy = vehicle.CancellationPolicy,
                    IsAvailable = vehicle.IsAvailable
                };

                normalizedVehicles.Add(dto);
            }

            // Sort by total price (ascending)
            var sortedVehicles = normalizedVehicles
                .OrderBy(v => v.TotalPrice)
                .ToList();

            // Create response
            var response = new SearchResponseDto
            {
                SearchId = searchId,
                PickupLocation = request.Pickup,
                FromDate = request.From,
                ToDate = request.To,
                DaysCount = (int)(toDate.DayNumber - fromDate.DayNumber),
                Results = sortedVehicles
            };

            _logger.LogInformation("Search completed successfully: SearchId={SearchId} ResultCount={ResultCount}", 
                searchId, response.Results.Count);

            return response;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Search validation error");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during search");
            throw;
        }
    }

    /// <summary>
    /// Derives a stable, deterministic <see cref="Guid"/> from a provider name and its internal vehicle ID.
    /// The same inputs always produce the same output, ensuring cross-request consistency.
    /// </summary>
    private static Guid DeterministicVehicleId(string providerName, string providerVehicleId)
    {
        var input = $"{providerName}:{providerVehicleId}";
        var hash = MD5.HashData(Encoding.UTF8.GetBytes(input));
        return new Guid(hash);
    }
}
