namespace CarRental.Api.Models;

/// <summary>
/// Represents a rental vehicle offered by a provider.
/// </summary>
public class Vehicle
{
    /// <summary>
    /// Gets or sets the unique identifier for the vehicle.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the vehicle category (Economy, Compact, SUV, Minivan).
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
    /// Gets or sets a value indicating whether the vehicle is available for booking.
    /// </summary>
    public bool IsAvailable { get; set; }

    /// <summary>
    /// Gets or sets the provider's internal vehicle reference identifier.
    /// </summary>
    public string ProviderReference { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the provider offering this vehicle.
    /// </summary>
    public string Provider { get; set; } = string.Empty;
}
