namespace CarRental.Api.Models;

/// <summary>
/// Represents a confirmed car rental booking.
/// </summary>
public class Booking
{
    /// <summary>
    /// Gets or sets the unique booking reference number.
    /// </summary>
    public string ReferenceNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the primary driver.
    /// </summary>
    public string DriverName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of travel document (NationalId or Passport).
    /// </summary>
    public string DocumentType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the travel document number.
    /// </summary>
    public string DocumentNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the booked vehicle identifier.
    /// </summary>
    public Guid VehicleId { get; set; }

    /// <summary>
    /// Gets or sets the rental provider name.
    /// </summary>
    public string Provider { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the pickup location.
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
    /// Gets or sets the total booking price.
    /// </summary>
    public decimal TotalPrice { get; set; }

    /// <summary>
    /// Gets or sets the booking creation timestamp.
    /// </summary>
    public DateTime BookingDate { get; set; }

    /// <summary>
    /// Gets or sets the insurance type included in the booking.
    /// </summary>
    public string InsuranceType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the cancellation policy description.
    /// </summary>
    public string CancellationPolicy { get; set; } = string.Empty;
}
