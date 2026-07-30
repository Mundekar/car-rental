namespace CarRental.Api.Common;

/// <summary>
/// Exception thrown when required booking fields are missing or malformed.
/// Results in HTTP 400 Bad Request.
/// </summary>
public class BookingValidationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BookingValidationException"/> class.
    /// </summary>
    /// <param name="message">The validation error message.</param>
    public BookingValidationException(string message) : base(message)
    {
    }
}
