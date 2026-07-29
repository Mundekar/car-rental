namespace CarRental.Api.Validators;

using CarRental.Api.DTOs;

/// <summary>
/// Validator for booking request input validation.
/// </summary>
public class BookingRequestValidator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BookingRequestValidator"/> class.
    /// </summary>
    public BookingRequestValidator()
    {
    }

    /// <summary>
    /// Validates the booking request.
    /// </summary>
    /// <param name="request">The booking request to validate.</param>
    /// <returns>A collection of validation error messages, empty if valid.</returns>
    public IEnumerable<string> Validate(BookingRequestDto request)
    {
        throw new NotImplementedException();
    }
}
