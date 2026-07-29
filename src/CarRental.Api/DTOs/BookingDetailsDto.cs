namespace CarRental.Api.DTOs;

/// <summary>
/// Data transfer object for vehicle details in booking confirmation.
/// </summary>
public class BookingDetailsDto
{
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
}
