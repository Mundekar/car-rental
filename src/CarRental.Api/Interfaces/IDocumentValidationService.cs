namespace CarRental.Api.Interfaces;

/// <summary>
/// Defines the contract for travel document validation based on location.
/// </summary>
public interface IDocumentValidationService
{
    /// <summary>
    /// Validates that the document type is appropriate for the pickup location.
    /// </summary>
    /// <param name="documentType">The type of travel document (NationalId or Passport).</param>
    /// <param name="location">The pickup location.</param>
    /// <returns>True if the document is valid for the location; otherwise false.</returns>
    bool IsDocumentValidForLocation(string documentType, string location);

    /// <summary>
    /// Determines if a location requires international travel documents.
    /// </summary>
    /// <param name="location">The location to check.</param>
    /// <returns>True if the location is international; otherwise false.</returns>
    bool IsInternationalLocation(string location);

    /// <summary>
    /// Determines if a location is a recognised pickup location.
    /// </summary>
    /// <param name="location">The location to check.</param>
    /// <returns>True if the location is in the defined city list; otherwise false.</returns>
    bool IsKnownLocation(string location);
}
