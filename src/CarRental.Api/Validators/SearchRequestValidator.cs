namespace CarRental.Api.Validators;

using CarRental.Api.DTOs;
using CarRental.Api.Interfaces;

/// <summary>
/// Validator for search request input validation.
/// Ensures all required parameters are present and logically valid.
/// </summary>
public class SearchRequestValidator
{
    private readonly IDocumentValidationService _documentValidationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchRequestValidator"/> class.
    /// </summary>
    /// <param name="documentValidationService">Service used to verify recognised pickup locations.</param>
    public SearchRequestValidator(IDocumentValidationService documentValidationService)
    {
        _documentValidationService = documentValidationService ?? throw new ArgumentNullException(nameof(documentValidationService));
    }

    /// <summary>
    /// Validates the search request.
    /// </summary>
    /// <param name="request">The search request to validate.</param>
    /// <returns>A collection of validation error messages, empty if valid.</returns>
    public IEnumerable<string> Validate(SearchRequestDto request)
    {
        var errors = new List<string>();

        if (request == null)
        {
            errors.Add("Search request cannot be null.");
            return errors;
        }

        if (string.IsNullOrWhiteSpace(request.Pickup))
        {
            errors.Add("Pickup location is required.");
        }
        else if (!_documentValidationService.IsKnownLocation(request.Pickup))
        {
            errors.Add($"'{request.Pickup}' is not a recognised pickup location.");
        }

        if (request.From == default)
        {
            errors.Add("From date is required.");
        }

        if (request.To == default)
        {
            errors.Add("To date is required.");
        }

        if (request.From != default && request.To != default && request.To <= request.From)
        {
            errors.Add("To date must be after From date.");
        }

        return errors;
    }
}
