namespace CarRental.Api.Extensions;

using CarRental.Api.Interfaces;
using CarRental.Api.Providers;
using CarRental.Api.Services;
using CarRental.Api.Strategies;
using CarRental.Api.Validators;

/// <summary>
/// Extension methods for dependency injection configuration.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Registers application services in the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection for method chaining.</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<ICarRentalService, CarRentalService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IDocumentValidationService, DocumentValidationService>();

        return services;
    }

    /// <summary>
    /// Registers rental car providers in the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection for method chaining.</returns>
    public static IServiceCollection AddRentalProviders(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<ICarRentalProvider, PremiumDriveProvider>();
        services.AddScoped<ICarRentalProvider, BudgetWheelsProvider>();

        return services;
    }

    /// <summary>
    /// Registers validators in the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection for method chaining.</returns>
    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<SearchRequestValidator>();
        services.AddScoped<BookingRequestValidator>();

        return services;
    }

    /// <summary>
    /// Registers pricing strategies in the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection for method chaining.</returns>
    public static IServiceCollection AddPricingStrategies(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<PremiumDrivePricingStrategy>();
        services.AddScoped<BudgetWheelsPricingStrategy>();
        services.AddScoped<IPricingStrategyRegistry, PricingStrategyRegistry>();

        return services;
    }
}

