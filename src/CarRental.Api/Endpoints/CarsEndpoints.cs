namespace CarRental.Api.Endpoints;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

/// <summary>
/// Endpoint registration for car search and availability operations.
/// </summary>
public static class CarsEndpoints
{
    /// <summary>
    /// Maps car search endpoints to the application route builder.
    /// </summary>
    /// <param name="routes">The route builder for Minimal API endpoints.</param>
    public static void MapCarsEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes
            .MapGroup("/cars")
            .WithName("Cars")
            .WithOpenApi();

        group.MapGet("/search", SearchCars)
            .WithName("SearchCars")
            .WithOpenApi()
            .WithDescription("Search for available rental vehicles");
    }

    /// <summary>
    /// Handles GET /cars/search requests.
    /// </summary>
    /// <returns>501 Not Implemented status.</returns>
    private static IResult SearchCars()
    {
        return Results.StatusCode(StatusCodes.Status501NotImplemented);
    }
}
