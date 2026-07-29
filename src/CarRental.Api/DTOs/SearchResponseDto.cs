namespace CarRental.Api.DTOs;

/// <summary>
/// Data transfer object for search response.
/// </summary>
public class SearchResponseDto
{
    /// <summary>
    /// Gets or sets the unique search identifier.
    /// </summary>
    public Guid SearchId { get; set; }

    /// <summary>
    /// Gets or sets the requested pickup location.
    /// </summary>
    public string PickupLocation { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the rental start date.
    /// </summary>
    public DateTime FromDate { get; set; }

    /// <summary>
    /// Gets or sets the rental end date.
    /// </summary>
    public DateTime ToDate { get; set; }

    /// <summary>
    /// Gets or sets the number of rental days.
    /// </summary>
    public int DaysCount { get; set; }

    /// <summary>
    /// Gets or sets the collection of available vehicle quotes sorted by price.
    /// </summary>
    public List<ProviderVehicleDto> Results { get; set; } = new();
}
