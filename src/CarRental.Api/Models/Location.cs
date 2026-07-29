namespace CarRental.Api.Models;

/// <summary>
/// Represents a geographic pickup location with validation information.
/// </summary>
public class Location
{
    /// <summary>
    /// Gets or sets the city name.
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the country name or code.
    /// </summary>
    public string Country { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the location is international.
    /// </summary>
    public bool IsInternational { get; set; }
}
