namespace CarRental.Tests.Providers;

using CarRental.Api.Common;
using CarRental.Api.DTOs;
using CarRental.Api.Providers;
using Xunit;

/// <summary>
/// Unit tests for the BudgetWheelsProvider.
/// </summary>
public class BudgetWheelsProviderTests
{
    private readonly BudgetWheelsProvider _provider = new();

    [Fact]
    public void ProviderName_ReturnsCorrectValue()
    {
        // Arrange & Act
        var name = _provider.ProviderName;

        // Assert
        Assert.Equal("BudgetWheels", name);
    }

    [Fact]
    public async Task SearchAsync_ReturnsDeterministicVehicles()
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
        var result1 = await _provider.SearchAsync(request);
        var result2 = await _provider.SearchAsync(request);

        // Assert
        var vehicles1 = result1.ToList();
        var vehicles2 = result2.ToList();
        Assert.Equal(vehicles1.Count, vehicles2.Count);
        Assert.Equal(8, vehicles1.Count); // 2 vehicles per category × 4 categories
    }

    [Fact]
    public async Task SearchAsync_IncludesUnavailableVehicles()
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
        var unavailableVehicles = vehicles.Where(v => !v.IsAvailable).ToList();
        Assert.NotEmpty(unavailableVehicles);
        Assert.True(unavailableVehicles.Count > 0, "BudgetWheels should include unavailable vehicles");
    }

    [Fact]
    public async Task SearchAsync_IncludesAvailableVehicles()
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
        var availableVehicles = vehicles.Where(v => v.IsAvailable).ToList();
        Assert.NotEmpty(availableVehicles);
        Assert.True(availableVehicles.Count > 0, "BudgetWheels should include available vehicles");
    }

    [Fact]
    public async Task SearchAsync_AllVehiclesHaveBasicInsurance()
    {
        // Arrange
        var request = new SearchRequestDto
        {
            Pickup = "London",
            From = new DateTime(2026, 8, 1),
            To = new DateTime(2026, 8, 3),
            Category = null
        };

        // Act
        var result = await _provider.SearchAsync(request);

        // Assert
        var vehicles = result.ToList();
        Assert.NotEmpty(vehicles);
        Assert.All(vehicles, v => Assert.Equal(InsuranceType.Basic, v.InsuranceType));
    }

    [Fact]
    public async Task SearchAsync_AllVehiclesHaveNonRefundableCancellation()
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
        Assert.All(vehicles, v => Assert.Equal(CancellationPolicy.NonRefundable, v.CancellationPolicy));
    }

    [Fact]
    public async Task SearchAsync_ReturnsFilteredByCategory_WhenCategoryProvided()
    {
        // Arrange
        var request = new SearchRequestDto
        {
            Pickup = "Mumbai",
            From = new DateTime(2026, 8, 1),
            To = new DateTime(2026, 8, 5),
            Category = "SUV"
        };

        // Act
        var result = await _provider.SearchAsync(request);

        // Assert
        var vehicles = result.ToList();
        Assert.NotEmpty(vehicles);
        Assert.All(vehicles, v => Assert.Equal(VehicleCategory.SUV, v.Category));
    }

    [Fact]
    public async Task SearchAsync_CalculatesPricingWithWeekendSurcharge()
    {
        // Arrange
        // Aug 1-3, 2026: Friday, Saturday, Sunday (all weekend nights)
        var request = new SearchRequestDto
        {
            Pickup = "Dubai",
            From = new DateTime(2026, 8, 1), // Friday
            To = new DateTime(2026, 8, 4),   // Monday (3 nights: Fri, Sat, Sun)
            Category = "Economy"
        };

        // Act
        var result = await _provider.SearchAsync(request);

        // Assert
        var vehicles = result.ToList();
        Assert.NotEmpty(vehicles);
        foreach (var vehicle in vehicles)
        {
            // All 3 nights are weekend nights, so total = baseRate * 1.2 * 3
            var expectedTotal = vehicle.DailyRate * 1.2m * 3;
            Assert.Equal(expectedTotal, vehicle.TotalPrice);
        }
    }

    [Fact]
    public async Task SearchAsync_UnavailableVehiclesHaveReason()
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
        var unavailableVehicles = vehicles.Where(v => !v.IsAvailable).ToList();
        Assert.NotEmpty(unavailableVehicles);
        Assert.All(unavailableVehicles, v => Assert.False(string.IsNullOrWhiteSpace(v.UnavailabilityReason)));
    }

    [Fact]
    public async Task SearchAsync_AvailableVehiclesHaveNoUnavailabilityReason()
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
        var availableVehicles = vehicles.Where(v => v.IsAvailable).ToList();
        Assert.NotEmpty(availableVehicles);
        Assert.All(availableVehicles, v => Assert.Null(v.UnavailabilityReason));
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
            Pickup = "London",
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
            Pickup = "Singapore",
            From = new DateTime(2026, 8, 1),
            To = new DateTime(2026, 8, 5),
            Category = "minivan" // lowercase
        };

        // Act
        var result = await _provider.SearchAsync(request);

        // Assert
        var vehicles = result.ToList();
        Assert.NotEmpty(vehicles);
        Assert.All(vehicles, v => Assert.Equal(VehicleCategory.Minivan, v.Category));
    }

    [Fact]
    public async Task SearchAsync_CalculatesWeekdayPricing_WithoutSurcharge()
    {
        // Arrange
        // Aug 4-8, 2026: Monday-Friday (no weekend nights)
        var request = new SearchRequestDto
        {
            Pickup = "Mumbai",
            From = new DateTime(2026, 8, 4),  // Monday
            To = new DateTime(2026, 8, 9),   // Saturday (5 nights: Mon-Fri)
            Category = "Compact"
        };

        // Act
        var result = await _provider.SearchAsync(request);

        // Assert
        var vehicles = result.ToList();
        Assert.NotEmpty(vehicles);
        foreach (var vehicle in vehicles)
        {
            // All 5 nights are weekdays, so total = baseRate * 5
            var expectedTotal = vehicle.DailyRate * 5;
            Assert.Equal(expectedTotal, vehicle.TotalPrice);
        }
    }
}

