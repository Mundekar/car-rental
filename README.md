# Car Rental Availability System

## Project Overview

The **Car Rental Availability System** is a cloud-ready, multi-provider aggregation platform designed to unify rental availability and pricing across competing rental service providers. The system presents a single, normalized search interface to end users while abstracting the complexities of provider-specific pricing models, availability logic, and booking workflows.

This is a greenfield project built to demonstrate enterprise-grade architectural patterns, extensibility principles, and separation of concerns in modern .NET applications.

---

## Business Problem

Travellers face fragmentation when searching for rental cars. Each rental provider operates independently with:

- **Different pricing models** (flat vs. dynamic, surcharges for weekends)
- **Distinct availability rules** (some always available, some may return unavailable vehicles)
- **Varying insurance options** (comprehensive vs. basic)
- **Inconsistent cancellation policies** (refundable vs. non-refundable)

**Solution:** Provide a unified search experience that queries multiple providers, normalizes results, and presents them in a consistent format while maintaining provider-specific business rules and constraints.

---

## Functional Overview

### Core Features

**Search**
- Query available vehicles across two rental providers in a single API call
- Filter by pickup location, dates, and optional vehicle category
- Return normalized results ranked by total price
- Display provider-specific terms and pricing breakdown

**Booking**
- Reserve a vehicle after validating traveller documents
- Support domestic and international locations with location-specific validation rules
- Generate and return booking confirmations with reference numbers

**Booking Lookup**
- Retrieve previously made booking confirmations by reference number

### Supported Vehicle Categories

- Economy
- Compact
- SUV
- Minivan

### Supported Locations

**Domestic (India)**
- Mumbai
- Bengaluru

**International**
- Dubai
- Singapore
- London

---

## Technology Stack

### Backend
- **.NET 8** - Latest stable framework
- **Minimal APIs** - Lightweight, modern endpoint definition
- **C#** - Primary implementation language
- **Dependency Injection** - Built-in DI container

### Frontend
- **React 18+** - UI framework
- **TypeScript** - Type-safe JavaScript
- **Responsive Design** - Mobile and desktop support

### Testing
- **xUnit** - Comprehensive unit and integration testing
- **Mocking Libraries** - Provider simulation and isolation

### Data Storage
- **In-Memory** - Single-instance data persistence (Phase 1)
- **No Database** - Simplified infrastructure for assessment scope

### Development Environment
- **Visual Studio** or **Visual Studio Code**
- **Git** - Version control
- **.gitignore** - Standard .NET configuration

---

## Proposed Architecture

### Design Principles

1. **Layered Architecture**
   - Endpoint/Controller layer (thin, responsible only for HTTP concerns)
   - Service/Application layer (business logic and orchestration)
   - Domain layer (core business entities and rules)
   - Infrastructure layer (external provider integration)

2. **Provider Abstraction**
   - Abstract interface for provider interaction
   - Concrete implementations for each rental provider
   - Extensible design to support additional providers with minimal code changes

3. **Separation of Concerns**
   - Pricing calculation isolated in dedicated services
   - Validation logic separated from endpoint handlers
   - Provider communication decoupled from business logic

4. **Dependency Inversion**
   - Interfaces define contracts
   - Implementation classes depend on abstractions
   - Runtime configuration determines concrete implementations

### Component Layers

```
┌─────────────────────────────────────────┐
│         Minimal API Endpoints           │  HTTP Contracts
├─────────────────────────────────────────┤
│      Application Services Layer         │  Orchestration & Validation
├─────────────────────────────────────────┤
│  Pricing | Booking | Search Aggregation │  Business Logic
├─────────────────────────────────────────┤
│  Provider Interfaces & Implementations  │  External Integration
├─────────────────────────────────────────┤
│       Domain Models & Entities          │  Core Business Concepts
└─────────────────────────────────────────┘
```

### Extensibility Strategy

The architecture is designed to onboard new providers with **zero changes** to existing endpoint code:

1. Implement new provider interface
2. Register in dependency injection
3. Add provider-specific pricing and availability logic
4. System automatically includes results in aggregated responses

---

## Repository Structure

### Backend (Single Monolithic Project)

```
car-rental/
├── README.md                           # Project overview
├── spec.md                             # Technical specification
├── prompts.md                          # Phase-by-phase development history
├── reflection.md                       # Architecture reflection & lessons
├── .gitignore                          # Git ignore rules
│
├── src/CarRental.Api/                 # Single .NET 8 project with logical layers
│   ├── Program.cs                      # Dependency injection & middleware setup
│   │
│   ├── Endpoints/                      # Thin HTTP layer (minimal API endpoints)
│   │   ├── CarsEndpoints.cs            # GET /cars/search endpoint
│   │   └── BookingEndpoints.cs         # POST /cars/book, GET /cars/booking/{ref}
│   │
│   ├── Services/                       # Business logic & orchestration
│   │   ├── CarRentalService.cs         # Search aggregation & result normalization
│   │   ├── BookingService.cs           # Booking creation & reference management
│   │   └── DocumentValidationService.cs # Document location validation
│   │
│   ├── Providers/                      # Provider implementations (ICarRentalProvider)
│   │   ├── PremiumDriveProvider.cs     # Flat pricing provider
│   │   └── BudgetWheelsProvider.cs     # Weekend surcharge provider
│   │
│   ├── Strategies/                     # Pricing calculation strategies
│   │   ├── PremiumDrivePricingStrategy.cs  # Flat daily rate × days
│   │   └── BudgetWheelsPricingStrategy.cs  # Base with 20% Fri/Sat/Sun surcharge
│   │
│   ├── Validators/                     # Request validation
│   │   ├── SearchRequestValidator.cs
│   │   └── BookingRequestValidator.cs
│   │
│   ├── DTOs/                           # Data transfer objects
│   │   ├── SearchRequestDto.cs
│   │   ├── SearchResponseDto.cs
│   │   ├── BookingRequestDto.cs
│   │   └── BookingResponseDto.cs
│   │
│   ├── Models/                         # Domain models
│   │   ├── Booking.cs
│   │   ├── ProviderVehicle.cs
│   │   └── SearchCriteria.cs
│   │
│   ├── Interfaces/                     # Abstractions
│   │   ├── ICarRentalProvider.cs
│   │   ├── ICarRentalService.cs
│   │   ├── IBookingService.cs
│   │   ├── IDocumentValidationService.cs
│   │   └── IPricingStrategy.cs
│   │
│   ├── Common/                         # Enums & constants
│   │   ├── VehicleCategory.cs
│   │   ├── DocumentType.cs
│   │   ├── InsuranceType.cs
│   │   └── CancellationPolicy.cs
│   │
│   ├── Extensions/                     # Dependency injection & middleware
│   │   ├── DependencyInjectionExtensions.cs
│   │   ├── MiddlewareExtensions.cs
│   │   └── SwaggerExtensions.cs
│   │
│   ├── Middleware/                     # HTTP middleware
│   │   └── GlobalExceptionHandlingMiddleware.cs
│   │
│   ├── Configuration/                  # Configuration classes
│   │   └── CorsConfiguration.cs
│   │
│   ├── appsettings.json                # Default configuration
│   └── appsettings.Development.json    # Development overrides
│
├── tests/CarRental.Tests/              # xUnit test project
│   ├── Endpoints/                      # API endpoint tests
│   ├── Services/                       # Service & business logic tests
│   ├── Strategies/                     # Pricing strategy tests
│   ├── Providers/                      # Provider tests with mocks
│   └── Validators/                     # Input validation tests
│
└── car-rental-ui/                      # React TypeScript frontend (separate directory)
    ├── src/
    │   ├── components/                 # Reusable React components
    │   │   ├── SearchForm.tsx
    │   │   ├── ResultsTable.tsx
    │   │   ├── BookingForm.tsx
    │   │   ├── BookingConfirmation.tsx
    │   │   ├── ErrorMessage.tsx
    │   │   └── LoadingSpinner.tsx
    │   │
    │   ├── pages/                      # Page components with routing
    │   │   ├── HomePage.tsx            # /
    │   │   ├── ResultsPage.tsx         # /results
    │   │   ├── BookingPage.tsx         # /booking
    │   │   ├── ConfirmationPage.tsx    # /confirmation/:reference
    │   │   └── NotFoundPage.tsx        # /* (catch-all)
    │   │
    │   ├── hooks/                      # Custom React hooks
    │   │   ├── useSearch.ts            # Search state management
    │   │   └── useBooking.ts           # Booking state management
    │   │
    │   ├── services/                   # API communication
    │   │   └── apiService.ts           # Axios HTTP client
    │   │
    │   ├── utils/                      # Utility functions
    │   │   ├── validation.ts           # Client-side validation logic
    │   │   ├── dateUtils.ts            # Date & price formatting
    │   │   └── index.ts                # Re-exports
    │   │
    │   ├── types/                      # TypeScript types
    │   │   └── index.ts                # All enums & interfaces
    │   │
    │   ├── styles/                     # Global styling
    │   │   └── globalStyles.ts
    │   │
    │   ├── layouts/                    # Layout wrappers
    │   │   └── Layout.tsx
    │   │
    │   └── main.tsx                    # App entry & routing config
    │
    ├── dist/                           # Production build output
    ├── package.json                    # Dependencies & scripts
    ├── tsconfig.json                   # TypeScript configuration
    └── vite.config.ts                  # Vite build configuration
```

### Architecture Notes

- **Logical Layering via Namespaces**: Despite being single project, code is organized by responsibility (Endpoints → Services → Providers → Common)
- **No Multi-Project Complexity**: Simpler deployment, faster local development
- **Future Scalability**: Can split into separate projects in Phase 6 without API changes

---

## Assumptions

1. **Single Instance Deployment** - In-memory storage supports a single application instance only
2. **Synchronous Provider Calls** - No real-time async provider streams; responses come on request
3. **Deterministic Availability** - Providers respond consistently to the same request
4. **No Authentication/Authorization** - Public endpoints; no user identity verification
5. **EST/UTC Time Zone** - Date calculations assume consistent time zone handling
6. **Provider Uptime** - Assumes provider endpoints are available; no fallback logic implemented
7. **Small Dataset** - In-memory storage suitable for assessment scale only

---

## Future Enhancements

### Phase 2: Persistence & Scalability
- **SQL Database Integration** - Azure SQL Database or PostgreSQL for booking records
- **Caching Layer** - Redis for vehicle inventory and pricing
- **Asynchronous Processing** - Background jobs for booking confirmations

### Phase 3: Advanced Features
- **Authentication & Authorization** - OAuth2, JWT tokens
- **Multi-User Support** - User profiles, booking history, preferences
- **Payment Processing** - Integration with payment gateways
- **Notification System** - Email/SMS confirmations and reminders
- **Analytics & Reporting** - Usage patterns, pricing trends

### Phase 4: Provider Ecosystem
- **Third & Fourth Providers** - New rental companies with custom logic
- **Dynamic Pricing Models** - Real-time rate adjustments
- **Promotional Codes** - Discount and coupon handling
- **Insurance Comparisons** - Side-by-side coverage analysis

### Phase 5: User Experience
- **Advanced Search Filters** - Transmission type, mileage allowance, age restrictions
- **Saved Searches** - User-defined alerts for price changes
- **Loyalty Integration** - Provider rewards program integration
- **Mobile App** - Native iOS/Android application

---

## Project Goals

✓ Demonstrate architectural patterns suitable for enterprise .NET applications  
✓ Show separation of concerns across layered architecture  
✓ Prove extensibility for adding new providers without modifying existing code  
✓ Implement comprehensive validation and error handling  
✓ Create clear contracts between layers using interfaces and DTOs  
✓ Build well-structured tests demonstrating business logic correctness  

---

**Status:** Architecture & Specification Phase (Phase 1A)  
**Next Step:** Implementation begins upon approval of spec.md
