# Car Rental Availability System - Technical Specification

## 1. Problem Statement

Travellers require a unified interface to search and book rental vehicles across competing providers that operate with fundamentally different business models, pricing strategies, and availability rules.

**Current State:**

- Each provider operates independently with distinct APIs
- Pricing calculations differ (flat vs. dynamic, weekend surcharges)
- Availability rules vary (always available vs. selective)
- Insurance options and cancellation policies are inconsistent
- Users must query multiple providers manually to compare options

**Desired State:**

- Single search endpoint aggregates results from multiple providers
- Unified response format normalizes provider-specific quirks
- Booking validation accounts for location-specific document requirements
- Pricing comparisons are accurate and transparent
- System easily extends to support additional providers

---

## 2. Functional Requirements

### 2.1 Search API

| Requirement | Description                                                                              |
| ----------- | ---------------------------------------------------------------------------------------- |
| FR-2.1.1    | Accept search parameters: pickup location, from date, to date, optional vehicle category |
| FR-2.1.2    | Query both PremiumDrive and BudgetWheels simultaneously or sequentially                  |
| FR-2.1.3    | Filter results to include only vehicles matching the requested category (if provided)    |
| FR-2.1.4    | Calculate total rental price for each vehicle from each provider                         |
| FR-2.1.5    | Exclude vehicles marked as unavailable by the provider                                   |
| FR-2.1.6    | Normalize provider responses into unified model                                          |
| FR-2.1.7    | Sort results by total price (ascending)                                                  |
| FR-2.1.8    | Return array of aggregated results with provider-specific details                        |

### 2.2 Booking API

| Requirement | Description                                                                                         |
| ----------- | --------------------------------------------------------------------------------------------------- |
| FR-2.2.1    | Accept booking parameters: driver name, document type, document number, vehicle ID, pickup location |
| FR-2.2.2    | Validate document requirements based on pickup location (domestic vs. international)                |
| FR-2.2.3    | Reject invalid document/location combinations with HTTP 422                                         |
| FR-2.2.4    | Store booking confirmation with auto-generated reference number                                     |
| FR-2.2.5    | Return booking confirmation including reference, dates, location, and total price                   |

### 2.3 Booking Lookup API

| Requirement | Description                                       |
| ----------- | ------------------------------------------------- |
| FR-2.3.1    | Accept booking reference number as path parameter |
| FR-2.3.2    | Retrieve previously stored booking by reference   |
| FR-2.3.3    | Return full booking confirmation details          |
| FR-2.3.4    | Return 404 if booking reference not found         |

---

## 3. Non-Functional Requirements

| Requirement | Description                                                                          |
| ----------- | ------------------------------------------------------------------------------------ |
| NFR-3.1     | **Extensibility** - Support additional providers with zero changes to endpoint code  |
| NFR-3.2     | **Maintainability** - Clear separation of concerns across layers                     |
| NFR-3.3     | **Testability** - All business logic independently testable via mocking              |
| NFR-3.4     | **Performance** - Search response within 2 seconds for average request               |
| NFR-3.5     | **Reliability** - Handle provider failures gracefully without crashing entire system |
| NFR-3.6     | **Scalability** - In-memory Phase 1; designed for database migration in Phase 2      |
| NFR-3.7     | **Documentation** - All public interfaces and DTOs clearly documented                |
| NFR-3.8     | **Error Handling** - Consistent error responses across all endpoints                 |

---

## 4. Business Rules

### 4.1 PremiumDrive

| Rule     | Description                                                                                       |
| -------- | ------------------------------------------------------------------------------------------------- |
| BR-4.1.1 | **Flat Daily Pricing** - Single daily rate applies every day of rental, regardless of day of week |
| BR-4.1.2 | **Always Available** - PremiumDrive never returns unavailable vehicles                            |
| BR-4.1.3 | **Comprehensive Insurance** - Insurance is included in all quotes; cannot be deselected           |
| BR-4.1.4 | **Free Cancellation** - Bookings can be cancelled up to 48 hours before pickup with full refund   |
| BR-4.1.5 | **Total Price = Daily Rate × Number of Days**                                                     |

### 4.2 BudgetWheels

| Rule     | Description                                                                                           |
| -------- | ----------------------------------------------------------------------------------------------------- |
| BR-4.2.1 | **Base Daily Pricing** - Starting daily rate before surcharges                                        |
| BR-4.2.2 | **Weekend Surcharge** - Friday, Saturday, Sunday nights incur 20% surcharge on base rate              |
| BR-4.2.3 | **Per-Night Calculation** - Price must be calculated night by night, not as daily rate × days         |
| BR-4.2.4 | **Basic Insurance** - Basic insurance is included; no premium options available                       |
| BR-4.2.5 | **Non-Refundable** - All bookings are non-refundable once confirmed                                   |
| BR-4.2.6 | **Selective Availability** - BudgetWheels may return unavailable vehicles; these must be filtered out |
| BR-4.2.7 | **Example Calculation** - 3-day rental Fri-Sun: Price = (Base × 1.2) + (Base × 1.2) + (Base × 1.2)    |

### 4.3 Vehicle Categories

| Category | Use Case                                                    |
| -------- | ----------------------------------------------------------- |
| Economy  | Single passenger, city driving, minimal cargo               |
| Compact  | 1-2 passengers, urban and highway, modest cargo             |
| SUV      | Families, multiple passengers, larger cargo, varied terrain |
| Minivan  | Large families, groups, maximum passenger capacity          |

### 4.4 Location Validation Rules

**Domestic Locations (India)**

- Mumbai
- Bengaluru

**International Locations**

- Dubai
- Singapore
- London

---

## 5. Architecture Overview

### 5.1 Layered Architecture Approach

The system is organized into four distinct layers, each with specific responsibilities and interfaces:

```
┌────────────────────────────────────────────────────┐
│  API Layer (Minimal API Endpoints)                 │
│  - HTTP request/response handling                  │
│  - Parameter binding and validation                │
│  - Status code decisions                           │
└────────────────────────────────────────────────────┘
                         ↓
┌────────────────────────────────────────────────────┐
│  Application Service Layer                         │
│  - Search orchestration                            │
│  - Booking creation and validation                 │
│  - Booking retrieval                               │
│  - Provider delegation                             │
│  - Result aggregation and sorting                  │
└────────────────────────────────────────────────────┘
                         ↓
┌────────────────────────────────────────────────────┐
│  Domain Service Layer                              │
│  - Pricing calculations                            │
│  - Validation rules enforcement                    │
│  - Business logic implementation                   │
│  - Entity transformations                          │
└────────────────────────────────────────────────────┘
                         ↓
┌────────────────────────────────────────────────────┐
│  Infrastructure Layer                              │
│  - Provider client implementations                 │
│  - Booking storage/retrieval                       │
│  - External system integration                     │
└────────────────────────────────────────────────────┘
```

### 5.2 Provider Abstraction Pattern

The architecture treats providers as interchangeable implementations of a common contract:

```
IProviderClient (Interface)
    ↑
    ├─ PremiumDriveClient (Concrete Implementation)
    └─ BudgetWheelsClient (Concrete Implementation)

Additional providers follow the same pattern without modifying existing code.
```

### 5.3 Data Flow: Search Request

1. **Endpoint** receives HTTP GET with search parameters
2. **Application Service** validates search criteria
3. **Application Service** concurrently calls multiple **Provider Clients**
4. Each **Provider Client** returns provider-specific vehicle quotes
5. **Pricing Service** calculates total prices according to provider rules
6. **Application Service** filters unavailable vehicles
7. **Application Service** normalizes results into common DTO
8. **Application Service** sorts by price
9. **Endpoint** returns unified response

### 5.4 Data Flow: Booking Request

1. **Endpoint** receives HTTP POST with booking details
2. **Validation Service** checks document type matches location rules
3. **Validation Service** rejects with 422 if invalid
4. **Booking Service** generates reference number
5. **Booking Service** stores booking in **Booking Store**
6. **Endpoint** returns booking confirmation

---

## 6. Validation Rules

### 6.1 Search Validation

| Rule                     | Validation                         | Response        |
| ------------------------ | ---------------------------------- | --------------- |
| Pickup location required | `pickup` must be provided          | 400 Bad Request |
| Valid pickup location    | `pickup` must be in supported list | 400 Bad Request |
| From date required       | `from` must be provided            | 400 Bad Request |
| Valid from date format   | `from` must be ISO 8601            | 400 Bad Request |
| To date required         | `to` must be provided              | 400 Bad Request |
| Valid to date format     | `to` must be ISO 8601              | 400 Bad Request |
| Date range validity      | `to` must be > `from`              | 400 Bad Request |
| Minimum rental period    | At least 1 day rental required     | 400 Bad Request |
| Category format          | If provided, must match enum       | 400 Bad Request |

**Supported Locations:**

- Domestic: Mumbai, Bengaluru
- International: Dubai, Singapore, London

### 6.2 Booking Validation

| Rule                     | Validation                                  | Response                 |
| ------------------------ | ------------------------------------------- | ------------------------ |
| Driver name required     | `driverName` must be provided               | 400 Bad Request          |
| Driver name format       | Non-empty string, no special chars          | 400 Bad Request          |
| Document type required   | Must be provided                            | 400 Bad Request          |
| Valid document type      | Must be "NationalId" or "Passport"          | 400 Bad Request          |
| Document number required | Must be provided                            | 400 Bad Request          |
| Document number format   | Non-empty string                            | 400 Bad Request          |
| Vehicle ID required      | Must be provided                            | 400 Bad Request          |
| Valid vehicle ID format  | Must be valid GUID                          | 400 Bad Request          |
| Pickup location required | Must be provided                            | 400 Bad Request          |
| Valid pickup location    | Must be in supported list                   | 400 Bad Request          |
| **Domestic rule**        | Mumbai/Bengaluru + NationalId OR Passport   | 422 Unprocessable Entity |
| **International rule**   | Dubai/Singapore/London ONLY accept Passport | 422 Unprocessable Entity |

**Document/Location Matrix:**

| Location  | NationalId | Passport |
| --------- | :--------: | :------: |
| Mumbai    |     ✓      |    ✓     |
| Bengaluru |     ✓      |    ✓     |
| Dubai     |     ✗      |    ✓     |
| Singapore |     ✗      |    ✓     |
| London    |     ✗      |    ✓     |

### 6.3 Booking Lookup Validation

| Rule               | Validation                        | Response        |
| ------------------ | --------------------------------- | --------------- |
| Reference required | Must be provided in path          | 400 Bad Request |
| Reference format   | Must match BK-YYYY-XXXXXX pattern | 400 Bad Request |
| Reference exists   | Must exist in booking store       | 404 Not Found   |

---

## 7. Provider Comparison

| Aspect                 | PremiumDrive                                    | BudgetWheels                                              |
| ---------------------- | ----------------------------------------------- | --------------------------------------------------------- |
| **Pricing Model**      | Flat daily rate                                 | Base rate + weekend surcharge                             |
| **Calculation Method** | Daily rate × days                               | Night-by-night calculation                                |
| **Weekend Surcharge**  | None                                            | Friday, Saturday, Sunday +20%                             |
| **Availability**       | Always available                                | Selective availability                                    |
| **Insurance**          | Comprehensive (included)                        | Basic (included)                                          |
| **Cancellation**       | Free up to 48h before pickup                    | Non-refundable                                            |
| **Refund Policy**      | Refundable                                      | Non-refundable                                            |
| **Best For**           | Travelers wanting flexibility and full coverage | Budget-conscious travelers accepting non-refundable terms |

### 7.1 Pricing Example

**Scenario:** 3-night rental Friday 23:00 - Monday 11:00

**PremiumDrive:**

- Daily rate: INR 2,000
- Total = 2,000 × 3 = **INR 6,000**

**BudgetWheels:**

- Base rate: INR 1,500
- Friday night: 1,500 × 1.2 = 1,800
- Saturday night: 1,500 × 1.2 = 1,800
- Sunday night: 1,500 × 1.2 = 1,800
- Total = 1,800 + 1,800 + 1,800 = **INR 5,400**

---

## 8. Pricing Rules

  ### 8.1 PremiumDrive

  **Pricing Formula**

  ```
  Total Price = Daily Rate × Number of Rental Days
  ```

  **Example**

  Rental Period: Friday to Monday (3 days)

  - Daily Rate: INR 2,000

  | Day      |     Price |
  | -------- | --------: |
  | Friday   | INR 2,000 |
  | Saturday | INR 2,000 |
  | Sunday   | INR 2,000 |

  **Total:** INR 6,000

  ---

  ### 8.2 BudgetWheels

  **Pricing Formula**

  ```
  Weekday Night = Base Rate
  Weekend Night = Base Rate × 1.2

  Total Price = Sum of All Nightly Charges
  ```

  **Example**

  Rental Period: Friday to Monday (3 nights)

  - Base Rate: INR 1,500

  | Night    |     Price |
  | -------- | --------: |
  | Friday   | INR 1,800 |
  | Saturday | INR 1,800 |
  | Sunday   | INR 1,800 |

  **Total:** INR 5,400

## 9. Interface Contracts

| Interface          | Responsibility                              |
| ------------------ | ------------------------------------------- |
| IProviderClient    | Communicates with external rental providers |
| IBookingService    | Creates and retrieves bookings              |
| ISearchService     | Aggregates provider search results          |
| IPricingService    | Calculates provider-specific pricing        |
| IValidationService | Applies business validation rules           |
| IBookingStore      | Persists booking information                |

## 10. Design Decisions

### 10.1 Extensibility Through Abstraction

**Decision:** All external provider communication flows through IProviderClient interface.

**Rationale:**

1. **Provider Isolation** - Each provider's logic is contained in its own class
2. **Easy Addition** - New provider is added by creating new class implementing interface
3. **Zero Changes** - SearchService doesn't change when new provider is added
4. **Testability** - IProviderClient can be mocked for unit testing
5. **Compliance** - Follows Dependency Inversion Principle

**Extension Example:**
To add Provider3 with custom pricing:

1. Create `Provider3Client : IProviderClient`
2. Register in dependency injection: `services.AddScoped<IProviderClient, Provider3Client>()`
3. SearchService automatically queries Provider3
4. Zero changes to endpoints, services, or validation logic

### 10.2 Pricing as Pluggable Service

**Decision:** IPricingService handles all pricing calculations with provider type parameter.

**Rationale:**

1. **Single Responsibility** - Pricing logic isolated from orchestration
2. **Provider Flexibility** - Different formulas handled in one place
3. **Testability** - Pricing rules tested independently
4. **Maintainability** - Changes to formulas contained to one service
5. **Reusability** - Same service used by search and booking flows

### 10.3 Validation Centralization

**Decision:** All validation flows through IValidationService.

**Rationale:**

1. **Consistency** - Same rules applied across all endpoints
2. **Non-Throwing** - Returns structured ValidationResult instead of exceptions
3. **Separation** - Endpoints don't contain business rules
4. **Evolution** - Rules can change without endpoint code changes
5. **Testing** - Rules tested independently with clear input/output

### 10.4 DTO Separation

**Decision:** Request and response DTOs distinct from domain entities.

**Rationale:**

1. **API Contract** - DTOs define HTTP contract independent of domain
2. **Transformation** - Mapping layer between HTTP and domain logic
3. **Future Flexibility** - Database schema can differ from API response
4. **Versioning** - API contracts can evolve separately from domain
5. **Security** - Sensitive domain properties never exposed in HTTP response

### 10.5 In-Memory Storage with Repository Interface

**Decision:** IBookingStore abstracts storage layer; Phase 1 uses in-memory; Phase 2 uses database.

**Rationale:**

1. **Database Agnostic** - No SQL-specific code in services
2. **Migration Path** - Swap InMemoryBookingStore with DatabaseBookingStore
3. **Testing** - Mock IBookingStore for unit tests
4. **SOLID** - Dependency Inversion Principle
5. **Scalability** - Application layer unchanged when storage changes

### 10.6 Minimal API Thin Endpoints

**Decision:** Endpoints delegate immediately to services; no business logic in endpoints.

**Rationale:**

1. **Testability** - Business logic tested through services, not HTTP mocks
2. **Reusability** - Services can be called from other endpoints, background jobs
3. **Clarity** - Endpoints clearly show which service owns which logic
4. **Maintainability** - Changes to business logic don't touch endpoints

---

## 11. Error Handling Strategy
---

400 - Validation error

404 - Resource not found

422 - Business rule violation

500 - Unexpected server 

---
## 12. Repository Structure
---
```
src/
├── CarRental.Api
├── Providers
├── Services
├── Models
├── Validators

tests/
├── CarRental.Tests

car-rental-ui/
```

---

## 13. Assumptions
1. **Single Instance** - The application runs as a single instance; bookings are stored in memory for this assignment.

2. **No Authentication** - Authentication and authorization are outside the scope of this assignment; all endpoints are publicly accessible.

3. **Provider Availability** - Rental providers are assumed to be available and return valid responses during search operations.

4. **Small Dataset** - In-memory storage is sufficient for the expected number of vehicles and bookings in this implementation.

5. **Globally Unique Vehicle IDs** - Vehicle identifiers are assumed to be unique across all providers.

---
