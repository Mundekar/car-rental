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
    /// Gets or sets the provider name.
    /// </summary>
    public string Provider { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the selected vehicle category label.
    /// </summary>
    public string VehicleCategory { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the selected vehicle make.
    /// </summary>
    public string VehicleMake { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the selected vehicle model.
    /// </summary>
    public string VehicleModel { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the selected vehicle daily rate.
    /// </summary>
    public decimal DailyRate { get; set; }

    /// <summary>
    /// Gets or sets the selected vehicle total price.
    /// </summary>
    public decimal TotalPrice { get; set; }

    /// <summary>
    /// Gets or sets the selected insurance type label.
    /// </summary>
    public string InsuranceType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the selected cancellation policy label.
    /// </summary>
    public string CancellationPolicy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the pickup location.
    /// </summary>
    public string PickupLocation { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the pickup date.
    /// </summary>
    public DateTime PickupDate { get; set; }

    /// <summary>
    /// Gets or sets the return date.
    /// </summary>
    public DateTime ReturnDate { get; set; }
}
