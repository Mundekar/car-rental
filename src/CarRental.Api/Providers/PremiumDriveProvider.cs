namespace CarRental.Api.Providers;

using CarRental.Api.Common;
using CarRental.Api.DTOs;
using CarRental.Api.Interfaces;
using CarRental.Api.Models;

/// <summary>
/// Provider implementation for PremiumDrive rental service.
/// PremiumDrive offers flat daily pricing with comprehensive insurance and free 48-hour cancellation.
/// All vehicles are always available.
/// </summary>
public class PremiumDriveProvider : ICarRentalProvider
{
    /// <summary>
    /// Gets the name of the provider.
    /// </summary>
    public string ProviderName => "PremiumDrive";

    /// <summary>
    /// Searches for available vehicles from PremiumDrive.
    /// Returns deterministic fleet with all categories always available.
    /// </summary>
    /// <param name="request">The search request containing location, dates, and optional category filter.</param>
    /// <returns>A collection of available provider vehicles with flat daily pricing.</returns>
    public Task<IEnumerable<ProviderVehicle>> SearchAsync(SearchRequestDto request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        var vehicles = GetDeterministicFleet(request);
        return Task.FromResult(vehicles.AsEnumerable());
    }

    /// <summary>
    /// Returns deterministic vehicle fleet data for PremiumDrive.
    /// </summary>
    private List<ProviderVehicle> GetDeterministicFleet(SearchRequestDto request)
    {
        var days = (request.To.Date - request.From.Date).Days;
        if (days <= 0)
        {
            days = 1;
        }

        var vehicles = new List<ProviderVehicle>
        {
            // Economy vehicles - Daily rate: $45
            new()
            {
                ProviderVehicleId = "PD-ECO-001",
                Category = VehicleCategory.Economy,
                Make = "Toyota",
                Model = "Corolla",
                Year = 2023,
                DailyRate = 45m,
                TotalPrice = 45m * days,
                InsuranceType = InsuranceType.Comprehensive,
                CancellationPolicy = CancellationPolicy.Free48Hours,
                IsAvailable = true,
                UnavailabilityReason = null,
                ProviderType = ProviderType.PremiumDrive
            },
            new()
            {
                ProviderVehicleId = "PD-ECO-002",
                Category = VehicleCategory.Economy,
                Make = "Hyundai",
                Model = "Elantra",
                Year = 2023,
                DailyRate = 42m,
                TotalPrice = 42m * days,
                InsuranceType = InsuranceType.Comprehensive,
                CancellationPolicy = CancellationPolicy.Free48Hours,
                IsAvailable = true,
                UnavailabilityReason = null,
                ProviderType = ProviderType.PremiumDrive
            },

            // Compact vehicles - Daily rate: $55
            new()
            {
                ProviderVehicleId = "PD-CMP-001",
                Category = VehicleCategory.Compact,
                Make = "Honda",
                Model = "Civic",
                Year = 2023,
                DailyRate = 55m,
                TotalPrice = 55m * days,
                InsuranceType = InsuranceType.Comprehensive,
                CancellationPolicy = CancellationPolicy.Free48Hours,
                IsAvailable = true,
                UnavailabilityReason = null,
                ProviderType = ProviderType.PremiumDrive
            },
            new()
            {
                ProviderVehicleId = "PD-CMP-002",
                Category = VehicleCategory.Compact,
                Make = "Mazda",
                Model = "3",
                Year = 2023,
                DailyRate = 52m,
                TotalPrice = 52m * days,
                InsuranceType = InsuranceType.Comprehensive,
                CancellationPolicy = CancellationPolicy.Free48Hours,
                IsAvailable = true,
                UnavailabilityReason = null,
                ProviderType = ProviderType.PremiumDrive
            },

            // SUV vehicles - Daily rate: $85
            new()
            {
                ProviderVehicleId = "PD-SUV-001",
                Category = VehicleCategory.SUV,
                Make = "Toyota",
                Model = "CR-V",
                Year = 2023,
                DailyRate = 85m,
                TotalPrice = 85m * days,
                InsuranceType = InsuranceType.Comprehensive,
                CancellationPolicy = CancellationPolicy.Free48Hours,
                IsAvailable = true,
                UnavailabilityReason = null,
                ProviderType = ProviderType.PremiumDrive
            },
            new()
            {
                ProviderVehicleId = "PD-SUV-002",
                Category = VehicleCategory.SUV,
                Make = "Ford",
                Model = "Edge",
                Year = 2023,
                DailyRate = 90m,
                TotalPrice = 90m * days,
                InsuranceType = InsuranceType.Comprehensive,
                CancellationPolicy = CancellationPolicy.Free48Hours,
                IsAvailable = true,
                UnavailabilityReason = null,
                ProviderType = ProviderType.PremiumDrive
            },

            // Minivan vehicles - Daily rate: $75
            new()
            {
                ProviderVehicleId = "PD-MIN-001",
                Category = VehicleCategory.Minivan,
                Make = "Honda",
                Model = "Odyssey",
                Year = 2023,
                DailyRate = 75m,
                TotalPrice = 75m * days,
                InsuranceType = InsuranceType.Comprehensive,
                CancellationPolicy = CancellationPolicy.Free48Hours,
                IsAvailable = true,
                UnavailabilityReason = null,
                ProviderType = ProviderType.PremiumDrive
            },
            new()
            {
                ProviderVehicleId = "PD-MIN-002",
                Category = VehicleCategory.Minivan,
                Make = "Chrysler",
                Model = "Pacifica",
                Year = 2023,
                DailyRate = 78m,
                TotalPrice = 78m * days,
                InsuranceType = InsuranceType.Comprehensive,
                CancellationPolicy = CancellationPolicy.Free48Hours,
                IsAvailable = true,
                UnavailabilityReason = null,
                ProviderType = ProviderType.PremiumDrive
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

