namespace CarRental.Api.Endpoints;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using CarRental.Api.Common;
using CarRental.Api.DTOs;
using CarRental.Api.Interfaces;

/// <summary>
/// Endpoint registration for booking operations.
/// Provides HTTP endpoints for creating and retrieving car rental bookings.
/// </summary>
public static class BookingEndpoints
{
    /// <summary>
    /// Maps booking endpoints to the application route builder.
    /// </summary>
    /// <param name="routes">The route builder for Minimal API endpoints.</param>
    public static void MapBookingEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes
            .MapGroup("/cars")
            .WithName("Bookings")
            .WithOpenApi();

        group.MapPost("/book", CreateBooking)
            .WithName("CreateBooking")
            .WithOpenApi()
            .WithDescription("Create a new car rental booking with document validation");

        group.MapGet("/booking/{reference}", GetBooking)
            .WithName("GetBooking")
            .WithOpenApi()
            .WithDescription("Retrieve booking details by reference number");
    }

    /// <summary>
    /// Handles POST /cars/book requests to create a new booking.
    /// </summary>
    /// <param name="request">The booking request containing driver and vehicle details.</param>
    /// <param name="bookingService">The injected booking service.</param>
    /// <param name="logger">The logger for recording booking operations.</param>
    /// <returns>201 Created with booking confirmation, or 422 Unprocessable Entity if validation fails.</returns>
    private static async Task<IResult> CreateBooking(
        BookingRequestDto request,
        IBookingService bookingService,
        ILogger<Program> logger)
    {
        try
        {
            logger.LogInformation(
                "Booking creation requested: DriverName={DriverName} VehicleId={VehicleId}",
                request?.DriverName,
                request?.VehicleId);

            var response = await bookingService.CreateBookingAsync(request);

            logger.LogInformation(
                "Booking created successfully: Reference={Reference} TotalPrice={TotalPrice}",
                response.ReferenceNumber,
                response.TotalPrice);

            return Results.Created($"/cars/booking/{response.ReferenceNumber}", response);
        }
        catch (BookingValidationException ex)
        {
            // Required field missing or malformed — 400 Bad Request
            logger.LogWarning(ex, "Booking field validation failed");
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            // Document/location combination invalid — 422 Unprocessable Entity
            logger.LogWarning(ex, "Booking document validation failed");
            return Results.UnprocessableEntity(new { message = ex.Message });
        }
        catch (ArgumentNullException ex)
        {
            logger.LogWarning(ex, "Null argument in booking request");
            return Results.BadRequest(new { message = "Booking request is required." });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error creating booking: DriverName={DriverName} VehicleId={VehicleId}",
                request?.DriverName, request?.VehicleId);
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Handles GET /cars/booking/{reference} requests to retrieve a booking.
    /// </summary>
    /// <param name="reference">The booking reference number.</param>
    /// <param name="bookingService">The injected booking service.</param>
    /// <param name="logger">The logger for recording booking operations.</param>
    /// <returns>200 OK with booking details, or 404 Not Found if booking doesn't exist.</returns>
    private static async Task<IResult> GetBooking(
        string reference,
        IBookingService bookingService,
        ILogger<Program> logger)
    {
        try
        {
            logger.LogInformation("Booking retrieval requested: Reference={Reference}", reference);

            if (string.IsNullOrWhiteSpace(reference))
            {
                logger.LogWarning("Booking retrieval requested with empty reference");
                return Results.BadRequest(new { message = "Booking reference is required." });
            }

            var booking = await bookingService.GetBookingByReferenceAsync(reference);

            if (booking == null)
            {
                logger.LogWarning("Booking not found: Reference={Reference}", reference);
                return Results.NotFound(new { message = $"Booking with reference '{reference}' not found." });
            }

            logger.LogInformation("Booking retrieved successfully: Reference={Reference}", reference);
            return Results.Ok(booking);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error retrieving booking: Reference={Reference}", reference);
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
