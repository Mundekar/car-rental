namespace CarRental.Api.Validators;

using CarRental.Api.DTOs;

/// <summary>
/// Validator for search request input validation.
/// </summary>
public class SearchRequestValidator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SearchRequestValidator"/> class.
    /// </summary>
    public SearchRequestValidator()
    {
    }

    /// <summary>
    /// Validates the search request.
    /// </summary>
    /// <param name="request">The search request to validate.</param>
    /// <returns>A collection of validation error messages, empty if valid.</returns>
    public IEnumerable<string> Validate(SearchRequestDto request)
    {
        throw new NotImplementedException();
    }
}
