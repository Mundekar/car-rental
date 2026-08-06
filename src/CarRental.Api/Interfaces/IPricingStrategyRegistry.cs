namespace CarRental.Api.Interfaces;

/// <summary>
/// Registry for pricing strategies by provider name.
/// Strategies self-register via <see cref="IPricingStrategy.ProviderName"/>, so new providers
/// require no changes to this registry.
/// </summary>
public interface IPricingStrategyRegistry
{
    /// <summary>
    /// Gets the pricing strategy for a specific provider.
    /// </summary>
    /// <param name="providerName">The provider name.</param>
    /// <returns>The pricing strategy for the provider.</returns>
    /// <exception cref="ArgumentException">Thrown when the provider is not registered.</exception>
    IPricingStrategy GetStrategy(string providerName);
}
