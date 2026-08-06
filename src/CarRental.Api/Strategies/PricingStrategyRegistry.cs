namespace CarRental.Api.Strategies;

using CarRental.Api.Interfaces;

/// <summary>
/// Registry for pricing strategies.
/// Discovers all registered <see cref="IPricingStrategy"/> implementations and indexes them
/// by <see cref="IPricingStrategy.ProviderName"/>, so adding a new provider requires no changes here.
/// </summary>
public class PricingStrategyRegistry : IPricingStrategyRegistry
{
    private readonly Dictionary<string, IPricingStrategy> _strategies;

    /// <summary>
    /// Initializes a new instance of the <see cref="PricingStrategyRegistry"/> class.
    /// </summary>
    /// <param name="strategies">All registered pricing strategies, injected by the DI container.</param>
    public PricingStrategyRegistry(IEnumerable<IPricingStrategy> strategies)
    {
        ArgumentNullException.ThrowIfNull(strategies, nameof(strategies));

        _strategies = new Dictionary<string, IPricingStrategy>(StringComparer.OrdinalIgnoreCase);

        foreach (var strategy in strategies)
        {
            if (!_strategies.TryAdd(strategy.ProviderName, strategy))
            {
                throw new InvalidOperationException(
                    $"Duplicate pricing strategy registered for provider: {strategy.ProviderName}");
            }
        }
    }

    /// <summary>
    /// Gets the pricing strategy for a provider.
    /// </summary>
    /// <param name="providerName">The provider name.</param>
    /// <returns>The pricing strategy.</returns>
    /// <exception cref="ArgumentException">Thrown when the provider is not registered.</exception>
    public IPricingStrategy GetStrategy(string providerName)
    {
        if (string.IsNullOrWhiteSpace(providerName) || !_strategies.TryGetValue(providerName, out var strategy))
        {
            throw new ArgumentException($"No pricing strategy registered for provider: {providerName}", nameof(providerName));
        }

        return strategy;
    }
}
