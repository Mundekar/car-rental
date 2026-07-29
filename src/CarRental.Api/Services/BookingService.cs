namespace CarRental.Api.Services;

using System.Collections.Concurrent;
using CarRental.Api.DTOs;
using CarRental.Api.Interfaces;
using CarRental.Api.Models;

/// <summary>
/// Service for managing car rental bookings.
/// Orchestrates booking creation, validation, and retrieval with in-memory thread-safe storage.
/// </summary>
public class BookingService : IBookingService
{
    private readonly IDocumentValidationService _documentValidationService;
    private static readonly ConcurrentDictionary<string, Booking> BookingStore = new();
    private static long _bookingCounter = 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="BookingService"/> class.
    /// </summary>
    /// <param name="documentValidationService">Service for validating travel documents.</param>
    public BookingService(IDocumentValidationService documentValidationService)
    {
        _documentValidationService = documentValidationService ?? throw new ArgumentNullException(nameof(documentValidationService));
    }

    /// <summary>
    /// Creates a new car rental booking with document validation and reference generation.
    /// </summary>
    /// <param name="request">The booking request details.</param>
    /// <returns>Booking confirmation with reference number.</returns>
    /// <exception>
    /// <cref>InvalidOperationException</cref> if document validation fails or request is invalid.
    /// </exception>
    public Task<BookingResponseDto> CreateBookingAsync(BookingRequestDto request)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Validate required fields
        ValidateRequest(request);

        // Validate document for location
        var docTypeString = request.DocumentType.ToString();
        if (!_documentValidationService.IsDocumentValidForLocation(docTypeString, request.PickupLocation))
        {
            throw new InvalidOperationException(
                $"{docTypeString} is not valid for {request.PickupLocation}. " +
                (_documentValidationService.IsInternationalLocation(request.PickupLocation)
                    ? "Passport is required for international pickup locations."
                    : "Either NationalId or Passport is required."));
        }

        // Generate booking reference
        var referenceNumber = GenerateBookingReference();

        // Create booking object
        var booking = new Booking
        {
            ReferenceNumber = referenceNumber,
            DriverName = request.DriverName,
            DocumentType = request.DocumentType,
            DocumentNumber = request.DocumentNumber,
            VehicleId = request.VehicleId,
            Provider = request.Provider,
            PickupLocation = request.PickupLocation,
            FromDate = request.PickupDate,
            ToDate = request.ReturnDate,
            TotalPrice = 0m, // Would be populated from search results in real scenario
            BookingDate = DateTime.UtcNow,
            InsuranceType = Common.InsuranceType.Basic, // Default, would come from vehicle selection
            CancellationPolicy = Common.CancellationPolicy.Free48Hours // Default, would come from vehicle selection
        };

        // Store booking
        if (!BookingStore.TryAdd(referenceNumber, booking))
        {
            throw new InvalidOperationException($"Failed to store booking with reference {referenceNumber}");
        }

        // Return response
        return Task.FromResult(MapToResponseDto(booking));
    }

    /// <summary>
    /// Retrieves a booking by its reference number.
    /// </summary>
    /// <param name="referenceNumber">The booking reference number.</param>
    /// <returns>The booking details or null if not found.</returns>
    public Task<BookingResponseDto?> GetBookingByReferenceAsync(string referenceNumber)
    {
        if (string.IsNullOrWhiteSpace(referenceNumber))
        {
            return Task.FromResult<BookingResponseDto?>(null);
        }

        if (BookingStore.TryGetValue(referenceNumber, out var booking))
        {
            return Task.FromResult<BookingResponseDto?>(MapToResponseDto(booking));
        }

        return Task.FromResult<BookingResponseDto?>(null);
    }

    /// <summary>
    /// Retrieves all stored bookings.
    /// </summary>
    /// <returns>A collection of all bookings.</returns>
    public Task<IEnumerable<Booking>> GetAllBookingsAsync()
    {
        return Task.FromResult(BookingStore.Values.AsEnumerable());
    }

    /// <summary>
    /// Generates a deterministic booking reference in format: CR-YYYYMMDD-XXXXXX.
    /// </summary>
    /// <returns>A unique booking reference.</returns>
    private static string GenerateBookingReference()
    {
        var counter = Interlocked.Increment(ref _bookingCounter);
        var datePrefix = DateTime.UtcNow.ToString("yyyyMMdd");
        var sequenceNumber = counter.ToString("D6");
        return $"CR-{datePrefix}-{sequenceNumber}";
    }

    /// <summary>
    /// Validates that all required fields are present in the booking request.
    /// </summary>
    /// <param name="request">The booking request to validate.</param>
    /// <exception>
    /// <cref>InvalidOperationException</cref> if any required field is missing or invalid.
    /// </exception>
    private static void ValidateRequest(BookingRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.DriverName))
        {
            throw new InvalidOperationException("Driver name is required.");
        }

        if (string.IsNullOrWhiteSpace(request.DocumentNumber))
        {
            throw new InvalidOperationException("Document number is required.");
        }

        if (request.VehicleId == Guid.Empty)
        {
            throw new InvalidOperationException("Vehicle ID is required.");
        }

        if (string.IsNullOrWhiteSpace(request.PickupLocation))
        {
            throw new InvalidOperationException("Pickup location is required.");
        }

        if (request.PickupDate == default)
        {
            throw new InvalidOperationException("Pickup date is required.");
        }

        if (request.ReturnDate == default)
        {
            throw new InvalidOperationException("Return date is required.");
        }

        if (request.ReturnDate <= request.PickupDate)
        {
            throw new InvalidOperationException("Return date must be after pickup date.");
        }
    }

    /// <summary>
    /// Maps a booking domain object to a response DTO.
    /// </summary>
    /// <param name="booking">The booking to map.</param>
    /// <returns>The booking response DTO.</returns>
    private static BookingResponseDto MapToResponseDto(Booking booking)
    {
        var daysCount = (int)(booking.ToDate - booking.FromDate).TotalDays;
        if (daysCount <= 0)
        {
            daysCount = 1;
        }

        return new BookingResponseDto
        {
            ReferenceNumber = booking.ReferenceNumber,
            DriverName = booking.DriverName,
            VehicleCategory = Common.VehicleCategory.Economy, // Would come from search results
            VehicleDetails = new BookingDetailsDto
            {
                Make = "Vehicle Make",  // Would come from vehicle lookup
                Model = "Vehicle Model",  // Would come from vehicle lookup
                Year = 2024  // Would come from vehicle lookup
            },
            Provider = booking.Provider,
            PickupLocation = booking.PickupLocation,
            FromDate = booking.FromDate,
            ToDate = booking.ToDate,
            DaysCount = daysCount,
            DailyRate = booking.TotalPrice > 0 ? booking.TotalPrice / daysCount : 0m,
            TotalPrice = booking.TotalPrice,
            InsuranceType = booking.InsuranceType,
            CancellationPolicy = booking.CancellationPolicy,
            BookingConfirmedAt = booking.BookingDate
        };
    }
}
