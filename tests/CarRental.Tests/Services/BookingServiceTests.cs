namespace CarRental.Tests.Services;

using Xunit;
using Moq;
using CarRental.Api.Services;
using CarRental.Api.Interfaces;
using CarRental.Api.DTOs;
using CarRental.Api.Common;

/// <summary>
/// Unit tests for BookingService.
/// </summary>
public class BookingServiceTests
{
    private readonly Mock<IDocumentValidationService> _mockDocumentValidationService;
    private readonly BookingService _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="BookingServiceTests"/> class.
    /// </summary>
    public BookingServiceTests()
    {
        _mockDocumentValidationService = new Mock<IDocumentValidationService>();
        _service = new BookingService(_mockDocumentValidationService.Object);
    }

    [Fact]
    public async Task CreateBookingAsync_SucceedsWithValidDomesticBookingAndNationalId()
    {
        // Arrange
        var request = new BookingRequestDto
        {
            DriverName = "John Doe",
            DocumentType = DocumentType.NationalId,
            DocumentNumber = "12345678",
            VehicleId = Guid.NewGuid(),
            Provider = "PremiumDrive",
            PickupLocation = "Mumbai",
            PickupDate = DateTime.UtcNow.AddDays(1),
            ReturnDate = DateTime.UtcNow.AddDays(3)
        };

        _mockDocumentValidationService
            .Setup(s => s.IsDocumentValidForLocation("NationalId", "Mumbai"))
            .Returns(true);

        // Act
        var response = await _service.CreateBookingAsync(request);

        // Assert
        Assert.NotNull(response);
        Assert.NotEmpty(response.ReferenceNumber);
        Assert.True(response.ReferenceNumber.StartsWith("CR-"));
        Assert.Equal("John Doe", response.DriverName);
        Assert.Equal("PremiumDrive", response.Provider);
    }

    [Fact]
    public async Task CreateBookingAsync_SucceedsWithValidDomesticBookingAndPassport()
    {
        // Arrange
        var request = new BookingRequestDto
        {
            DriverName = "Jane Smith",
            DocumentType = DocumentType.Passport,
            DocumentNumber = "A12345678",
            VehicleId = Guid.NewGuid(),
            Provider = "BudgetWheels",
            PickupLocation = "Bengaluru",
            PickupDate = DateTime.UtcNow.AddDays(2),
            ReturnDate = DateTime.UtcNow.AddDays(5)
        };

        _mockDocumentValidationService
            .Setup(s => s.IsDocumentValidForLocation("Passport", "Bengaluru"))
            .Returns(true);

        // Act
        var response = await _service.CreateBookingAsync(request);

        // Assert
        Assert.NotNull(response);
        Assert.NotEmpty(response.ReferenceNumber);
        Assert.Equal("Jane Smith", response.DriverName);
    }

    [Fact]
    public async Task CreateBookingAsync_SucceedsWithValidInternationalBookingAndPassport()
    {
        // Arrange
        var request = new BookingRequestDto
        {
            DriverName = "Bob Johnson",
            DocumentType = DocumentType.Passport,
            DocumentNumber = "B87654321",
            VehicleId = Guid.NewGuid(),
            Provider = "PremiumDrive",
            PickupLocation = "Dubai",
            PickupDate = DateTime.UtcNow.AddDays(1),
            ReturnDate = DateTime.UtcNow.AddDays(4)
        };

        _mockDocumentValidationService
            .Setup(s => s.IsDocumentValidForLocation("Passport", "Dubai"))
            .Returns(true);

        // Act
        var response = await _service.CreateBookingAsync(request);

        // Assert
        Assert.NotNull(response);
        Assert.NotEmpty(response.ReferenceNumber);
        Assert.Equal("Bob Johnson", response.DriverName);
    }

    [Fact]
    public async Task CreateBookingAsync_FailsWithInternationalLocationAndNationalId()
    {
        // Arrange
        var request = new BookingRequestDto
        {
            DriverName = "Alice Wong",
            DocumentType = DocumentType.NationalId,
            DocumentNumber = "ID123456",
            VehicleId = Guid.NewGuid(),
            Provider = "BudgetWheels",
            PickupLocation = "Singapore",
            PickupDate = DateTime.UtcNow.AddDays(1),
            ReturnDate = DateTime.UtcNow.AddDays(3)
        };

        _mockDocumentValidationService
            .Setup(s => s.IsDocumentValidForLocation("NationalId", "Singapore"))
            .Returns(false);
        _mockDocumentValidationService
            .Setup(s => s.IsInternationalLocation("Singapore"))
            .Returns(true);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateBookingAsync(request));
        Assert.Contains("Passport is required for international pickup locations", ex.Message);
    }

    [Fact]
    public async Task CreateBookingAsync_GeneratesUniqueBookingReferences()
    {
        // Arrange
        var request1 = new BookingRequestDto
        {
            DriverName = "Driver One",
            DocumentType = DocumentType.Passport,
            DocumentNumber = "P111111",
            VehicleId = Guid.NewGuid(),
            Provider = "PremiumDrive",
            PickupLocation = "Mumbai",
            PickupDate = DateTime.UtcNow.AddDays(1),
            ReturnDate = DateTime.UtcNow.AddDays(2)
        };

        var request2 = new BookingRequestDto
        {
            DriverName = "Driver Two",
            DocumentType = DocumentType.Passport,
            DocumentNumber = "P222222",
            VehicleId = Guid.NewGuid(),
            Provider = "BudgetWheels",
            PickupLocation = "Mumbai",
            PickupDate = DateTime.UtcNow.AddDays(3),
            ReturnDate = DateTime.UtcNow.AddDays(4)
        };

        _mockDocumentValidationService
            .Setup(s => s.IsDocumentValidForLocation(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(true);

        // Act
        var response1 = await _service.CreateBookingAsync(request1);
        var response2 = await _service.CreateBookingAsync(request2);

        // Assert
        Assert.NotEqual(response1.ReferenceNumber, response2.ReferenceNumber);
        Assert.True(response1.ReferenceNumber.StartsWith("CR-"));
        Assert.True(response2.ReferenceNumber.StartsWith("CR-"));
    }

    [Fact]
    public async Task GetBookingByReferenceAsync_ReturnsBookingWhenExists()
    {
        // Arrange
        var request = new BookingRequestDto
        {
            DriverName = "Test Driver",
            DocumentType = DocumentType.Passport,
            DocumentNumber = "TEST123",
            VehicleId = Guid.NewGuid(),
            Provider = "PremiumDrive",
            PickupLocation = "Mumbai",
            PickupDate = DateTime.UtcNow.AddDays(1),
            ReturnDate = DateTime.UtcNow.AddDays(2)
        };

        _mockDocumentValidationService
            .Setup(s => s.IsDocumentValidForLocation(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(true);

        var bookingResponse = await _service.CreateBookingAsync(request);

        // Act
        var retrievedBooking = await _service.GetBookingByReferenceAsync(bookingResponse.ReferenceNumber);

        // Assert
        Assert.NotNull(retrievedBooking);
        Assert.Equal(bookingResponse.ReferenceNumber, retrievedBooking.ReferenceNumber);
        Assert.Equal(bookingResponse.DriverName, retrievedBooking.DriverName);
    }

    [Fact]
    public async Task GetBookingByReferenceAsync_ReturnsNullWhenBookingDoesNotExist()
    {
        // Arrange & Act
        var result = await _service.GetBookingByReferenceAsync("CR-99999999-999999");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetBookingByReferenceAsync_ReturnsNullForNullReference()
    {
        // Arrange & Act
        var result = await _service.GetBookingByReferenceAsync(null!);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetBookingByReferenceAsync_ReturnsNullForEmptyReference()
    {
        // Arrange & Act
        var result = await _service.GetBookingByReferenceAsync(string.Empty);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateBookingAsync_FailsWhenDriverNameIsEmpty()
    {
        // Arrange
        var request = new BookingRequestDto
        {
            DriverName = string.Empty,
            DocumentType = DocumentType.Passport,
            DocumentNumber = "TEST123",
            VehicleId = Guid.NewGuid(),
            Provider = "PremiumDrive",
            PickupLocation = "Mumbai",
            PickupDate = DateTime.UtcNow.AddDays(1),
            ReturnDate = DateTime.UtcNow.AddDays(2)
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<BookingValidationException>(() => _service.CreateBookingAsync(request));
        Assert.Contains("Driver name is required", ex.Message);
    }

    [Fact]
    public async Task CreateBookingAsync_FailsWhenDocumentNumberIsEmpty()
    {
        // Arrange
        var request = new BookingRequestDto
        {
            DriverName = "John Doe",
            DocumentType = DocumentType.Passport,
            DocumentNumber = string.Empty,
            VehicleId = Guid.NewGuid(),
            Provider = "PremiumDrive",
            PickupLocation = "Mumbai",
            PickupDate = DateTime.UtcNow.AddDays(1),
            ReturnDate = DateTime.UtcNow.AddDays(2)
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<BookingValidationException>(() => _service.CreateBookingAsync(request));
        Assert.Contains("Document number is required", ex.Message);
    }

    [Fact]
    public async Task CreateBookingAsync_FailsWhenVehicleIdIsEmpty()
    {
        // Arrange
        var request = new BookingRequestDto
        {
            DriverName = "John Doe",
            DocumentType = DocumentType.Passport,
            DocumentNumber = "TEST123",
            VehicleId = Guid.Empty,
            Provider = "PremiumDrive",
            PickupLocation = "Mumbai",
            PickupDate = DateTime.UtcNow.AddDays(1),
            ReturnDate = DateTime.UtcNow.AddDays(2)
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<BookingValidationException>(() => _service.CreateBookingAsync(request));
        Assert.Contains("Vehicle ID is required", ex.Message);
    }

    [Fact]
    public async Task CreateBookingAsync_FailsWhenPickupLocationIsEmpty()
    {
        // Arrange
        var request = new BookingRequestDto
        {
            DriverName = "John Doe",
            DocumentType = DocumentType.Passport,
            DocumentNumber = "TEST123",
            VehicleId = Guid.NewGuid(),
            Provider = "PremiumDrive",
            PickupLocation = string.Empty,
            PickupDate = DateTime.UtcNow.AddDays(1),
            ReturnDate = DateTime.UtcNow.AddDays(2)
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<BookingValidationException>(() => _service.CreateBookingAsync(request));
        Assert.Contains("Pickup location is required", ex.Message);
    }

    [Fact]
    public async Task CreateBookingAsync_FailsWhenReturnDateIsBeforePickupDate()
    {
        // Arrange
        var request = new BookingRequestDto
        {
            DriverName = "John Doe",
            DocumentType = DocumentType.Passport,
            DocumentNumber = "TEST123",
            VehicleId = Guid.NewGuid(),
            Provider = "PremiumDrive",
            PickupLocation = "Mumbai",
            PickupDate = DateTime.UtcNow.AddDays(3),
            ReturnDate = DateTime.UtcNow.AddDays(1)
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<BookingValidationException>(() => _service.CreateBookingAsync(request));
        Assert.Contains("Return date must be after pickup date", ex.Message);
    }

    [Fact]
    public async Task CreateBookingAsync_FailsWhenRequestIsNull()
    {
        // Arrange & Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateBookingAsync(null!));
    }

    [Fact]
    public async Task GetAllBookingsAsync_ReturnsAllCreatedBookings()
    {
        // Arrange
        var request1 = new BookingRequestDto
        {
            DriverName = "Driver One",
            DocumentType = DocumentType.Passport,
            DocumentNumber = "P111111",
            VehicleId = Guid.NewGuid(),
            Provider = "PremiumDrive",
            PickupLocation = "Mumbai",
            PickupDate = DateTime.UtcNow.AddDays(1),
            ReturnDate = DateTime.UtcNow.AddDays(2)
        };

        var request2 = new BookingRequestDto
        {
            DriverName = "Driver Two",
            DocumentType = DocumentType.NationalId,
            DocumentNumber = "ID111111",
            VehicleId = Guid.NewGuid(),
            Provider = "BudgetWheels",
            PickupLocation = "Bengaluru",
            PickupDate = DateTime.UtcNow.AddDays(3),
            ReturnDate = DateTime.UtcNow.AddDays(4)
        };

        _mockDocumentValidationService
            .Setup(s => s.IsDocumentValidForLocation(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(true);

        // Act
        await _service.CreateBookingAsync(request1);
        await _service.CreateBookingAsync(request2);
        var allBookings = await _service.GetAllBookingsAsync();

        // Assert
        Assert.NotNull(allBookings);
        Assert.NotEmpty(allBookings);
    }
}
