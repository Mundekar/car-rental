namespace CarRental.Api.Endpoints;

using CarRental.Api.DTOs;
using CarRental.Api.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

/// <summary>
/// Endpoint registration for car search and availability operations.
/// </summary>
public static class SearchEndpoints
{
    /// <summary>
    /// Maps car search endpoints to the application route builder.
    /// </summary>
    /// <param name="routes">The route builder for Minimal API endpoints.</param>
    public static void MapSearchEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes
            .MapGroup("/cars")
            .WithName("Search")
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
    /// <param name="logger">The logger for recording search operations.</param>
    /// <returns>Search results or validation error.</returns>
    private static async Task<IResult> SearchCars(
        string? pickup,
        string? from,
        string? to,
        string? category,
        ICarRentalService service,
        ILogger<Program> logger)
    {
        try
        {
            logger.LogInformation(
                "Search request: Pickup={Pickup} From={From} To={To} Category={Category}",
                pickup,
                from,
                to,
                category);

            // Parse dates
            if (!DateTime.TryParse(from, out var fromDate))
            {
                logger.LogWarning("Invalid 'from' date format provided: {From}", from);
                return Results.BadRequest(new { error = "Invalid 'from' date format. Use YYYY-MM-DD." });
            }

            if (!DateTime.TryParse(to, out var toDate))
            {
                logger.LogWarning("Invalid 'to' date format provided: {To}", to);
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

            logger.LogInformation(
                "Search completed successfully: {ResultCount} vehicles found",
                result.Results.Count);

            return Results.Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            // Validation error from service
            logger.LogWarning(ex, "Search validation error");
            return Results.BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            // Unexpected error
            logger.LogError(ex, "Unexpected error during search: Pickup={Pickup} From={From} To={To}", pickup, from, to);
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}