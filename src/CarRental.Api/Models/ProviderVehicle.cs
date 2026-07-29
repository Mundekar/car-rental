namespace CarRental.Api.Models;

/// <summary>
/// Represents a vehicle quote from a rental provider.
/// </summary>
public class ProviderVehicle
{
    /// <summary>
    /// Gets or sets the provider's internal vehicle identifier.
    /// </summary>
    public string ProviderVehicleId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the vehicle category.
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the vehicle manufacturer.
    /// </summary>
    public string Make { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the vehicle model name.
    /// </summary>
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the manufacturing year.
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// Gets or sets the daily rental rate.
    /// </summary>
    public decimal DailyRate { get; set; }

    /// <summary>
    /// Gets or sets the total rental price for the requested period.
    /// </summary>
    public decimal TotalPrice { get; set; }

    /// <summary>
    /// Gets or sets the insurance type included.
    /// </summary>
    public string InsuranceType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the cancellation policy.
    /// </summary>
    public string CancellationPolicy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the vehicle is available.
    /// </summary>
    public bool IsAvailable { get; set; }

    /// <summary>
    /// Gets or sets the reason if vehicle is unavailable.
    /// </summary>
    public string? UnavailabilityReason { get; set; }
}
