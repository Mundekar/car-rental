namespace CarRental.Tests.Services;

using CarRental.Api.Common;
using CarRental.Api.DTOs;
using CarRental.Api.Interfaces;
using CarRental.Api.Models;
using CarRental.Api.Services;
using CarRental.Api.Strategies;
using CarRental.Api.Validators;
using Moq;
using Xunit;

/// <summary>
/// Unit tests for CarRentalService.
/// </summary>
public class CarRentalServiceTests
{
    private readonly Mock<ICarRentalProvider> _mockPremiumDriveProvider;
    private readonly Mock<ICarRentalProvider> _mockBudgetWheelsProvider;
    private readonly SearchRequestValidator _validator;
    private readonly PremiumDrivePricingStrategy _premiumDrivePricingStrategy;
    private readonly BudgetWheelsPricingStrategy _budgetWheelsPricingStrategy;
    private readonly CarRentalService _service;

    public CarRentalServiceTests()
    {
        _mockPremiumDriveProvider = new Mock<ICarRentalProvider>();
        _mockPremiumDriveProvider.Setup(p => p.ProviderName).Returns("PremiumDrive");

        _mockBudgetWheelsProvider = new Mock<ICarRentalProvider>();
        _mockBudgetWheelsProvider.Setup(p => p.ProviderName).Returns("BudgetWheels");

        _validator = new SearchRequestValidator();
        _premiumDrivePricingStrategy = new PremiumDrivePricingStrategy();
        _budgetWheelsPricingStrategy = new BudgetWheelsPricingStrategy();

        var providers = new[] { _mockPremiumDriveProvider.Object, _mockBudgetWheelsProvider.Object };

        _service = new CarRentalService(
            providers,
            _validator,
            _premiumDrivePricingStrategy,
            _budgetWheelsPricingStrategy);
    }

    [Fact]
    public async Task SearchCarsAsync_ReturnsAggregatedResults_FromBothProviders()
    {
        // Arrange
        var request = new SearchRequestDto
        {
            Pickup = "Mumbai",
            From = new DateTime(2026, 8, 1),
            To = new DateTime(2026, 8, 5),
            Category = null
        };

        var premiumVehicles = new List<ProviderVehicle>
        {
            new()
            {
                ProviderVehicleId = "PD-ECO-001",
                Category = VehicleCategory.Economy,
                Make = "Toyota",
                Model = "Corolla",
                Year = 2023,
                DailyRate = 45m,
                TotalPrice = 180m,
                InsuranceType = InsuranceType.Comprehensive,
                CancellationPolicy = CancellationPolicy.Free48Hours,
                IsAvailable = true
            }
        };

        var budgetVehicles = new List<ProviderVehicle>
        {
            new()
            {
                ProviderVehicleId = "BW-ECO-001",
                Category = VehicleCategory.Economy,
                Make = "Kia",
                Model = "Rio",
                Year = 2022,
                DailyRate = 35m,
                TotalPrice = 140m,
                InsuranceType = InsuranceType.Basic,
                CancellationPolicy = CancellationPolicy.NonRefundable,
                IsAvailable = true
            }
        };

        _mockPremiumDriveProvider
            .Setup(p => p.SearchAsync(It.IsAny<SearchRequestDto>()))
            .ReturnsAsync(premiumVehicles);

        _mockBudgetWheelsProvider
            .Setup(p => p.SearchAsync(It.IsAny<SearchRequestDto>()))
            .ReturnsAsync(budgetVehicles);

        // Act
        var result = await _service.SearchCarsAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Results.Count);
    }

    [Fact]
    public async Task SearchCarsAsync_FiltersBudgetWheelsUnavailableVehicles()
    {
        // Arrange
        var request = new SearchRequestDto
        {
            Pickup = "Mumbai",
            From = new DateTime(2026, 8, 1),
            To = new DateTime(2026, 8, 5),
            Category = null
        };

        var premiumVehicles = new List<ProviderVehicle>();

        var budgetVehicles = new List<ProviderVehicle>
        {
            new()
            {
                ProviderVehicleId = "BW-ECO-001",
                Category = VehicleCategory.Economy,
                Make = "Kia",
                Model = "Rio",
                Year = 2022,
                DailyRate = 35m,
                TotalPrice = 140m,
                InsuranceType = InsuranceType.Basic,
                CancellationPolicy = CancellationPolicy.NonRefundable,
                IsAvailable = true
            },
            new()
            {
                ProviderVehicleId = "BW-ECO-002",
                Category = VehicleCategory.Economy,
                Make = "Nissan",
                Model = "Versa",
                Year = 2022,
                DailyRate = 33m,
                TotalPrice = 132m,
                InsuranceType = InsuranceType.Basic,
                CancellationPolicy = CancellationPolicy.NonRefundable,
                IsAvailable = false,
                UnavailabilityReason = "Reserved"
            }
        };

        _mockPremiumDriveProvider
            .Setup(p => p.SearchAsync(It.IsAny<SearchRequestDto>()))
            .ReturnsAsync(premiumVehicles);

        _mockBudgetWheelsProvider
            .Setup(p => p.SearchAsync(It.IsAny<SearchRequestDto>()))
            .ReturnsAsync(budgetVehicles);

        // Act
        var result = await _service.SearchCarsAsync(request);

        // Assert
        Assert.Single(result.Results);
        Assert.Equal("Kia", result.Results[0].Make);
    }

    [Fact]
    public async Task SearchCarsAsync_CalculatesPricingUsingPremiumDriveStrategy()
    {
        // Arrange
        var request = new SearchRequestDto
        {
            Pickup = "Mumbai",
            From = new DateTime(2026, 8, 1),
            To = new DateTime(2026, 8, 6), // 5 days
            Category = null
        };

        var premiumVehicles = new List<ProviderVehicle>
        {
            new()
            {
                ProviderVehicleId = "PD-ECO-001",
                Category = VehicleCategory.Economy,
                Make = "Toyota",
                Model = "Corolla",
                Year = 2023,
                DailyRate = 45m,
                TotalPrice = 225m, // 45 × 5
                InsuranceType = InsuranceType.Comprehensive,
                CancellationPolicy = CancellationPolicy.Free48Hours,
                IsAvailable = true
            }
        };

        _mockPremiumDriveProvider
            .Setup(p => p.SearchAsync(It.IsAny<SearchRequestDto>()))
            .ReturnsAsync(premiumVehicles);

        _mockBudgetWheelsProvider
            .Setup(p => p.SearchAsync(It.IsAny<SearchRequestDto>()))
            .ReturnsAsync(new List<ProviderVehicle>());

        // Act
        var result = await _service.SearchCarsAsync(request);

        // Assert
        Assert.Single(result.Results);
        Assert.Equal(225m, result.Results[0].TotalPrice); // Flat rate: 45 × 5 = 225
    }

    [Fact]
    public async Task SearchCarsAsync_CalculatesPricingUsingBudgetWheelsStrategyWithWeekendSurcharge()
    {
        // Arrange
        // Aug 1-4, 2026: Saturday, Sunday, Monday, Tuesday (2 weekend nights: Sat, Sun)
        var request = new SearchRequestDto
        {
            Pickup = "Mumbai",
            From = new DateTime(2026, 8, 1), // Saturday
            To = new DateTime(2026, 8, 4),   // Tuesday (3 nights: Sat, Sun, Mon)
            Category = null
        };

        var budgetVehicles = new List<ProviderVehicle>
        {
            new()
            {
                ProviderVehicleId = "BW-ECO-001",
                Category = VehicleCategory.Economy,
                Make = "Kia",
                Model = "Rio",
                Year = 2022,
                DailyRate = 35m,
                TotalPrice = 0m, // Will be recalculated by service
                InsuranceType = InsuranceType.Basic,
                CancellationPolicy = CancellationPolicy.NonRefundable,
                IsAvailable = true
            }
        };

        _mockPremiumDriveProvider
            .Setup(p => p.SearchAsync(It.IsAny<SearchRequestDto>()))
            .ReturnsAsync(new List<ProviderVehicle>());

        _mockBudgetWheelsProvider
            .Setup(p => p.SearchAsync(It.IsAny<SearchRequestDto>()))
            .ReturnsAsync(budgetVehicles);

        // Act
        var result = await _service.SearchCarsAsync(request);

        // Assert
        Assert.Single(result.Results);
        // Sat: 35×1.2=42, Sun: 35×1.2=42, Mon: 35 = 119
        Assert.Equal(119m, result.Results[0].TotalPrice);
    }

    [Fact]
    public async Task SearchCarsAsync_SortsByTotalPriceAscending()
    {
        // Arrange
        var request = new SearchRequestDto
        {
            Pickup = "Mumbai",
            From = new DateTime(2026, 8, 1),
            To = new DateTime(2026, 8, 5),
            Category = null
        };

        var vehicles = new List<ProviderVehicle>
        {
            new()
            {
                ProviderVehicleId = "PD-SUV-001",
                Category = VehicleCategory.SUV,
                Make = "Toyota",
                Model = "CR-V",
                Year = 2023,
                DailyRate = 85m,
                TotalPrice = 340m,
                InsuranceType = InsuranceType.Comprehensive,
                CancellationPolicy = CancellationPolicy.Free48Hours,
                IsAvailable = true
            },
            new()
            {
                ProviderVehicleId = "PD-ECO-001",
                Category = VehicleCategory.Economy,
                Make = "Toyota",
                Model = "Corolla",
                Year = 2023,
                DailyRate = 45m,
                TotalPrice = 180m,
                InsuranceType = InsuranceType.Comprehensive,
                CancellationPolicy = CancellationPolicy.Free48Hours,
                IsAvailable = true
            }
        };

        _mockPremiumDriveProvider
            .Setup(p => p.SearchAsync(It.IsAny<SearchRequestDto>()))
            .ReturnsAsync(vehicles);

        _mockBudgetWheelsProvider
            .Setup(p => p.SearchAsync(It.IsAny<SearchRequestDto>()))
            .ReturnsAsync(new List<ProviderVehicle>());

        // Act
        var result = await _service.SearchCarsAsync(request);

        // Assert
        Assert.Equal(2, result.Results.Count);
        Assert.Equal(180m, result.Results[0].TotalPrice);
        Assert.Equal(340m, result.Results[1].TotalPrice);
    }

    [Fact]
    public async Task SearchCarsAsync_FiltersByCategory()
    {
        // Arrange
        var request = new SearchRequestDto
        {
            Pickup = "Mumbai",
            From = new DateTime(2026, 8, 1),
            To = new DateTime(2026, 8, 5),
            Category = "Economy"
        };

        var premiumVehicles = new List<ProviderVehicle>
        {
            new()
            {
                ProviderVehicleId = "PD-ECO-001",
                Category = VehicleCategory.Economy,
                Make = "Toyota",
                Model = "Corolla",
                Year = 2023,
                DailyRate = 45m,
                TotalPrice = 180m,
                InsuranceType = InsuranceType.Comprehensive,
                CancellationPolicy = CancellationPolicy.Free48Hours,
                IsAvailable = true
            }
        };

        _mockPremiumDriveProvider
            .Setup(p => p.SearchAsync(It.IsAny<SearchRequestDto>()))
            .ReturnsAsync(premiumVehicles);

        _mockBudgetWheelsProvider
            .Setup(p => p.SearchAsync(It.IsAny<SearchRequestDto>()))
            .ReturnsAsync(new List<ProviderVehicle>());

        // Act
        var result = await _service.SearchCarsAsync(request);

        // Assert
        Assert.NotEmpty(result.Results);
        Assert.All(result.Results, v => Assert.Equal(VehicleCategory.Economy, v.Category));
    }

    [Fact]
    public async Task SearchCarsAsync_ThrowsInvalidOperationException_WhenValidationFails()
    {
        // Arrange
        var request = new SearchRequestDto
        {
            Pickup = string.Empty, // Invalid: missing pickup
            From = new DateTime(2026, 8, 1),
            To = new DateTime(2026, 8, 5),
            Category = null
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.SearchCarsAsync(request));
    }

    [Fact]
    public async Task SearchCarsAsync_ThrowsArgumentNullException_WhenRequestIsNull()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.SearchCarsAsync(null!));
    }

    [Fact]
    public async Task SearchCarsAsync_ReturnsSearchId()
    {
        // Arrange
        var request = new SearchRequestDto
        {
            Pickup = "Mumbai",
            From = new DateTime(2026, 8, 1),
            To = new DateTime(2026, 8, 5),
            Category = null
        };

        _mockPremiumDriveProvider
            .Setup(p => p.SearchAsync(It.IsAny<SearchRequestDto>()))
            .ReturnsAsync(new List<ProviderVehicle>());

        _mockBudgetWheelsProvider
            .Setup(p => p.SearchAsync(It.IsAny<SearchRequestDto>()))
            .ReturnsAsync(new List<ProviderVehicle>());

        // Act
        var result = await _service.SearchCarsAsync(request);

        // Assert
        Assert.NotEqual(Guid.Empty, result.SearchId);
    }

    [Fact]
    public async Task SearchCarsAsync_PopulatesResponseDetails()
    {
        // Arrange
        var request = new SearchRequestDto
        {
            Pickup = "Dubai",
            From = new DateTime(2026, 8, 1),
            To = new DateTime(2026, 8, 6),
            Category = null
        };

        _mockPremiumDriveProvider
            .Setup(p => p.SearchAsync(It.IsAny<SearchRequestDto>()))
            .ReturnsAsync(new List<ProviderVehicle>());

        _mockBudgetWheelsProvider
            .Setup(p => p.SearchAsync(It.IsAny<SearchRequestDto>()))
            .ReturnsAsync(new List<ProviderVehicle>());

        // Act
        var result = await _service.SearchCarsAsync(request);

        // Assert
        Assert.Equal("Dubai", result.PickupLocation);
        Assert.Equal(new DateTime(2026, 8, 1), result.FromDate);
        Assert.Equal(new DateTime(2026, 8, 6), result.ToDate);
        Assert.Equal(5, result.DaysCount);
    }
}

