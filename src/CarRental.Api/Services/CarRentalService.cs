namespace CarRental.Api.Services;

using CarRental.Api.Common;
using CarRental.Api.DTOs;
using CarRental.Api.Interfaces;
using CarRental.Api.Models;
using CarRental.Api.Strategies;
using CarRental.Api.Validators;

/// <summary>
/// Service for searching available rental cars across multiple providers.
/// Orchestrates provider queries, pricing calculations, result normalization, and sorting.
/// </summary>
public class CarRentalService : ICarRentalService
{
    private readonly IEnumerable<ICarRentalProvider> _providers;
    private readonly SearchRequestValidator _validator;
    private readonly PremiumDrivePricingStrategy _premiumDrivePricingStrategy;
    private readonly BudgetWheelsPricingStrategy _budgetWheelsPricingStrategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="CarRentalService"/> class.
    /// </summary>
    /// <param name="providers">The collection of rental providers.</param>
    /// <param name="validator">The search request validator.</param>
    /// <param name="premiumDrivePricingStrategy">The pricing strategy for PremiumDrive.</param>
    /// <param name="budgetWheelsPricingStrategy">The pricing strategy for BudgetWheels.</param>
    public CarRentalService(
        IEnumerable<ICarRentalProvider> providers,
        SearchRequestValidator validator,
        PremiumDrivePricingStrategy premiumDrivePricingStrategy,
        BudgetWheelsPricingStrategy budgetWheelsPricingStrategy)
    {
        _providers = providers ?? throw new ArgumentNullException(nameof(providers));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _premiumDrivePricingStrategy = premiumDrivePricingStrategy ?? throw new ArgumentNullException(nameof(premiumDrivePricingStrategy));
        _budgetWheelsPricingStrategy = budgetWheelsPricingStrategy ?? throw new ArgumentNullException(nameof(budgetWheelsPricingStrategy));
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

        // Validate request
        var validationErrors = _validator.Validate(request);
        if (validationErrors.Any())
        {
            throw new InvalidOperationException(string.Join("; ", validationErrors));
        }

        var searchId = Guid.NewGuid();
        var allVehicles = new List<ProviderVehicle>();

        // Query all providers
        var providerTasks = _providers.Select(provider => provider.SearchAsync(request));
        var providerResults = await Task.WhenAll(providerTasks);

        // Collect results from all providers
        foreach (var result in providerResults)
        {
            allVehicles.AddRange(result);
        }

        // Filter out unavailable vehicles (BudgetWheels may return unavailable)
        var availableVehicles = allVehicles.Where(v => v.IsAvailable).ToList();

        // Normalize and calculate pricing
        var normalizedVehicles = new List<ProviderVehicleDto>();
        var fromDate = DateOnly.FromDateTime(request.From);
        var toDate = DateOnly.FromDateTime(request.To);

        foreach (var vehicle in availableVehicles)
        {
            var pricing = GetPricingStrategy(vehicle);
            var totalPrice = pricing.CalculateTotalPrice(vehicle.DailyRate, fromDate, toDate);

            var dto = new ProviderVehicleDto
            {
                VehicleId = Guid.NewGuid(), // Generate new ID for this quote
                Provider = vehicle.ProviderVehicleId.StartsWith("PD-") ? "PremiumDrive" : "BudgetWheels",
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

        return response;
    }

    /// <summary>
    /// Determines the appropriate pricing strategy for a vehicle based on provider.
    /// </summary>
    private IPricingStrategy GetPricingStrategy(ProviderVehicle vehicle)
    {
        return vehicle.ProviderVehicleId.StartsWith("PD-")
            ? _premiumDrivePricingStrategy
            : _budgetWheelsPricingStrategy;
    }
}
