namespace CarRental.Api.DTOs;

using CarRental.Api.Common;

/// <summary>
/// Data transfer object for booking response confirmation.
/// </summary>
public class BookingResponseDto
{
    /// <summary>
    /// Gets or sets the booking confirmation reference number.
    /// </summary>
    public string ReferenceNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the driver name from the booking.
    /// </summary>
    public string DriverName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the booked vehicle category.
    /// </summary>
    public VehicleCategory VehicleCategory { get; set; }

    /// <summary>
    /// Gets or sets the vehicle booking details.
    /// </summary>
    public BookingDetailsDto? VehicleDetails { get; set; }

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
    /// Gets or sets the number of rental days.
    /// </summary>
    public int DaysCount { get; set; }

    /// <summary>
    /// Gets or sets the daily rental rate.
    /// </summary>
    public decimal DailyRate { get; set; }

    /// <summary>
    /// Gets or sets the total booking price.
    /// </summary>
    public decimal TotalPrice { get; set; }

    /// <summary>
    /// Gets or sets the insurance type.
    /// </summary>
    public InsuranceType InsuranceType { get; set; }

    /// <summary>
    /// Gets or sets the cancellation policy.
    /// </summary>
    public CancellationPolicy CancellationPolicy { get; set; }

    /// <summary>
    /// Gets or sets the booking confirmation timestamp.
    /// </summary>
    public DateTime BookingConfirmedAt { get; set; }
}
