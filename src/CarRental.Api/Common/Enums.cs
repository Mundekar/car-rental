namespace CarRental.Api.Common;

/// <summary>
/// Vehicle rental categories.
/// </summary>
public enum VehicleCategory
{
    /// <summary>
    /// Economy class vehicle (compact, fuel-efficient).
    /// </summary>
    Economy = 0,

    /// <summary>
    /// Compact class vehicle (small, easy to park).
    /// </summary>
    Compact = 1,

    /// <summary>
    /// SUV class vehicle (spacious, off-road capable).
    /// </summary>
    SUV = 2,

    /// <summary>
    /// Minivan class vehicle (family-friendly, maximum seating).
    /// </summary>
    Minivan = 3
}

/// <summary>
/// Insurance coverage types.
/// </summary>
public enum InsuranceType
{
    /// <summary>
    /// Basic insurance covering third-party liability only.
    /// </summary>
    Basic = 0,

    /// <summary>
    /// Comprehensive insurance including damage, theft, and collision.
    /// </summary>
    Comprehensive = 1
}

/// <summary>
/// Cancellation policy types.
/// </summary>
public enum CancellationPolicy
{
    /// <summary>
    /// Free cancellation up to 48 hours before pickup.
    /// </summary>
    Free48Hours = 0,

    /// <summary>
    /// Non-refundable once confirmed.
    /// </summary>
    NonRefundable = 1
}

/// <summary>
/// Travel document types for booking validation.
/// </summary>
public enum DocumentType
{
    /// <summary>
    /// National identification (valid for domestic travel).
    /// </summary>
    NationalId = 0,

    /// <summary>
    /// Passport (required for international travel).
    /// </summary>
    Passport = 1
}

/// <summary>
/// Rental provider types.
/// </summary>
public enum ProviderType
{
    /// <summary>
    /// PremiumDrive provider (flat daily rate).
    /// </summary>
    PremiumDrive = 0,

    /// <summary>
    /// BudgetWheels provider (base rate with weekend surcharge).
    /// </summary>
    BudgetWheels = 1
}
