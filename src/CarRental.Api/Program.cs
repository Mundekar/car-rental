using CarRental.Api.Endpoints;
using CarRental.Api.Extensions;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add logging configuration
builder.Services.AddLogging(config =>
{
    config.ClearProviders();
    config.AddConsole();
    config.AddDebug();

    // Set log levels
    config.SetMinimumLevel(LogLevel.Information);
    config.AddFilter("Microsoft", LogLevel.Warning);
    config.AddFilter("System", LogLevel.Warning);
});

// Add services to the container
builder.Services
    .AddSwaggerConfiguration()
    .AddCorsConfiguration()
    .AddApplicationServices()
    .AddRentalProviders()
    .AddValidators()
    .AddPricingStrategies()
    .AddHealthChecks();

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Add request/response logging middleware
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

    var request = context.Request;
    logger.LogInformation(
        "Request: {Method} {Path}",
        request.Method,
        request.Path);

    await next();

    logger.LogInformation(
        "Response: {Method} {Path} Status={StatusCode}",
        request.Method,
        request.Path,
        context.Response.StatusCode);
});

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerConfiguration();
}

app.UseHttpsRedirection();

app.UseCorsConfiguration();

app.UseGlobalExceptionHandling();

app.MapHealthChecks("/health");

// Map endpoints
app.MapSearchEndpoints();
app.MapBookingEndpoints();

app.Run();
