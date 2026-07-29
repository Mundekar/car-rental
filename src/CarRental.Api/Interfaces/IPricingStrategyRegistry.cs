namespace CarRental.Api.Interfaces;

using CarRental.Api.Common;

/// <summary>
/// Registry for pricing strategies by provider type.
/// Provides centralized strategy lookup without string magic or conditionals.
/// </summary>
public interface IPricingStrategyRegistry
{
    /// <summary>
    /// Gets the pricing strategy for a specific provider type.
    /// </summary>
    /// <param name="providerType">The provider type.</param>
    /// <returns>The pricing strategy for the provider.</returns>
    /// <exception cref="ArgumentException">Thrown when provider type is not registered.</exception>
    IPricingStrategy GetStrategy(ProviderType providerType);
}
