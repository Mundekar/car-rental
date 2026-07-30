namespace CarRental.Tests.Validators;

using CarRental.Api.DTOs;
using CarRental.Api.Interfaces;
using CarRental.Api.Validators;
using Moq;
using Xunit;

/// <summary>
/// Unit tests for SearchRequestValidator.
/// </summary>
public class SearchRequestValidatorTests
{
    private readonly Mock<IDocumentValidationService> _docValidationMock;
    private readonly SearchRequestValidator _validator;

    public SearchRequestValidatorTests()
    {
        _docValidationMock = new Mock<IDocumentValidationService>();

        // By default treat any non-empty location as known so existing tests stay green.
        _docValidationMock
            .Setup(s => s.IsKnownLocation(It.IsAny<string>()))
            .Returns((string loc) => new[] { "Mumbai", "Bengaluru", "Dubai", "Singapore", "London" }
                .Contains(loc, StringComparer.OrdinalIgnoreCase));

        _validator = new SearchRequestValidator(_docValidationMock.Object);
    }

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

    [Fact]
    public void Validate_ReturnsError_WhenPickupIsUnrecognisedLocation()
    {
        // Arrange
        var request = new SearchRequestDto
        {
            Pickup = "Paris",
            From = new DateTime(2026, 8, 1),
            To = new DateTime(2026, 8, 5),
            Category = null
        };

        // Act
        var errors = _validator.Validate(request);

        // Assert
        Assert.NotEmpty(errors);
        Assert.Contains("'Paris' is not a recognised pickup location.", errors);
    }

    [Theory]
    [InlineData("Mumbai")]
    [InlineData("Bengaluru")]
    [InlineData("Dubai")]
    [InlineData("Singapore")]
    [InlineData("London")]
    public void Validate_ReturnsNoErrors_WhenPickupIsRecognisedLocation(string location)
    {
        // Arrange
        var request = new SearchRequestDto
        {
            Pickup = location,
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
    public void Validate_DoesNotReturnLocationError_WhenPickupIsMissing()
    {
        // Arrange – an empty pickup should produce "required" error, not "unrecognised" error
        var request = new SearchRequestDto
        {
            Pickup = string.Empty,
            From = new DateTime(2026, 8, 1),
            To = new DateTime(2026, 8, 5),
            Category = null
        };

        // Act
        var errors = _validator.Validate(request).ToList();

        // Assert
        Assert.Contains("Pickup location is required.", errors);
        Assert.DoesNotContain(errors, e => e.Contains("is not a recognised pickup location"));
    }
}

