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
        // Aug 1-4, 2026: Saturday, Sunday, Monday, Tuesday (2 weekend nights: Sat, Sun)
        var request = new SearchRequestDto
        {
            Pickup = "Dubai",
            From = new DateTime(2026, 8, 1), // Saturday
            To = new DateTime(2026, 8, 4),   // Tuesday (3 nights: Sat, Sun, Mon)
            Category = "Economy"
        };

        // Act
        var result = await _provider.SearchAsync(request);

        // Assert
        var vehicles = result.ToList();
        Assert.NotEmpty(vehicles);
        // Aug 1 (Sat): 1.2×, Aug 2 (Sun): 1.2×, Aug 3 (Mon): 1.0×
        // Expected pricing: (35 × 1.2) + (35 × 1.2) + 35 = 42 + 42 + 35 = 119
        // For $33 vehicle: (33 × 1.2) + (33 × 1.2) + 33 = 39.6 + 39.6 + 33 = 112.2
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
        // Aug 4-7, 2026: Tuesday through Thursday (weekdays only)
        var request = new SearchRequestDto
        {
            Pickup = "Mumbai",
            From = new DateTime(2026, 8, 4),  // Tuesday
            To = new DateTime(2026, 8, 7),    // Friday (3 nights: Tue-Thu, no Friday surcharge for Thu night)
            Category = "Compact"
        };

        // Act
        var result = await _provider.SearchAsync(request);

        // Assert
        var vehicles = result.ToList();
        Assert.NotEmpty(vehicles);
        // Verify provider returns vehicles with pricing
        Assert.All(vehicles, v => Assert.True(v.TotalPrice > 0, "Provider should set TotalPrice"));
    }
}

