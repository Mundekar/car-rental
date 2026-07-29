namespace CarRental.Tests.Services;

using Xunit;
using CarRental.Api.Services;

/// <summary>
/// Unit tests for DocumentValidationService.
/// </summary>
public class DocumentValidationServiceTests
{
    private readonly DocumentValidationService _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentValidationServiceTests"/> class.
    /// </summary>
    public DocumentValidationServiceTests()
    {
        _service = new DocumentValidationService();
    }

    [Fact]
    public void IsDocumentValidForLocation_AcceptsNationalIdForDomesticLocation()
    {
        // Arrange
        const string location = "Mumbai";
        const string documentType = "NationalId";

        // Act
        var result = _service.IsDocumentValidForLocation(documentType, location);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsDocumentValidForLocation_AcceptsPassportForDomesticLocation()
    {
        // Arrange
        const string location = "Bengaluru";
        const string documentType = "Passport";

        // Act
        var result = _service.IsDocumentValidForLocation(documentType, location);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsDocumentValidForLocation_AcceptsPassportForInternationalLocation()
    {
        // Arrange
        const string location = "Dubai";
        const string documentType = "Passport";

        // Act
        var result = _service.IsDocumentValidForLocation(documentType, location);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsDocumentValidForLocation_RejectsNationalIdForInternationalLocation()
    {
        // Arrange
        const string location = "Singapore";
        const string documentType = "NationalId";

        // Act
        var result = _service.IsDocumentValidForLocation(documentType, location);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsDocumentValidForLocation_RejectsNationalIdForLondon()
    {
        // Arrange
        const string location = "London";
        const string documentType = "NationalId";

        // Act
        var result = _service.IsDocumentValidForLocation(documentType, location);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsDocumentValidForLocation_ReturnsFalseForNullLocation()
    {
        // Arrange
        const string documentType = "Passport";

        // Act
        var result = _service.IsDocumentValidForLocation(documentType, null!);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsDocumentValidForLocation_ReturnsFalseForEmptyLocation()
    {
        // Arrange
        const string documentType = "Passport";

        // Act
        var result = _service.IsDocumentValidForLocation(documentType, string.Empty);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsDocumentValidForLocation_ReturnsFalseForNullDocumentType()
    {
        // Arrange
        const string location = "Mumbai";

        // Act
        var result = _service.IsDocumentValidForLocation(null!, location);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsInternationalLocation_IdentifiesDubaiAsInternational()
    {
        // Arrange
        const string location = "Dubai";

        // Act
        var result = _service.IsInternationalLocation(location);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsInternationalLocation_IdentifiesSingaporeAsInternational()
    {
        // Arrange
        const string location = "Singapore";

        // Act
        var result = _service.IsInternationalLocation(location);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsInternationalLocation_IdentifiesLondonAsInternational()
    {
        // Arrange
        const string location = "London";

        // Act
        var result = _service.IsInternationalLocation(location);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsInternationalLocation_IdentifiesMumbaiAsDomestic()
    {
        // Arrange
        const string location = "Mumbai";

        // Act
        var result = _service.IsInternationalLocation(location);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsInternationalLocation_IdentifiesBengaluruAsDomestic()
    {
        // Arrange
        const string location = "Bengaluru";

        // Act
        var result = _service.IsInternationalLocation(location);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsInternationalLocation_ReturnsFalseForNullLocation()
    {
        // Arrange & Act
        var result = _service.IsInternationalLocation(null!);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsInternationalLocation_ReturnsFalseForEmptyLocation()
    {
        // Arrange & Act
        var result = _service.IsInternationalLocation(string.Empty);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsDocumentValidForLocation_IsCaseInsensitive()
    {
        // Arrange
        const string location = "dubai"; // lowercase
        const string documentType = "passport"; // lowercase

        // Act
        var result = _service.IsDocumentValidForLocation(documentType, location);

        // Assert
        Assert.True(result);
    }
}
