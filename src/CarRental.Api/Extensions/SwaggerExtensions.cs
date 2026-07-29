namespace CarRental.Api.Extensions;

/// <summary>
/// Extension methods for Swagger/OpenAPI configuration.
/// </summary>
public static class SwaggerExtensions
{
    /// <summary>
    /// Adds Swagger/OpenAPI documentation configuration to services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection for method chaining.</returns>
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "Car Rental Availability API",
                Version = "v1",
                Description = "API for searching and booking rental vehicles across multiple providers"
            });
        });

        return services;
    }

    /// <summary>
    /// Configures Swagger/OpenAPI middleware in the application pipeline.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The updated application builder for method chaining.</returns>
    public static WebApplication UseSwaggerConfiguration(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Car Rental API v1");
            });
        }

        return app;
    }
}
