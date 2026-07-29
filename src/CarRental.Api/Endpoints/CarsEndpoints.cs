namespace CarRental.Api.Endpoints;

using CarRental.Api.DTOs;
using CarRental.Api.Interfaces;
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
            .WithDescription("Search for available rental vehicles across all providers");
    }

    /// <summary>
    /// Handles GET /cars/search requests.
    /// Searches for available vehicles based on provided criteria.
    /// </summary>
    /// <param name="pickup">The pickup location (required).</param>
    /// <param name="from">The rental start date in ISO format YYYY-MM-DD (required).</param>
    /// <param name="to">The rental end date in ISO format YYYY-MM-DD (required).</param>
    /// <param name="category">The vehicle category filter, optional (Economy, Compact, SUV, Minivan).</param>
    /// <param name="service">The car rental search service.</param>
    /// <returns>Search results or validation error.</returns>
    private static async Task<IResult> SearchCars(
        string? pickup,
        string? from,
        string? to,
        string? category,
        ICarRentalService service)
    {
        try
        {
            // Parse dates
            if (!DateTime.TryParse(from, out var fromDate))
            {
                return Results.BadRequest(new { error = "Invalid 'from' date format. Use YYYY-MM-DD." });
            }

            if (!DateTime.TryParse(to, out var toDate))
            {
                return Results.BadRequest(new { error = "Invalid 'to' date format. Use YYYY-MM-DD." });
            }

            // Create request DTO
            var request = new SearchRequestDto
            {
                Pickup = pickup ?? string.Empty,
                From = fromDate,
                To = toDate,
                Category = category
            };

            // Execute search
            var result = await service.SearchCarsAsync(request);

            return Results.Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            // Validation error from service
            return Results.BadRequest(new { error = ex.Message });
        }
        catch (Exception)
        {
            // Unexpected error
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
