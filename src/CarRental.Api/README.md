# Car Rental Backend - Minimal API

.NET 8 Minimal API backend for car rental aggregation system.

## Features

- Minimal API endpoint registration
- Interface-based service architecture
- Multi-provider aggregation pattern
- Dependency injection configuration
- Exception middleware
- Health checks
- Swagger/OpenAPI documentation
- CORS support

## Structure

### Endpoints
- **GET /cars/search** - Search available vehicles
- **POST /cars/book** - Create booking
- **GET /cars/booking/{reference}** - Retrieve booking

### Services
- `ICarRentalService` - Search orchestration
- `IBookingService` - Booking operations
- `IDocumentValidationService` - Document validation

### Providers
- `ICarRentalProvider` - Provider abstraction
- `PremiumDriveProvider` - PremiumDrive implementation
- `BudgetWheelsProvider` - BudgetWheels implementation

## Building

```bash
dotnet build
dotnet run --project src/CarRental.Api
```

## Testing

```bash
dotnet test
```

## API Documentation

Swagger documentation available at `/swagger` when running in development.
