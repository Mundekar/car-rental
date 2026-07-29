namespace CarRental.Api.DTOs;

using CarRental.Api.Common;

/// <summary>
/// Data transfer object for booking request parameters.
/// </summary>
public class BookingRequestDto
{
    /// <summary>
    /// Gets or sets the primary driver name.
    /// </summary>
    public string DriverName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the travel document type (NationalId or Passport).
    /// </summary>
    public DocumentType DocumentType { get; set; }

    /// <summary>
    /// Gets or sets the travel document number.
    /// </summary>
    public string DocumentNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the vehicle to book identifier.
    /// </summary>
    public Guid VehicleId { get; set; }

    /// <summary>
    /// Gets or sets the pickup location.
    /// </summary>
    public string PickupLocation { get; set; } = string.Empty;
}
