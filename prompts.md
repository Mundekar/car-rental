# Phase 1B: Project Skeleton & Boilerplate

## Objective

Generate a production-ready project skeleton for the Car Rental Availability System with complete architectural boilerplate, demonstrating enterprise-grade .NET 8 and React TypeScript conventions without any business logic implementation.

---

## Phase

**Phase 1B - Architecture & Boilerplate Scaffolding**

**Status:** Complete

**Deliverables:**
- Complete .NET 8 Minimal API project structure
- Interface-first architectural design
- Service layer with dependency injection
- Provider abstraction pattern
- Endpoint registration framework
- Global exception middleware
- Validator framework skeleton
- Complete test project structure
- React + TypeScript + Vite frontend setup
- Solution configuration and project files

---

## Prompt Used

```
You are a Principal .NET 8 Solution Architect.

This is PHASE 1B of the project.

The project specification (README.md and spec.md) has already been completed and committed.

Your responsibility is to generate the project skeleton ONLY.

Generate production-quality project structure with:

CarRental.Api
CarRental.Tests
car-rental-ui

Create all folders, interfaces, services, providers, endpoints, middleware, validators, extensions, and configuration files.

Every class should contain XML summary comments.
Every method should throw NotImplementedException.
No business logic. No implementation. No fake data.
```

---

## Key Architectural Decisions

### 1. .NET 8 Minimal API

**Decision:** Use Minimal APIs instead of traditional Controllers.

**Rationale:**
- **Lightweight** - Reduces boilerplate for simple endpoints
- **Performance** - Lower memory footprint and faster startup
- **Modern** - Aligns with current .NET best practices (v8+)
- **Explicit** - Route configuration is clear and centralized in Program.cs
- **Testing** - Endpoints are testable without full HTTP server instantiation
- **Scalability** - Scales from simple CRUD to complex enterprise APIs

**Trade-off:** Cannot use model binding attributes; validation handled via services.

**Example Pattern:**
```csharp
app.MapGet("/cars/search", endpoint => { ... });
```

---

### 2. Dependency Injection (DI)

**Decision:** Use .NET built-in `IServiceCollection` and `IServiceProvider`.

**Rationale:**
- **Built-in** - No external container dependency
- **Production-Ready** - Used in enterprise applications
- **Testability** - Mock implementations registered for unit tests
- **Extensibility** - Easy to register new providers or services
- **SOLID** - Enforces Dependency Inversion Principle

**Implementation Pattern:**
```csharp
// Extension method for clean registration
services.AddApplicationServices();
services.AddRentalProviders();
services.AddValidators();
```

**Scope Strategy:**
- `Scoped` for services (per-request lifetime)
- `Transient` for stateless utilities
- `Singleton` for configuration

---

### 3. Interface-First Design

**Decision:** All services and providers defined by interfaces before implementation.

**Rationale:**
- **Contract Definition** - Interfaces define behavior clearly
- **Testability** - Easy to mock for unit tests
- **Extensibility** - New providers added by implementing interface
- **Decoupling** - Implementation details hidden behind abstractions
- **SOLID** - Follows Interface Segregation & Dependency Inversion principles

**Interfaces Defined:**
- `ICarRentalProvider` - Provider abstraction
- `ICarRentalService` - Search orchestration
- `IBookingService` - Booking operations
- `IDocumentValidationService` - Document validation
- `IPricingStrategy` - Pricing calculations

**Zero-Code Extension:** Adding a new provider:
1. Create `ThirdProviderClient : ICarRentalProvider`
2. Implement two methods
3. Register in DI: `services.AddScoped<ICarRentalProvider, ThirdProviderClient>()`
4. No changes to SearchService or endpoints

---

### 4. Project Structure

**Decision:** Layered architecture with clear separation of concerns.

**Rationale:**
- **Maintainability** - Each layer has single responsibility
- **Testability** - Layers tested independently
- **Scalability** - Easy to add new components
- **Clarity** - Code organization is immediately obvious

**Folder Structure:**
```
CarRental.Api/
├── Endpoints/          ← HTTP contracts (thin, no logic)
├── Services/           ← Business orchestration
├── Providers/          ← External integrations
├── Interfaces/         ← Service contracts
├── DTOs/               ← Data transfer objects
├── Models/             ← Domain entities
├── Validators/         ← Validation rules (framework)
├── Middleware/         ← Cross-cutting concerns
├── Extensions/         ← DI and configuration helpers
├── Common/             ← Shared utilities
├── Configuration/      ← Settings and options
└── Program.cs          ← Application entry point
```

**Layering Pattern:**
```
Endpoints (HTTP) 
  ↓ (delegates to)
Services (Orchestration)
  ↓ (delegates to)
Domain Logic / Providers
  ↓ (depends on)
Interfaces (Abstractions)
```

---

### 5. Extension Methods for Configuration

**Decision:** Use extension methods for clean, fluent service registration.

**Rationale:**
- **Readability** - Program.cs is concise and clear
- **Maintainability** - Configuration logic grouped by concern
- **Reusability** - Extensions can be used across projects
- **Testability** - Each extension tested independently

**Implemented Extensions:**
- `DependencyInjectionExtensions` - Service registration
- `SwaggerExtensions` - OpenAPI documentation
- `MiddlewareExtensions` - Middleware and CORS configuration

**Usage in Program.cs:**
```csharp
builder.Services
    .AddSwaggerConfiguration()
    .AddCorsConfiguration()
    .AddApplicationServices()
    .AddRentalProviders()
    .AddValidators();

app
    .UseSwaggerConfiguration()
    .UseCorsConfiguration()
    .UseGlobalExceptionHandling();
```

---

### 6. Middleware Pipeline

**Decision:** Implement global exception middleware for cross-cutting concerns.

**Rationale:**
- **Centralized Error Handling** - Single point for exception processing
- **Consistency** - All errors follow same format
- **Logging** - All exceptions logged before response
- **Security** - Internal details not exposed in responses

**Middleware Stack:**
1. Exception Handling
2. Logging
3. CORS
4. Routing
5. Endpoints

---

### 7. Validation Framework

**Decision:** Separate validator classes with no validation logic yet.

**Rationale:**
- **Separation of Concerns** - Validation logic isolated
- **Reusability** - Same validators used by endpoints and services
- **Testability** - Validators tested independently
- **Extensibility** - Easy to add FluentValidation or similar

**Validators Created:**
- `SearchRequestValidator` - Validates search criteria
- `BookingRequestValidator` - Validates booking details

---

### 8. React + TypeScript + Vite Frontend

**Decision:** Modern frontend stack with type safety and fast development.

**Rationale:**
- **Type Safety** - TypeScript catches errors at compile-time
- **Development Speed** - Vite provides instant HMR and fast builds
- **Modern Framework** - React 18 with hooks for functional components
- **Ecosystem** - Large ecosystem of libraries and tools

**Frontend Structure:**
```
car-rental-ui/
├── src/
│   ├── components/    ← Reusable UI components
│   ├── pages/         ← Page components
│   ├── services/      ← API communication
│   ├── hooks/         ← Custom React hooks
│   ├── types/         ← TypeScript type definitions
│   └── styles/        ← Global styles
├── public/            ← Static assets
├── package.json       ← Dependencies
├── vite.config.ts     ← Build configuration
└── tsconfig.json      ← TypeScript configuration
```

---

### 9. xUnit Testing Framework

**Decision:** xUnit with Moq for test doubles.

**Rationale:**
- **Modern** - Built for .NET Core/.NET 5+
- **Flexible** - Theory tests for parameterized testing
- **Community** - Widely used in .NET community
- **Mocking** - Moq provides clean mock creation

**Test Structure:**
- `Services/` - Service behavior tests
- `Providers/` - Provider implementation tests
- `Validators/` - Validation rule tests
- `Endpoints/` - HTTP endpoint tests

---

### 10. No Business Logic in Skeleton

**Decision:** All methods throw `NotImplementedException`.

**Rationale:**
- **Clear Contract** - Skeleton shows structure, not implementation
- **TDD Ready** - Tests drive implementation
- **Prevents Confusion** - Clear that skeleton is placeholder
- **Safe** - No accidentally-included logic
- **Architectural Clarity** - Architecture independent of logic

---

## Project Files Created

### Backend (.NET 8)

**Projects:**
- `CarRental.Api.csproj` - Main API project
- `CarRental.Tests.csproj` - Unit test project

**Models:** Vehicle, Booking, ProviderVehicle, Location

**DTOs:** SearchRequestDto, SearchResponseDto, BookingRequestDto, BookingResponseDto, BookingDetailsDto, ProviderVehicleDto

**Interfaces:** ICarRentalProvider, ICarRentalService, IBookingService, IDocumentValidationService, IPricingStrategy

**Services:**
- CarRentalService
- BookingService
- DocumentValidationService

**Providers:**
- PremiumDriveProvider
- BudgetWheelsProvider

**Endpoints:**
- CarsEndpoints (GET /cars/search)
- BookingEndpoints (POST /cars/book, GET /cars/booking/{reference})

**Middleware:** GlobalExceptionMiddleware

**Validators:** SearchRequestValidator, BookingRequestValidator

**Extensions:**
- DependencyInjectionExtensions
- SwaggerExtensions
- MiddlewareExtensions

**Configuration:**
- Program.cs
- appsettings.json
- appsettings.Development.json
- launchSettings.json

### Frontend (React + TypeScript + Vite)

**Configuration:**
- package.json
- vite.config.ts
- tsconfig.json
- tsconfig.node.json
- index.html

**Source Structure:**
- src/main.tsx - Entry point
- src/types/index.ts - Type definitions
- src/services/api.ts - API client placeholder
- src/components/ - Component directory
- src/pages/ - Page directory
- src/hooks/ - Custom hooks directory
- src/styles/ - Styles directory

### Solution & Tests

**Solution:**
- CarRental.sln

**Test Project:**
- CarRental.Tests (xUnit + Moq)
- Test class placeholders for all major components

---

## Build & Run Instructions

### Backend

```bash
# Navigate to solution directory
cd d:\car-rental

# Restore packages
dotnet restore

# Build solution
dotnet build

# Run API (Development)
dotnet run --project src/CarRental.Api

# API available at http://localhost:5000
# Swagger UI at http://localhost:5000/swagger
```

### Frontend

```bash
# Navigate to frontend directory
cd car-rental-ui

# Install dependencies
npm install

# Development server
npm run dev

# Build for production
npm run build

# Preview production build
npm run preview
```

### Tests

```bash
# Run all tests
dotnet test

# Run with verbose output
dotnet test --verbosity detailed

# Run specific test file
dotnet test --filter "FullyQualifiedName~CarRentalTests"
```

---

## Next Steps (Phase 1C)

1. **Implement Services** - Add business logic to service methods
2. **Implement Providers** - Add provider-specific logic
3. **Implement Validation** - Add validation rules
4. **Implement Pricing** - Add pricing calculations
5. **Add Endpoints Logic** - Wire services to endpoints
6. **Add Frontend Components** - Create UI components
7. **Add Tests** - Implement test cases
8. **Integration Testing** - End-to-end scenario testing

---

## Conventions Applied

### C# Code Style
- XML summary comments on all public members
- Proper nullable reference types (`#nullable enable`)
- Async/await pattern for I/O operations
- PascalCase for class and method names
- camelCase for local variables and parameters

### Project Organization
- Namespace matches folder structure
- Interfaces prefixed with `I`
- Service implementations follow `{Name}Service` pattern
- Provider implementations follow `{Name}Provider` pattern
- DTOs suffixed with `Dto`

### TypeScript Code Style
- JSDoc comments for functions
- Type definitions in dedicated files
- Strict type checking enabled
- Const assertions for const references
- Arrow functions for event handlers

---

**Version:** 1.0  
**Date:** 2026-07-29  
**Phase:** 1B Complete
