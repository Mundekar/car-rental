namespace CarRental.Tests.Endpoints;

using System.Net;
using CarRental.Api.DTOs;
using CarRental.Api.Endpoints;
using CarRental.Api.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

/// <summary>
/// Tests for search endpoints.
/// </summary>
public class SearchEndpointsTests
{
    /// <summary>
    /// Verifies validation exceptions from the service are mapped to HTTP 400.
    /// </summary>
    [Fact]
    public async Task SearchCars_ReturnsBadRequest_WhenServiceThrowsInvalidOperationException()
    {
        var serviceMock = new Mock<ICarRentalService>();
        serviceMock
            .Setup(service => service.SearchCarsAsync(It.IsAny<SearchRequestDto>()))
            .ThrowsAsync(new InvalidOperationException("Validation failed"));

        await using var app = await BuildAppAsync(serviceMock.Object);
        var client = app.GetTestClient();

        var response = await client.GetAsync("/cars/search?pickup=Mumbai&from=2026-08-10&to=2026-08-12");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    /// <summary>
    /// Verifies unexpected exceptions from the service are mapped to HTTP 500.
    /// </summary>
    [Fact]
    public async Task SearchCars_ReturnsInternalServerError_WhenServiceThrowsUnexpectedException()
    {
        var serviceMock = new Mock<ICarRentalService>();
        serviceMock
            .Setup(service => service.SearchCarsAsync(It.IsAny<SearchRequestDto>()))
            .ThrowsAsync(new Exception("Unexpected failure"));

        await using var app = await BuildAppAsync(serviceMock.Object);
        var client = app.GetTestClient();

        var response = await client.GetAsync("/cars/search?pickup=Mumbai&from=2026-08-10&to=2026-08-12");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    /// <summary>
    /// Verifies malformed date input is rejected at endpoint level with HTTP 400.
    /// </summary>
    [Fact]
    public async Task SearchCars_ReturnsBadRequest_WhenDateFormatIsInvalid()
    {
        var serviceMock = new Mock<ICarRentalService>(MockBehavior.Strict);

        await using var app = await BuildAppAsync(serviceMock.Object);
        var client = app.GetTestClient();

        var response = await client.GetAsync("/cars/search?pickup=Mumbai&from=invalid-date&to=2026-08-12");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        serviceMock.Verify(service => service.SearchCarsAsync(It.IsAny<SearchRequestDto>()), Times.Never);
    }

    private static async Task<WebApplication> BuildAppAsync(ICarRentalService carRentalService)
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();

        builder.Services.AddLogging();
        builder.Services.AddSingleton(carRentalService);

        var app = builder.Build();
        app.MapSearchEndpoints();

        await app.StartAsync();
        return app;
    }
}