namespace CarRental.Api.Services;

using CarRental.Api.Interfaces;

/// <summary>
/// Service for validating travel documents based on pickup location.
/// </summary>
public class DocumentValidationService : IDocumentValidationService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentValidationService"/> class.
    /// </summary>
    public DocumentValidationService()
    {
    }

    /// <summary>
    /// Validates that the document type is appropriate for the pickup location.
    /// </summary>
    /// <param name="documentType">The type of travel document (NationalId or Passport).</param>
    /// <param name="location">The pickup location.</param>
    /// <returns>True if the document is valid for the location; otherwise false.</returns>
    public bool IsDocumentValidForLocation(string documentType, string location)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Determines if a location requires international travel documents.
    /// </summary>
    /// <param name="location">The location to check.</param>
    /// <returns>True if the location is international; otherwise false.</returns>
    public bool IsInternationalLocation(string location)
    {
        throw new NotImplementedException();
    }
}
