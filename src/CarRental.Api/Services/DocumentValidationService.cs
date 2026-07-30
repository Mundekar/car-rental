namespace CarRental.Api.Services;

using CarRental.Api.Interfaces;

/// <summary>
/// Service for validating travel documents based on pickup location.
/// Implements business logic to enforce document requirements for domestic vs. international locations.
/// </summary>
public class DocumentValidationService : IDocumentValidationService
{
    private static readonly HashSet<string> DomesticLocations = new(StringComparer.OrdinalIgnoreCase)
    {
        "Mumbai",
        "Bengaluru"
    };

    private static readonly HashSet<string> InternationalLocations = new(StringComparer.OrdinalIgnoreCase)
    {
        "Dubai",
        "Singapore",
        "London"
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentValidationService"/> class.
    /// </summary>
    public DocumentValidationService()
    {
    }

    /// <summary>
    /// Validates that the document type is appropriate for the pickup location.
    /// Domestic locations accept both NationalId and Passport.
    /// International locations require Passport only.
    /// </summary>
    /// <param name="documentType">The type of travel document (NationalId or Passport).</param>
    /// <param name="location">The pickup location.</param>
    /// <returns>True if the document is valid for the location; otherwise false.</returns>
    public bool IsDocumentValidForLocation(string documentType, string location)
    {
        if (string.IsNullOrWhiteSpace(documentType) || string.IsNullOrWhiteSpace(location))
        {
            return false;
        }

        // Check if location is international
        if (IsInternationalLocation(location))
        {
            // International locations require Passport only
            return documentType.Equals("Passport", StringComparison.OrdinalIgnoreCase);
        }

        // Domestic locations accept both NationalId and Passport
        return documentType.Equals("NationalId", StringComparison.OrdinalIgnoreCase) ||
               documentType.Equals("Passport", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Determines if a location requires international travel documents.
    /// </summary>
    /// <param name="location">The location to check.</param>
    /// <returns>True if the location is international; otherwise false.</returns>
    public bool IsInternationalLocation(string location)
    {
        if (string.IsNullOrWhiteSpace(location))
        {
            return false;
        }

        return InternationalLocations.Contains(location);
    }

    /// <inheritdoc />
    public bool IsKnownLocation(string location)
    {
        if (string.IsNullOrWhiteSpace(location))
        {
            return false;
        }

        return DomesticLocations.Contains(location) || InternationalLocations.Contains(location);
    }
}
