namespace CarRental.Tests.Strategies;

using CarRental.Api.Strategies;
using Xunit;

/// <summary>
/// Unit tests for PremiumDrivePricingStrategy.
/// </summary>
public class PremiumDrivePricingStrategyTests
{
    private readonly PremiumDrivePricingStrategy _strategy = new();

    [Fact]
    public void CalculateTotalPrice_ReturnsCorrectFlatRate()
    {
        // Arrange
        var dailyRate = 50m;
        var from = new DateOnly(2026, 8, 1);
        var to = new DateOnly(2026, 8, 6); // 5 nights

        // Act
        var total = _strategy.CalculateTotalPrice(dailyRate, from, to);

        // Assert
        Assert.Equal(250m, total); // 50 × 5 = 250
    }

    [Fact]
    public void CalculateTotalPrice_HandlesSingleDay()
    {
        // Arrange
        var dailyRate = 45m;
        var from = new DateOnly(2026, 8, 1);
        var to = new DateOnly(2026, 8, 2); // 1 night

        // Act
        var total = _strategy.CalculateTotalPrice(dailyRate, from, to);

        // Assert
        Assert.Equal(45m, total); // 45 × 1 = 45
    }

    [Fact]
    public void CalculateTotalPrice_CalculatesForOneWeek()
    {
        // Arrange
        var dailyRate = 30m;
        var from = new DateOnly(2026, 8, 1);
        var to = new DateOnly(2026, 8, 8); // 7 nights

        // Act
        var total = _strategy.CalculateTotalPrice(dailyRate, from, to);

        // Assert
        Assert.Equal(210m, total); // 30 × 7 = 210
    }

    [Fact]
    public void CalculateTotalPrice_HandlesSameDateAs1Night()
    {
        // Arrange
        var dailyRate = 60m;
        var from = new DateOnly(2026, 8, 5);
        var to = new DateOnly(2026, 8, 5); // Same date

        // Act
        var total = _strategy.CalculateTotalPrice(dailyRate, from, to);

        // Assert
        Assert.Equal(60m, total); // Treat as 1 night
    }

    [Fact]
    public void CalculateTotalPrice_WorksWithDecimalDailyRates()
    {
        // Arrange
        var dailyRate = 42.50m;
        var from = new DateOnly(2026, 8, 1);
        var to = new DateOnly(2026, 8, 4); // 3 nights

        // Act
        var total = _strategy.CalculateTotalPrice(dailyRate, from, to);

        // Assert
        Assert.Equal(127.50m, total); // 42.50 × 3 = 127.50
    }
}

/// <summary>
/// Unit tests for BudgetWheelsPricingStrategy.
/// </summary>
public class BudgetWheelsPricingStrategyTests
{
    private readonly BudgetWheelsPricingStrategy _strategy = new();

    [Fact]
    public void CalculateTotalPrice_AppliesWeekendSurcharge()
    {
        // Arrange
        // Aug 1-4, 2026: Saturday, Sunday, Monday, Tuesday (2 weekend nights: Sat, Sun)
        var dailyRate = 35m;
        var from = new DateOnly(2026, 8, 1);  // Saturday
        var to = new DateOnly(2026, 8, 4);    // Tuesday (3 nights: Sat, Sun, Mon)

        // Act
        var total = _strategy.CalculateTotalPrice(dailyRate, from, to);

        // Assert
        // Sat: 35 × 1.2 = 42, Sun: 35 × 1.2 = 42, Mon: 35 = 35
        // Total = 119
        Assert.Equal(119m, total);
    }

    [Fact]
    public void CalculateTotalPrice_WeekdaysOnly_NoSurcharge()
    {
        // Arrange
        // Aug 4-7, 2026: Tuesday through Thursday (no weekend nights)
        var dailyRate = 35m;
        var from = new DateOnly(2026, 8, 4);  // Tuesday
        var to = new DateOnly(2026, 8, 7);    // Friday (3 nights: Tue-Thu, no Friday surcharge on Thu night)

        // Act
        var total = _strategy.CalculateTotalPrice(dailyRate, from, to);

        // Assert
        // Tue-Thu all weekdays: 35 × 3 = 105
        Assert.Equal(105m, total);
    }

    [Fact]
    public void CalculateTotalPrice_MixedWeekdaysAndWeekends()
    {
        // Arrange
        // Aug 7-10, 2026: Friday through Monday (3 nights: Fri, Sat, Sun all weekend)
        var dailyRate = 40m;
        var from = new DateOnly(2026, 8, 7);  // Friday
        var to = new DateOnly(2026, 8, 10);   // Monday (3 nights: Fri, Sat, Sun)

        // Act
        var total = _strategy.CalculateTotalPrice(dailyRate, from, to);

        // Assert
        // Fri: 40 × 1.2 = 48, Sat: 40 × 1.2 = 48, Sun: 40 × 1.2 = 48
        // Total = 48 + 48 + 48 = 144
        Assert.Equal(144m, total);
    }

    [Fact]
    public void CalculateTotalPrice_SingleWeekendDay()
    {
        // Arrange
        var dailyRate = 50m;
        var from = new DateOnly(2026, 8, 1);  // Friday
        var to = new DateOnly(2026, 8, 2);    // Saturday (1 night: Friday)

        // Act
        var total = _strategy.CalculateTotalPrice(dailyRate, from, to);

        // Assert
        // Fri: 50 × 1.2 = 60
        Assert.Equal(60m, total);
    }

    [Fact]
    public void CalculateTotalPrice_SundayToMonday()
    {
        // Arrange
        var dailyRate = 45m;
        var from = new DateOnly(2026, 8, 2);  // Sunday
        var to = new DateOnly(2026, 8, 3);    // Monday (1 night: Sunday)

        // Act
        var total = _strategy.CalculateTotalPrice(dailyRate, from, to);

        // Assert
        // Sun: 45 × 1.2 = 54
        Assert.Equal(54m, total);
    }

    [Fact]
    public void CalculateTotalPrice_TwoWeekRental_MultipleSurcharges()
    {
        // Arrange
        // Aug 1-15, 2026: Two full weeks
        var dailyRate = 30m;
        var from = new DateOnly(2026, 8, 1);   // Friday
        var to = new DateOnly(2026, 8, 15);    // Saturday (14 nights)

        // Act
        var total = _strategy.CalculateTotalPrice(dailyRate, from, to);

        // Assert
        // Week 1: Fri(36), Sat(36), Sun(36), Mon(30), Tue(30), Wed(30), Thu(30) = 228
        // Week 2: Fri(36), Sat(36), Sun(36), Mon(30), Tue(30), Wed(30), Thu(30) = 228
        // Total = 456
        Assert.Equal(456m, total);
    }

    [Fact]
    public void CalculateTotalPrice_HandlesSameDateAs1Night()
    {
        // Arrange
        var dailyRate = 40m;
        var from = new DateOnly(2026, 8, 1);  // Friday
        var to = new DateOnly(2026, 8, 1);    // Same date

        // Act
        var total = _strategy.CalculateTotalPrice(dailyRate, from, to);

        // Assert
        Assert.Equal(0m, total); // No days between same dates
    }

    [Fact]
    public void CalculateTotalPrice_WorksWithDecimalRates()
    {
        // Arrange
        var dailyRate = 33.50m;
        var from = new DateOnly(2026, 8, 1);  // Friday
        var to = new DateOnly(2026, 8, 2);    // Saturday (1 night: Friday)

        // Act
        var total = _strategy.CalculateTotalPrice(dailyRate, from, to);

        // Assert
        // Fri: 33.50 × 1.2 = 40.20
        Assert.Equal(40.20m, total);
    }
}
