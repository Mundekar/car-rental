namespace CarRental.Api.Strategies;

using CarRental.Api.Common;
using CarRental.Api.Interfaces;

/// <summary>
/// Registry for pricing strategies.
/// Centralizes strategy registration and lookup by provider type.
/// </summary>
public class PricingStrategyRegistry : IPricingStrategyRegistry
{
    private readonly Dictionary<ProviderType, IPricingStrategy> _strategies;

    /// <summary>
    /// Initializes a new instance of the <see cref="PricingStrategyRegistry"/> class.
    /// </summary>
    /// <param name="premiumDriveStrategy">The strategy for PremiumDrive.</param>
    /// <param name="budgetWheelsStrategy">The strategy for BudgetWheels.</param>
    public PricingStrategyRegistry(
        PremiumDrivePricingStrategy premiumDriveStrategy,
        BudgetWheelsPricingStrategy budgetWheelsStrategy)
    {
        ArgumentNullException.ThrowIfNull(premiumDriveStrategy, nameof(premiumDriveStrategy));
        ArgumentNullException.ThrowIfNull(budgetWheelsStrategy, nameof(budgetWheelsStrategy));

        _strategies = new Dictionary<ProviderType, IPricingStrategy>
        {
            { ProviderType.PremiumDrive, premiumDriveStrategy },
            { ProviderType.BudgetWheels, budgetWheelsStrategy }
        };
    }

    /// <summary>
    /// Gets the pricing strategy for a provider type.
    /// </summary>
    /// <param name="providerType">The provider type.</param>
    /// <returns>The pricing strategy.</returns>
    /// <exception cref="ArgumentException">Thrown when provider type is not registered.</exception>
    public IPricingStrategy GetStrategy(ProviderType providerType)
    {
        if (!_strategies.TryGetValue(providerType, out var strategy))
        {
            throw new ArgumentException($"No pricing strategy registered for provider type: {providerType}", nameof(providerType));
        }

        return strategy;
    }
}
