namespace CarRental.Tests.Providers;

using CarRental.Api.Common;
using CarRental.Api.DTOs;
using CarRental.Api.Providers;
using Xunit;

/// <summary>
/// Unit tests for the PremiumDriveProvider.
/// </summary>
public class PremiumDriveProviderTests
{
    private readonly PremiumDriveProvider _provider = new();

    [Fact]
    public void ProviderName_ReturnsCorrectValue()
    {
        // Arrange & Act
        var name = _provider.ProviderName;

        // Assert
        Assert.Equal("PremiumDrive", name);
    }

    [Fact]
    public async Task SearchAsync_ReturnsAllVehicles_WhenNoCategoryFilter()
    {
        // Arrange
        var request = new SearchRequestDto
        {
            Pickup = "Mumbai",
            From = new DateTime(2026, 8, 1),
            To = new DateTime(2026, 8, 5),
            Category = null
        };

        // Act
        var result = await _provider.SearchAsync(request);

        // Assert
        var vehicles = result.ToList();
        Assert.NotEmpty(vehicles);
        Assert.Equal(8, vehicles.Count); // 2 vehicles per category × 4 categories
    }

    [Fact]
    public async Task SearchAsync_ReturnsOnlyFilteredCategory_WhenCategoryProvided()
    {
        // Arrange
        var request = new SearchRequestDto
        {
            Pickup = "Mumbai",
            From = new DateTime(2026, 8, 1),
            To = new DateTime(2026, 8, 5),
            Category = "Economy"
        };

        // Act
        var result = await _provider.SearchAsync(request);

        // Assert
        var vehicles = result.ToList();
        Assert.NotEmpty(vehicles);
        Assert.All(vehicles, v => Assert.Equal(VehicleCategory.Economy, v.Category));
    }

    [Fact]
    public async Task SearchAsync_CalculatesTotalPrice_BasedOnDayCount()
    {
        // Arrange
        var from = new DateTime(2026, 8, 1);
        var to = new DateTime(2026, 8, 6); // 5 days
        var request = new SearchRequestDto
        {
            Pickup = "Mumbai",
            From = from,
            To = to,
            Category = "Economy"
        };

        // Act
        var result = await _provider.SearchAsync(request);

        // Assert
        var vehicles = result.ToList();
        Assert.NotEmpty(vehicles);
        foreach (var vehicle in vehicles)
        {
            // PremiumDrive uses flat daily rate
            var expectedTotal = vehicle.DailyRate * 5;
            Assert.Equal(expectedTotal, vehicle.TotalPrice);
        }
    }

    [Fact]
    public async Task SearchAsync_AllVehiclesAvailable_PremiumDriveAlwaysAvailable()
    {
        // Arrange
        var request = new SearchRequestDto
        {
            Pickup = "London",
            From = new DateTime(2026, 8, 1),
            To = new DateTime(2026, 8, 15),
            Category = null
        };

        // Act
        var result = await _provider.SearchAsync(request);

        // Assert
        var vehicles = result.ToList();
        Assert.NotEmpty(vehicles);
        Assert.All(vehicles, v => Assert.True(v.IsAvailable));
        Assert.All(vehicles, v => Assert.Null(v.UnavailabilityReason));
    }

    [Fact]
    public async Task SearchAsync_AllVehiclesHaveComprehensiveInsurance()
    {
        // Arrange
        var request = new SearchRequestDto
        {
            Pickup = "Dubai",
            From = new DateTime(2026, 8, 1),
            To = new DateTime(2026, 8, 3),
            Category = null
        };

        // Act
        var result = await _provider.SearchAsync(request);

        // Assert
        var vehicles = result.ToList();
        Assert.NotEmpty(vehicles);
        Assert.All(vehicles, v => Assert.Equal(InsuranceType.Comprehensive, v.InsuranceType));
    }

    [Fact]
    public async Task SearchAsync_AllVehiclesHaveFree48HoursCancellation()
    {
        // Arrange
        var request = new SearchRequestDto
        {
            Pickup = "Singapore",
            From = new DateTime(2026, 8, 1),
            To = new DateTime(2026, 8, 7),
            Category = null
        };

        // Act
        var result = await _provider.SearchAsync(request);

        // Assert
        var vehicles = result.ToList();
        Assert.NotEmpty(vehicles);
        Assert.All(vehicles, v => Assert.Equal(CancellationPolicy.Free48Hours, v.CancellationPolicy));
    }

    [Fact]
    public async Task SearchAsync_ThrowsArgumentNullException_WhenRequestIsNull()
    {
        // Arrange & Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _provider.SearchAsync(null!));
    }

    [Fact]
    public async Task SearchAsync_IncludeAllVehicleCategories()
    {
        // Arrange
        var request = new SearchRequestDto
        {
            Pickup = "Dubai",
            From = new DateTime(2026, 8, 1),
            To = new DateTime(2026, 8, 5),
            Category = null
        };

        // Act
        var result = await _provider.SearchAsync(request);

        // Assert
        var vehicles = result.ToList();
        var categories = vehicles.Select(v => v.Category).Distinct().OrderBy(c => c).ToList();

        Assert.Equal(4, categories.Count);
        Assert.Contains(VehicleCategory.Economy, categories);
        Assert.Contains(VehicleCategory.Compact, categories);
        Assert.Contains(VehicleCategory.SUV, categories);
        Assert.Contains(VehicleCategory.Minivan, categories);
    }

    [Fact]
    public async Task SearchAsync_HandlesCaseInsensitiveCategoryFilter()
    {
        // Arrange
        var request = new SearchRequestDto
        {
            Pickup = "Mumbai",
            From = new DateTime(2026, 8, 1),
            To = new DateTime(2026, 8, 5),
            Category = "COMPACT" // uppercase
        };

        // Act
        var result = await _provider.SearchAsync(request);

        // Assert
        var vehicles = result.ToList();
        Assert.NotEmpty(vehicles);
        Assert.All(vehicles, v => Assert.Equal(VehicleCategory.Compact, v.Category));
    }
}

