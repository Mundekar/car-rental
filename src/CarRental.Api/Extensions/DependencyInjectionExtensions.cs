namespace CarRental.Api.Extensions;

using System.Reflection;
using CarRental.Api.Interfaces;
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
    /// Discovers every <see cref="ICarRentalProvider"/> implementation in this assembly,
    /// so a new provider class is picked up automatically without editing this method.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection for method chaining.</returns>
    public static IServiceCollection AddRentalProviders(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddImplementationsOf<ICarRentalProvider>();

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
    /// Discovers every <see cref="IPricingStrategy"/> implementation in this assembly,
    /// so a new strategy class is picked up automatically without editing this method.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection for method chaining.</returns>
    public static IServiceCollection AddPricingStrategies(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddImplementationsOf<IPricingStrategy>();
        services.AddScoped<IPricingStrategyRegistry, PricingStrategyRegistry>();

        return services;
    }

    /// <summary>
    /// Scans this assembly for concrete, non-abstract implementations of <typeparamref name="TService"/>
    /// and registers each of them against <typeparamref name="TService"/>.
    /// </summary>
    /// <typeparam name="TService">The service abstraction to scan for implementations of.</typeparam>
    /// <param name="services">The service collection.</param>
    private static void AddImplementationsOf<TService>(this IServiceCollection services)
    {
        var implementationTypes = typeof(TService).Assembly.GetTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false } && typeof(TService).IsAssignableFrom(type));

        foreach (var implementationType in implementationTypes)
        {
            services.AddScoped(typeof(TService), implementationType);
        }
    }
}

