using CarRental.Api.Endpoints;
using CarRental.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services
    .AddSwaggerConfiguration()
    .AddCorsConfiguration()
    .AddApplicationServices()
    .AddRentalProviders()
    .AddValidators()
    .AddHealthChecks();

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

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
app.MapCarsEndpoints();
app.MapBookingEndpoints();

app.Run();
