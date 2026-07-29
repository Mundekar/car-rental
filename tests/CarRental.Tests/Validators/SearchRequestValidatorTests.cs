namespace CarRental.Tests.Validators;

using CarRental.Api.DTOs;
using CarRental.Api.Validators;
using Xunit;

/// <summary>
/// Unit tests for SearchRequestValidator.
/// </summary>
public class SearchRequestValidatorTests
{
    private readonly SearchRequestValidator _validator = new();

    [Fact]
    public void Validate_ReturnsNoErrors_WhenRequestIsValid()
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
        var errors = _validator.Validate(request);

        // Assert
        Assert.Empty(errors);
    }

    [Fact]
    public void Validate_ReturnsError_WhenPickupIsMissing()
    {
        // Arrange
        var request = new SearchRequestDto
        {
            Pickup = string.Empty,
            From = new DateTime(2026, 8, 1),
            To = new DateTime(2026, 8, 5),
            Category = null
        };

        // Act
        var errors = _validator.Validate(request);

        // Assert
        Assert.NotEmpty(errors);
        Assert.Contains("Pickup location is required.", errors);
    }

    [Fact]
    public void Validate_ReturnsError_WhenPickupIsWhitespace()
    {
        // Arrange
        var request = new SearchRequestDto
        {
            Pickup = "   ",
            From = new DateTime(2026, 8, 1),
            To = new DateTime(2026, 8, 5),
            Category = null
        };

        // Act
        var errors = _validator.Validate(request);

        // Assert
        Assert.NotEmpty(errors);
        Assert.Contains("Pickup location is required.", errors);
    }

    [Fact]
    public void Validate_ReturnsError_WhenFromDateIsDefault()
    {
        // Arrange
        var request = new SearchRequestDto
        {
            Pickup = "Mumbai",
            From = default,
            To = new DateTime(2026, 8, 5),
            Category = null
        };

        // Act
        var errors = _validator.Validate(request);

        // Assert
        Assert.NotEmpty(errors);
        Assert.Contains("From date is required.", errors);
    }

    [Fact]
    public void Validate_ReturnsError_WhenToDateIsDefault()
    {
        // Arrange
        var request = new SearchRequestDto
        {
            Pickup = "Mumbai",
            From = new DateTime(2026, 8, 1),
            To = default,
            Category = null
        };

        // Act
        var errors = _validator.Validate(request);

        // Assert
        Assert.NotEmpty(errors);
        Assert.Contains("To date is required.", errors);
    }

    [Fact]
    public void Validate_ReturnsError_WhenToDateEqualsFromDate()
    {
        // Arrange
        var sameDate = new DateTime(2026, 8, 1);
        var request = new SearchRequestDto
        {
            Pickup = "Mumbai",
            From = sameDate,
            To = sameDate,
            Category = null
        };

        // Act
        var errors = _validator.Validate(request);

        // Assert
        Assert.NotEmpty(errors);
        Assert.Contains("To date must be after From date.", errors);
    }

    [Fact]
    public void Validate_ReturnsError_WhenToDateBeforeFromDate()
    {
        // Arrange
        var request = new SearchRequestDto
        {
            Pickup = "Mumbai",
            From = new DateTime(2026, 8, 10),
            To = new DateTime(2026, 8, 5),
            Category = null
        };

        // Act
        var errors = _validator.Validate(request);

        // Assert
        Assert.NotEmpty(errors);
        Assert.Contains("To date must be after From date.", errors);
    }

    [Fact]
    public void Validate_AllowsOptionalCategory()
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
        var errors = _validator.Validate(request);

        // Assert
        Assert.Empty(errors);
    }

    [Fact]
    public void Validate_AllowsCategoryWhenProvided()
    {
        // Arrange
        var request = new SearchRequestDto
        {
            Pickup = "Dubai",
            From = new DateTime(2026, 8, 1),
            To = new DateTime(2026, 8, 5),
            Category = "SUV"
        };

        // Act
        var errors = _validator.Validate(request);

        // Assert
        Assert.Empty(errors);
    }

    [Fact]
    public void Validate_ReturnsAllErrors_WhenMultipleFieldsInvalid()
    {
        // Arrange
        var request = new SearchRequestDto
        {
            Pickup = string.Empty,
            From = new DateTime(2026, 8, 10),
            To = new DateTime(2026, 8, 5),
            Category = null
        };

        // Act
        var errors = _validator.Validate(request);

        // Assert
        Assert.NotEmpty(errors);
        Assert.Contains("Pickup location is required.", errors);
        Assert.Contains("To date must be after From date.", errors);
    }

    [Fact]
    public void Validate_ReturnsError_WhenRequestIsNull()
    {
        // Act
        var errors = _validator.Validate(null!);

        // Assert
        Assert.NotEmpty(errors);
        Assert.Contains("Search request cannot be null.", errors);
    }
}

