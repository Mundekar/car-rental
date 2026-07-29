namespace CarRental.Api.Services;

using CarRental.Api.DTOs;
using CarRental.Api.Interfaces;
using CarRental.Api.Models;

/// <summary>
/// Service for managing car rental bookings.
/// </summary>
public class BookingService : IBookingService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BookingService"/> class.
    /// </summary>
    public BookingService()
    {
    }

    /// <summary>
    /// Creates a new car rental booking.
    /// </summary>
    /// <param name="request">The booking request details.</param>
    /// <returns>Booking confirmation with reference number.</returns>
    public Task<BookingResponseDto> CreateBookingAsync(BookingRequestDto request)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Retrieves a booking by its reference number.
    /// </summary>
    /// <param name="referenceNumber">The booking reference number.</param>
    /// <returns>The booking details or null if not found.</returns>
    public Task<BookingResponseDto?> GetBookingByReferenceAsync(string referenceNumber)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Retrieves all stored bookings.
    /// </summary>
    /// <returns>A collection of all bookings.</returns>
    public Task<IEnumerable<Booking>> GetAllBookingsAsync()
    {
        throw new NotImplementedException();
    }
}
