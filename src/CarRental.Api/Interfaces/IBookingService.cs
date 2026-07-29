namespace CarRental.Api.Interfaces;

using CarRental.Api.DTOs;
using CarRental.Api.Models;

/// <summary>
/// Defines the contract for booking service operations.
/// </summary>
public interface IBookingService
{
    /// <summary>
    /// Creates a new car rental booking.
    /// </summary>
    /// <param name="request">The booking request details.</param>
    /// <returns>Booking confirmation with reference number.</returns>
    Task<BookingResponseDto> CreateBookingAsync(BookingRequestDto request);

    /// <summary>
    /// Retrieves a booking by its reference number.
    /// </summary>
    /// <param name="referenceNumber">The booking reference number.</param>
    /// <returns>The booking details or null if not found.</returns>
    Task<BookingResponseDto?> GetBookingByReferenceAsync(string referenceNumber);

    /// <summary>
    /// Retrieves all stored bookings.
    /// </summary>
    /// <returns>A collection of all bookings.</returns>
    Task<IEnumerable<Booking>> GetAllBookingsAsync();
}
