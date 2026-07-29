namespace CarRental.Api.Models;

/// <summary>
/// Represents a geographic pickup location with validation information.
/// </summary>
public record Location(
    /// <summary>
    /// The city name.
    /// </summary>
    string City,
    /// <summary>
    /// The country name or code.
    /// </summary>
    string Country,
    /// <summary>
    /// A value indicating whether the location is international.
    /// </summary>
    bool IsInternational);
