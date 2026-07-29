namespace CarRental.Api.DTOs;

using CarRental.Api.Common;

/// <summary>
/// Data transfer object for vehicle quote in search results.
/// </summary>
public class ProviderVehicleDto
{
    /// <summary>
    /// Gets or sets the unique vehicle identifier.
    /// </summary>
    public Guid VehicleId { get; set; }

    /// <summary>
    /// Gets or sets the provider name.
    /// </summary>
    public string Provider { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the vehicle category.
    /// </summary>
    public VehicleCategory Category { get; set; }

    /// <summary>
    /// Gets or sets the vehicle manufacturer.
    /// </summary>
    public string Make { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the vehicle model name.
    /// </summary>
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the daily rate.
    /// </summary>
    public decimal DailyRate { get; set; }

    /// <summary>
    /// Gets or sets the total rental price.
    /// </summary>
    public decimal TotalPrice { get; set; }

    /// <summary>
    /// Gets or sets the price breakdown by day.
    /// </summary>
    public Dictionary<string, decimal>? PriceBreakdown { get; set; }

    /// <summary>
    /// Gets or sets the insurance type.
    /// </summary>
    public InsuranceType InsuranceType { get; set; }

    /// <summary>
    /// Gets or sets the cancellation policy.
    /// </summary>
    public CancellationPolicy CancellationPolicy { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the vehicle is available.
    /// </summary>
    public bool IsAvailable { get; set; }
}
