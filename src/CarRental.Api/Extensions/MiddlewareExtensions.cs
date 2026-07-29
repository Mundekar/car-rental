namespace CarRental.Api.Extensions;

using CarRental.Api.Middleware;

/// <summary>
/// Extension methods for middleware configuration.
/// </summary>
public static class MiddlewareExtensions
{
    /// <summary>
    /// Adds the global exception handling middleware to the application pipeline.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The updated application builder for method chaining.</returns>
    public static WebApplication UseGlobalExceptionHandling(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.UseMiddleware<GlobalExceptionMiddleware>();

        return app;
    }

    /// <summary>
    /// Adds CORS configuration to the application.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection for method chaining.</returns>
    public static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        return services;
    }

    /// <summary>
    /// Applies CORS policy to the application.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The updated application builder for method chaining.</returns>
    public static WebApplication UseCorsConfiguration(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.UseCors("AllowAll");

        return app;
    }
}
