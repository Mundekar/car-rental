namespace CarRental.Api.DTOs;

/// <summary>
/// Data transfer object for search request parameters.
/// </summary>
public class SearchRequestDto
{
    /// <summary>
    /// Gets or sets the pickup location city.
    /// </summary>
    public string Pickup { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the rental start date.
    /// </summary>
    public DateTime From { get; set; }

    /// <summary>
    /// Gets or sets the rental end date.
    /// </summary>
    public DateTime To { get; set; }

    /// <summary>
    /// Gets or sets the optional vehicle category filter.
    /// </summary>
    public string? Category { get; set; }
}
