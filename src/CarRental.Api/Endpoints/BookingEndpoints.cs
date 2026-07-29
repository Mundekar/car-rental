namespace CarRental.Api.Endpoints;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

/// <summary>
/// Endpoint registration for booking operations.
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
            .WithDescription("Create a new car rental booking");

        group.MapGet("/booking/{reference}", GetBooking)
            .WithName("GetBooking")
            .WithOpenApi()
            .WithDescription("Retrieve booking details by reference number");
    }

    /// <summary>
    /// Handles POST /cars/book requests.
    /// </summary>
    /// <returns>501 Not Implemented status.</returns>
    private static IResult CreateBooking()
    {
        return Results.StatusCode(StatusCodes.Status501NotImplemented);
    }

    /// <summary>
    /// Handles GET /cars/booking/{reference} requests.
    /// </summary>
    /// <param name="reference">The booking reference number.</param>
    /// <returns>501 Not Implemented status.</returns>
    private static IResult GetBooking(string reference)
    {
        return Results.StatusCode(StatusCodes.Status501NotImplemented);
    }
}
