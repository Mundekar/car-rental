namespace CarRental.Api.Providers;

using CarRental.Api.Common;
using CarRental.Api.DTOs;
using CarRental.Api.Interfaces;
using CarRental.Api.Models;

/// <summary>
/// Provider implementation for BudgetWheels rental service.
/// BudgetWheels offers base daily pricing with 20% weekend surcharge, basic insurance, and non-refundable bookings.
/// Returns a mix of available and unavailable vehicles.
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
    /// Returns deterministic fleet with mix of available and unavailable vehicles.
    /// Prices calculated with 20% weekend surcharge on Friday, Saturday, Sunday nights.
    /// </summary>
    /// <param name="request">The search request containing location, dates, and optional category filter.</param>
    /// <returns>A collection of provider vehicles (both available and unavailable) with dynamic pricing.</returns>
    public Task<IEnumerable<ProviderVehicle>> SearchAsync(SearchRequestDto request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        var vehicles = GetDeterministicFleet(request);
        return Task.FromResult(vehicles.AsEnumerable());
    }

    /// <summary>
    /// Calculates total price with 20% weekend surcharge.
    /// Weekend nights are Friday, Saturday, Sunday.
    /// </summary>
    private decimal CalculateTotalPriceWithWeekendSurcharge(DateTime from, DateTime to, decimal baseRate)
    {
        var totalPrice = 0m;
        var currentDate = from.Date;

        while (currentDate < to.Date)
        {
            var dayOfWeek = currentDate.DayOfWeek;
            var isWeekendNight = dayOfWeek == DayOfWeek.Friday ||
                                dayOfWeek == DayOfWeek.Saturday ||
                                dayOfWeek == DayOfWeek.Sunday;

            if (isWeekendNight)
            {
                totalPrice += baseRate * 1.2m; // 20% surcharge
            }
            else
            {
                totalPrice += baseRate;
            }

            currentDate = currentDate.AddDays(1);
        }

        return totalPrice;
    }

    /// <summary>
    /// Returns deterministic vehicle fleet data for BudgetWheels with mix of available/unavailable.
    /// </summary>
    private List<ProviderVehicle> GetDeterministicFleet(SearchRequestDto request)
    {
        var vehicles = new List<ProviderVehicle>
        {
            // Economy vehicles - Base rate: $35
            new()
            {
                ProviderVehicleId = "BW-ECO-001",
                Category = VehicleCategory.Economy,
                Make = "Kia",
                Model = "Rio",
                Year = 2022,
                DailyRate = 35m,
                TotalPrice = CalculateTotalPriceWithWeekendSurcharge(request.From, request.To, 35m),
                InsuranceType = InsuranceType.Basic,
                CancellationPolicy = CancellationPolicy.NonRefundable,
                IsAvailable = true,
                UnavailabilityReason = null,
                ProviderType = ProviderType.BudgetWheels
            },
            new()
            {
                ProviderVehicleId = "BW-ECO-002",
                Category = VehicleCategory.Economy,
                Make = "Nissan",
                Model = "Versa",
                Year = 2022,
                DailyRate = 33m,
                TotalPrice = CalculateTotalPriceWithWeekendSurcharge(request.From, request.To, 33m),
                InsuranceType = InsuranceType.Basic,
                CancellationPolicy = CancellationPolicy.NonRefundable,
                IsAvailable = false,
                UnavailabilityReason = "Reserved for other dates",
                ProviderType = ProviderType.BudgetWheels
            },

            // Compact vehicles - Base rate: $45
            new()
            {
                ProviderVehicleId = "BW-CMP-001",
                Category = VehicleCategory.Compact,
                Make = "Volkswagen",
                Model = "Golf",
                Year = 2022,
                DailyRate = 45m,
                TotalPrice = CalculateTotalPriceWithWeekendSurcharge(request.From, request.To, 45m),
                InsuranceType = InsuranceType.Basic,
                CancellationPolicy = CancellationPolicy.NonRefundable,
                IsAvailable = true,
                UnavailabilityReason = null,
                ProviderType = ProviderType.BudgetWheels
            },
            new()
            {
                ProviderVehicleId = "BW-CMP-002",
                Category = VehicleCategory.Compact,
                Make = "Hyundai",
                Model = "i30",
                Year = 2022,
                DailyRate = 43m,
                TotalPrice = CalculateTotalPriceWithWeekendSurcharge(request.From, request.To, 43m),
                InsuranceType = InsuranceType.Basic,
                CancellationPolicy = CancellationPolicy.NonRefundable,
                IsAvailable = false,
                UnavailabilityReason = "Under maintenance",
                ProviderType = ProviderType.BudgetWheels
            },

            // SUV vehicles - Base rate: $65
            new()
            {
                ProviderVehicleId = "BW-SUV-001",
                Category = VehicleCategory.SUV,
                Make = "Chevrolet",
                Model = "Trax",
                Year = 2022,
                DailyRate = 65m,
                TotalPrice = CalculateTotalPriceWithWeekendSurcharge(request.From, request.To, 65m),
                InsuranceType = InsuranceType.Basic,
                CancellationPolicy = CancellationPolicy.NonRefundable,
                IsAvailable = true,
                UnavailabilityReason = null,
                ProviderType = ProviderType.BudgetWheels
            },
            new()
            {
                ProviderVehicleId = "BW-SUV-002",
                Category = VehicleCategory.SUV,
                Make = "Kia",
                Model = "Seltos",
                Year = 2022,
                DailyRate = 68m,
                TotalPrice = CalculateTotalPriceWithWeekendSurcharge(request.From, request.To, 68m),
                InsuranceType = InsuranceType.Basic,
                CancellationPolicy = CancellationPolicy.NonRefundable,
                IsAvailable = true,
                UnavailabilityReason = null,
                ProviderType = ProviderType.BudgetWheels
            },

            // Minivan vehicles - Base rate: $58
            new()
            {
                ProviderVehicleId = "BW-MIN-001",
                Category = VehicleCategory.Minivan,
                Make = "Kia",
                Model = "Carnival",
                Year = 2022,
                DailyRate = 58m,
                TotalPrice = CalculateTotalPriceWithWeekendSurcharge(request.From, request.To, 58m),
                InsuranceType = InsuranceType.Basic,
                CancellationPolicy = CancellationPolicy.NonRefundable,
                IsAvailable = true,
                UnavailabilityReason = null,
                ProviderType = ProviderType.BudgetWheels
            },
            new()
            {
                ProviderVehicleId = "BW-MIN-002",
                Category = VehicleCategory.Minivan,
                Make = "Toyota",
                Model = "Sienna",
                Year = 2022,
                DailyRate = 62m,
                TotalPrice = CalculateTotalPriceWithWeekendSurcharge(request.From, request.To, 62m),
                InsuranceType = InsuranceType.Basic,
                CancellationPolicy = CancellationPolicy.NonRefundable,
                IsAvailable = false,
                UnavailabilityReason = "Vehicle not available for selected dates",
                ProviderType = ProviderType.BudgetWheels
            }
        };

        // Filter by category if provided
        if (!string.IsNullOrWhiteSpace(request.Category) &&
            Enum.TryParse<VehicleCategory>(request.Category, ignoreCase: true, out var category))
        {
            vehicles = vehicles.Where(v => v.Category == category).ToList();
        }

        return vehicles;
    }
}

