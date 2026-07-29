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

| Requirement | Description |
|------------|-------------|
| FR-2.1.1 | Accept search parameters: pickup location, from date, to date, optional vehicle category |
| FR-2.1.2 | Query both PremiumDrive and BudgetWheels simultaneously or sequentially |
| FR-2.1.3 | Filter results to include only vehicles matching the requested category (if provided) |
| FR-2.1.4 | Calculate total rental price for each vehicle from each provider |
| FR-2.1.5 | Exclude vehicles marked as unavailable by the provider |
| FR-2.1.6 | Normalize provider responses into unified model |
| FR-2.1.7 | Sort results by total price (ascending) |
| FR-2.1.8 | Return array of aggregated results with provider-specific details |

### 2.2 Booking API

| Requirement | Description |
|------------|-------------|
| FR-2.2.1 | Accept booking parameters: driver name, document type, document number, vehicle ID, pickup location |
| FR-2.2.2 | Validate document requirements based on pickup location (domestic vs. international) |
| FR-2.2.3 | Reject invalid document/location combinations with HTTP 422 |
| FR-2.2.4 | Store booking confirmation with auto-generated reference number |
| FR-2.2.5 | Return booking confirmation including reference, dates, location, and total price |

### 2.3 Booking Lookup API

| Requirement | Description |
|------------|-------------|
| FR-2.3.1 | Accept booking reference number as path parameter |
| FR-2.3.2 | Retrieve previously stored booking by reference |
| FR-2.3.3 | Return full booking confirmation details |
| FR-2.3.4 | Return 404 if booking reference not found |

---

## 3. Non-Functional Requirements

| Requirement | Description |
|------------|-------------|
| NFR-3.1 | **Extensibility** - Support additional providers with zero changes to endpoint code |
| NFR-3.2 | **Maintainability** - Clear separation of concerns across layers |
| NFR-3.3 | **Testability** - All business logic independently testable via mocking |
| NFR-3.4 | **Performance** - Search response within 2 seconds for average request |
| NFR-3.5 | **Reliability** - Handle provider failures gracefully without crashing entire system |
| NFR-3.6 | **Scalability** - In-memory Phase 1; designed for database migration in Phase 2 |
| NFR-3.7 | **Documentation** - All public interfaces and DTOs clearly documented |
| NFR-3.8 | **Error Handling** - Consistent error responses across all endpoints |

---

## 4. Business Rules

### 4.1 PremiumDrive

| Rule | Description |
|------|-------------|
| BR-4.1.1 | **Flat Daily Pricing** - Single daily rate applies every day of rental, regardless of day of week |
| BR-4.1.2 | **Always Available** - PremiumDrive never returns unavailable vehicles |
| BR-4.1.3 | **Comprehensive Insurance** - Insurance is included in all quotes; cannot be deselected |
| BR-4.1.4 | **Free Cancellation** - Bookings can be cancelled up to 48 hours before pickup with full refund |
| BR-4.1.5 | **Total Price = Daily Rate × Number of Days** |

### 4.2 BudgetWheels

| Rule | Description |
|------|-------------|
| BR-4.2.1 | **Base Daily Pricing** - Starting daily rate before surcharges |
| BR-4.2.2 | **Weekend Surcharge** - Friday, Saturday, Sunday nights incur 20% surcharge on base rate |
| BR-4.2.3 | **Per-Night Calculation** - Price must be calculated night by night, not as daily rate × days |
| BR-4.2.4 | **Basic Insurance** - Basic insurance is included; no premium options available |
| BR-4.2.5 | **Non-Refundable** - All bookings are non-refundable once confirmed |
| BR-4.2.6 | **Selective Availability** - BudgetWheels may return unavailable vehicles; these must be filtered out |
| BR-4.2.7 | **Example Calculation** - 3-day rental Fri-Sun: Price = (Base × 1.2) + (Base × 1.2) + (Base × 1.2) |

### 4.3 Vehicle Categories

| Category | Use Case |
|----------|----------|
| Economy | Single passenger, city driving, minimal cargo |
| Compact | 1-2 passengers, urban and highway, modest cargo |
| SUV | Families, multiple passengers, larger cargo, varied terrain |
| Minivan | Large families, groups, maximum passenger capacity |

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

## 6. Domain Model

### 6.1 Core Entities

#### Vehicle
**Purpose:** Represents a rental vehicle in the system.

**Properties:**
- `VehicleId` - Unique identifier across all providers
- `Provider` - Which provider offers this vehicle (PremiumDrive, BudgetWheels)
- `Category` - Vehicle category (Economy, Compact, SUV, Minivan)
- `Make` - Vehicle manufacturer
- `Model` - Vehicle model name
- `Year` - Manufacturing year
- `IsAvailable` - Whether vehicle is available for the requested dates
- `ProviderReference` - Provider's internal vehicle identifier

#### SearchCriteria
**Purpose:** Encapsulates a search request with validated input.

**Properties:**
- `PickupLocation` - Location where vehicle is rented
- `FromDate` - Rental start date
- `ToDate` - Rental end date
- `Category` - Optional vehicle category filter
- `DaysCount` - Calculated number of rental days

#### Quote
**Purpose:** Represents a single rental option from a provider.

**Properties:**
- `VehicleId` - Reference to vehicle
- `Provider` - Source provider
- `Category` - Vehicle category
- `DailyRate` - Per-day base rate
- `TotalPrice` - Calculated total for rental period
- `PriceBreakdown` - Day-by-day pricing details
- `InsuranceType` - Type of insurance (Comprehensive, Basic)
- `CancellationPolicy` - Refund terms
- `AvailabilityStatus` - Available or reason for unavailability

#### Booking
**Purpose:** Represents a confirmed car rental reservation.

**Properties:**
- `ReferenceNumber` - Unique booking identifier
- `DriverName` - Name of primary driver
- `DocumentType` - Type of travel document (NationalId, Passport)
- `DocumentNumber` - Document identifier
- `VehicleId` - Which vehicle is booked
- `Provider` - Which provider is fulfilling the booking
- `PickupLocation` - Rental location
- `FromDate` - Rental start
- `ToDate` - Rental end
- `TotalPrice` - Final quoted price
- `BookingDate` - When booking was created
- `InsuranceType` - Insurance selected
- `CancellationPolicy` - Applicable cancellation terms

### 6.2 Value Objects

#### Location
**Purpose:** Represents a geographic pickup location with validation.

**Properties:**
- `City` - City name
- `Country` - Country code or name
- `IsInternational` - Boolean flag: true if international

#### DateRange
**Purpose:** Encapsulates rental period validation.

**Properties:**
- `FromDate` - Start date
- `ToDate` - End date
- `DaysCount` - Number of days
- `NightsCount` - Number of nights

#### Money
**Purpose:** Represents currency values with rounding rules.

**Properties:**
- `Amount` - Decimal value
- `Currency` - Currency code (e.g., INR, AED, GBP)

#### DocumentInfo
**Purpose:** Encapsulates travel document validation.

**Properties:**
- `DocumentType` - National ID or Passport
- `DocumentNumber` - Identifier value
- `IsValid` - Whether document is valid for location

### 6.3 Enumerations

**VehicleCategory**
- Economy
- Compact
- SUV
- Minivan

**DocumentType**
- NationalId
- Passport

**InsuranceType**
- Comprehensive
- Basic

**ProviderName**
- PremiumDrive
- BudgetWheels

---

## 7. DTO Definitions

### 7.1 Search Request DTO

**Endpoint:** `GET /cars/search`

**Request Parameters:**
- `pickup` - City name (string, required)
- `from` - Start date (ISO 8601 format, required)
- `to` - End date (ISO 8601 format, required)
- `category` - Vehicle category filter (string, optional)

**Example Query:**
```
GET /cars/search?pickup=Mumbai&from=2024-08-15&to=2024-08-18&category=SUV
```

### 7.2 Search Response DTO

**Properties:**
- `SearchId` - Unique identifier for this search
- `PickupLocation` - Requested location
- `FromDate` - Requested start date
- `ToDate` - Requested end date
- `DaysCount` - Number of days
- `Results` - Array of AggregatedQuote objects

**AggregatedQuote Properties:**
- `VehicleId` - Unique identifier
- `Provider` - Provider name
- `Category` - Vehicle category
- `Make` - Manufacturer
- `Model` - Model name
- `DailyRate` - Base daily rate
- `TotalPrice` - Total price for rental period
- `PriceBreakdown` - Object with daily prices
- `InsuranceType` - Comprehensive or Basic
- `CancellationPolicy` - Refund policy description
- `IsAvailable` - Availability status

### 7.3 Booking Request DTO

**Endpoint:** `POST /cars/book`

**Request Body Properties:**
- `DriverName` - Full name of driver (string, required)
- `DocumentType` - "NationalId" or "Passport" (string, required)
- `DocumentNumber` - ID or passport number (string, required)
- `VehicleId` - Vehicle to book (GUID, required)
- `PickupLocation` - Rental location (string, required)

**Example Body:**
```json
{
  "driverName": "Rajesh Kumar",
  "documentType": "NationalId",
  "documentNumber": "1234567890123",
  "vehicleId": "f47ac10b-58cc-4372-a567-0e02b2c3d479",
  "pickupLocation": "Mumbai"
}
```

### 7.4 Booking Response DTO

**Properties:**
- `ReferenceNumber` - Booking confirmation code (string)
- `DriverName` - Driver name from request
- `VehicleCategory` - Category of booked vehicle
- `VehicleDetails` - Make, model, year
- `Provider` - Which provider fulfills booking
- `PickupLocation` - Rental location
- `FromDate` - Rental start date
- `ToDate` - Rental end date
- `DaysCount` - Number of days
- `DailyRate` - Per-day price
- `TotalPrice` - Total booking amount
- `InsuranceType` - Insurance included
- `CancellationPolicy` - Refund policy
- `BookingConfirmedAt` - Timestamp

### 7.5 Booking Lookup Response DTO

**Endpoint:** `GET /cars/booking/{reference}`

**Response Properties:**
- Same as Booking Response DTO

### 7.6 Error Response DTO

**Properties:**
- `ErrorCode` - Machine-readable code (e.g., "INVALID_DOCUMENT")
- `Message` - Human-readable description
- `Details` - Additional context (object, optional)
- `Timestamp` - When error occurred
- `RequestId` - For tracing (correlation ID)

---

## 8. API Contracts

### 8.1 Search Endpoint

| Property | Value |
|----------|-------|
| **Endpoint** | `/cars/search` |
| **HTTP Method** | `GET` |
| **Authentication** | None |
| **Rate Limiting** | None |

**Request Parameters:**
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `pickup` | string | Yes | City name (Mumbai, Bengaluru, Dubai, Singapore, London) |
| `from` | date | Yes | Rental start date (ISO 8601: YYYY-MM-DD) |
| `to` | date | Yes | Rental end date (ISO 8601: YYYY-MM-DD) |
| `category` | string | No | Vehicle category (Economy, Compact, SUV, Minivan) |

**Success Response (200 OK):**
```
Content-Type: application/json

{
  "searchId": "uuid",
  "pickupLocation": "Mumbai",
  "fromDate": "2024-08-15",
  "toDate": "2024-08-18",
  "daysCount": 3,
  "results": [
    {
      "vehicleId": "uuid",
      "provider": "PremiumDrive",
      "category": "Economy",
      "make": "Maruti",
      "model": "Swift",
      "dailyRate": 2000,
      "totalPrice": 6000,
      "priceBreakdown": {
        "day1": 2000,
        "day2": 2000,
        "day3": 2000
      },
      "insuranceType": "Comprehensive",
      "cancellationPolicy": "Free cancellation up to 48 hours",
      "isAvailable": true
    },
    {
      "vehicleId": "uuid",
      "provider": "BudgetWheels",
      "category": "Economy",
      "make": "Hyundai",
      "model": "i10",
      "dailyRate": 1500,
      "totalPrice": 5400,
      "priceBreakdown": {
        "friday": 1800,
        "saturday": 1800,
        "sunday": 1800
      },
      "insuranceType": "Basic",
      "cancellationPolicy": "Non-refundable",
      "isAvailable": true
    }
  ]
}
```

**Error Responses:**

| Status | Code | Description |
|--------|------|-------------|
| 400 | INVALID_PARAMETERS | Missing or malformed parameters |
| 400 | INVALID_DATE_RANGE | End date before start date |
| 400 | INVALID_LOCATION | Location not supported |
| 400 | INVALID_CATEGORY | Category not recognized |
| 500 | INTERNAL_ERROR | Unexpected server error |

---

### 8.2 Booking Endpoint

| Property | Value |
|----------|-------|
| **Endpoint** | `/cars/book` |
| **HTTP Method** | `POST` |
| **Content-Type** | `application/json` |
| **Authentication** | None |

**Request Body:**
```json
{
  "driverName": "string (required)",
  "documentType": "string (required: NationalId | Passport)",
  "documentNumber": "string (required)",
  "vehicleId": "uuid (required)",
  "pickupLocation": "string (required)"
}
```

**Success Response (201 Created):**
```
Content-Type: application/json
Location: /cars/booking/{reference}

{
  "referenceNumber": "BK-2024-000123",
  "driverName": "Rajesh Kumar",
  "vehicleCategory": "Economy",
  "vehicleDetails": {
    "make": "Maruti",
    "model": "Swift",
    "year": 2024
  },
  "provider": "BudgetWheels",
  "pickupLocation": "Mumbai",
  "fromDate": "2024-08-15",
  "toDate": "2024-08-18",
  "daysCount": 3,
  "dailyRate": 1500,
  "totalPrice": 5400,
  "insuranceType": "Basic",
  "cancellationPolicy": "Non-refundable",
  "bookingConfirmedAt": "2024-08-01T10:30:00Z"
}
```

**Error Responses:**

| Status | Code | Description |
|--------|------|-------------|
| 400 | MISSING_FIELDS | Required fields missing |
| 400 | INVALID_VEHICLE | Vehicle ID not found |
| 422 | INVALID_DOCUMENT | Document type invalid for location |
| 422 | INVALID_LOCATION | Location invalid for document type |
| 404 | VEHICLE_NOT_AVAILABLE | Vehicle no longer available |
| 500 | INTERNAL_ERROR | Unexpected server error |

---

### 8.3 Booking Lookup Endpoint

| Property | Value |
|----------|-------|
| **Endpoint** | `/cars/booking/{reference}` |
| **HTTP Method** | `GET` |
| **Authentication** | None |

**URL Parameters:**
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `reference` | string | Yes | Booking reference number (BK-YYYY-XXXXXX) |

**Success Response (200 OK):**
```
Content-Type: application/json

{
  "referenceNumber": "BK-2024-000123",
  "driverName": "Rajesh Kumar",
  "vehicleCategory": "Economy",
  "vehicleDetails": {
    "make": "Maruti",
    "model": "Swift",
    "year": 2024
  },
  "provider": "BudgetWheels",
  "pickupLocation": "Mumbai",
  "fromDate": "2024-08-15",
  "toDate": "2024-08-18",
  "daysCount": 3,
  "dailyRate": 1500,
  "totalPrice": 5400,
  "insuranceType": "Basic",
  "cancellationPolicy": "Non-refundable",
  "bookingConfirmedAt": "2024-08-01T10:30:00Z"
}
```

**Error Responses:**

| Status | Code | Description |
|--------|------|-------------|
| 404 | BOOKING_NOT_FOUND | Reference number does not exist |
| 400 | INVALID_REFERENCE_FORMAT | Reference format is invalid |
| 500 | INTERNAL_ERROR | Unexpected server error |

---

## 9. Validation Rules

### 9.1 Search Validation

| Rule | Validation | Response |
|------|-----------|----------|
| Pickup location required | `pickup` must be provided | 400 Bad Request |
| Valid pickup location | `pickup` must be in supported list | 400 Bad Request |
| From date required | `from` must be provided | 400 Bad Request |
| Valid from date format | `from` must be ISO 8601 | 400 Bad Request |
| To date required | `to` must be provided | 400 Bad Request |
| Valid to date format | `to` must be ISO 8601 | 400 Bad Request |
| Date range validity | `to` must be > `from` | 400 Bad Request |
| Minimum rental period | At least 1 day rental required | 400 Bad Request |
| Category format | If provided, must match enum | 400 Bad Request |

**Supported Locations:**
- Domestic: Mumbai, Bengaluru
- International: Dubai, Singapore, London

### 9.2 Booking Validation

| Rule | Validation | Response |
|------|-----------|----------|
| Driver name required | `driverName` must be provided | 400 Bad Request |
| Driver name format | Non-empty string, no special chars | 400 Bad Request |
| Document type required | Must be provided | 400 Bad Request |
| Valid document type | Must be "NationalId" or "Passport" | 400 Bad Request |
| Document number required | Must be provided | 400 Bad Request |
| Document number format | Non-empty string | 400 Bad Request |
| Vehicle ID required | Must be provided | 400 Bad Request |
| Valid vehicle ID format | Must be valid GUID | 400 Bad Request |
| Pickup location required | Must be provided | 400 Bad Request |
| Valid pickup location | Must be in supported list | 400 Bad Request |
| **Domestic rule** | Mumbai/Bengaluru + NationalId OR Passport | 422 Unprocessable Entity |
| **International rule** | Dubai/Singapore/London ONLY accept Passport | 422 Unprocessable Entity |

**Document/Location Matrix:**

| Location | NationalId | Passport |
|----------|:----------:|:--------:|
| Mumbai | ✓ | ✓ |
| Bengaluru | ✓ | ✓ |
| Dubai | ✗ | ✓ |
| Singapore | ✗ | ✓ |
| London | ✗ | ✓ |

### 9.3 Booking Lookup Validation

| Rule | Validation | Response |
|------|-----------|----------|
| Reference required | Must be provided in path | 400 Bad Request |
| Reference format | Must match BK-YYYY-XXXXXX pattern | 400 Bad Request |
| Reference exists | Must exist in booking store | 404 Not Found |

---

## 10. Provider Comparison

| Aspect | PremiumDrive | BudgetWheels |
|--------|--------------|--------------|
| **Pricing Model** | Flat daily rate | Base rate + weekend surcharge |
| **Calculation Method** | Daily rate × days | Night-by-night calculation |
| **Weekend Surcharge** | None | Friday, Saturday, Sunday +20% |
| **Availability** | Always available | Selective availability |
| **Insurance** | Comprehensive (included) | Basic (included) |
| **Cancellation** | Free up to 48h before pickup | Non-refundable |
| **Refund Policy** | Refundable | Non-refundable |
| **Best For** | Travelers wanting flexibility and full coverage | Budget-conscious travelers accepting non-refundable terms |

### 10.1 Pricing Example

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

## 11. Pricing Rules

### 11.1 PremiumDrive Pricing

**Model:** Flat daily rate, all days identical

**Formula:**
```
Total Price = Daily Rate × Number of Days
```

**Characteristics:**
- Same rate every day (Monday-Sunday)
- No weekend premiums
- Simple, predictable pricing
- Comprehensive insurance included

**Example:**
```
From: Friday, August 15
To: Monday, August 18
Days: 3
Daily Rate: INR 2,000

Calculation:
Day 1 (Friday): 2,000
Day 2 (Saturday): 2,000
Day 3 (Sunday): 2,000
Total: 6,000
```

### 11.2 BudgetWheels Pricing

**Model:** Base daily rate with weekend surcharge applied nightly

**Formula:**
```
For each night:
  if night is Friday, Saturday, or Sunday:
    Night Price = Base Rate × 1.2
  else:
    Night Price = Base Rate

Total Price = Sum of all night prices
```

**Characteristics:**
- Base rate for weekday nights (Monday-Thursday)
- 20% surcharge for Friday, Saturday, Sunday nights
- Must calculate night-by-night, not day × days
- Basic insurance included

**Critical Implementation Detail:**
The surcharge applies to the **night**, not the day. A Friday pickup means Friday night carries the surcharge.

**Example 1: Friday-Sunday (3 nights, all weekend)**
```
From: Friday, August 15 (17:00)
To: Monday, August 18 (11:00)
Base Rate: INR 1,500

Night 1 (Fri-Sat night): 1,500 × 1.2 = 1,800
Night 2 (Sat-Sun night): 1,500 × 1.2 = 1,800
Night 3 (Sun-Mon night): 1,500 × 1.2 = 1,800
Total: 5,400
```

**Example 2: Wednesday-Saturday (4 nights, mixed)**
```
From: Wednesday, August 13 (17:00)
To: Sunday, August 17 (11:00)
Base Rate: INR 1,500

Night 1 (Wed-Thu night): 1,500 × 1.0 = 1,500
Night 2 (Thu-Fri night): 1,500 × 1.0 = 1,500
Night 3 (Fri-Sat night): 1,500 × 1.2 = 1,800
Night 4 (Sat-Sun night): 1,500 × 1.2 = 1,800
Total: 6,600
```

**Example 3: Tuesday-Thursday (3 nights, all weekday)**
```
From: Tuesday, August 12 (17:00)
To: Friday, August 15 (11:00)
Base Rate: INR 1,500

Night 1 (Tue-Wed night): 1,500 × 1.0 = 1,500
Night 2 (Wed-Thu night): 1,500 × 1.0 = 1,500
Night 3 (Thu-Fri night): 1,500 × 1.0 = 1,500
Total: 4,500
```

### 11.3 Pricing Service Responsibilities

The pricing service must:
1. Accept provider type, base rate, and date range
2. Determine the number of nights
3. For each night, determine day-of-week
4. Apply appropriate surcharge formula
5. Sum nightly prices
6. Return breakdown (night-by-night) and total
7. Handle edge cases (midnight transitions, DST)

---

## 12. Interface Contracts

### 12.1 IProviderClient

**Responsibility:** Encapsulate communication with external rental provider APIs.

**Purpose:**
- Abstract provider-specific HTTP calls and response parsing
- Enable dependency injection and mocking
- Allow new providers to be added by implementing this interface

**Methods:**

**SearchAvailableVehicles**
- Input: SearchCriteria (location, dates, category)
- Output: List of ProviderVehicleQuote
- Responsibility: Query provider API, parse response, return quote objects
- Throws: ProviderException if API call fails

**GetVehicleDetails**
- Input: Vehicle ID
- Output: VehicleDetails
- Responsibility: Retrieve full vehicle specifications
- Throws: ProviderException if vehicle not found

### 12.2 IPricingService

**Responsibility:** Calculate rental prices according to provider-specific rules.

**Purpose:**
- Encapsulate pricing logic away from endpoints
- Support complex calculations (daily rates, surcharges)
- Enable testability of pricing rules

**Methods:**

**CalculateTotalPrice**
- Input: Provider type, base rate, date range
- Output: Money (total) and List of daily breakdowns
- Responsibility: Apply provider's pricing formula correctly
- Throws: InvalidPricingException if calculation fails

**GetPriceBreakdown**
- Input: Provider type, base rate, date range
- Output: Dictionary of date → price
- Responsibility: Return night-by-night or day-by-day prices
- Throws: InvalidPricingException

### 12.3 ISearchService

**Responsibility:** Orchestrate search across multiple providers and aggregate results.

**Purpose:**
- Coordinate provider queries
- Aggregate and normalize results
- Apply filtering and sorting

**Methods:**

**SearchAvailableCars**
- Input: SearchCriteria
- Output: SearchResult with aggregated quotes
- Responsibility: Call all providers, normalize, sort by price, filter unavailable
- Throws: SearchException if fatal error

**FilterByCategory**
- Input: List of quotes, category
- Output: Filtered list
- Responsibility: Remove quotes not matching category
- Returns: Empty list if no matches

### 12.4 IBookingService

**Responsibility:** Create and manage car rental bookings.

**Purpose:**
- Generate booking confirmations
- Persist bookings to storage
- Support booking retrieval

**Methods:**

**CreateBooking**
- Input: BookingRequest (validated)
- Output: BookingConfirmation with reference number
- Responsibility: Generate reference, create entity, store, return DTO
- Throws: BookingException if storage fails

**GetBookingByReference**
- Input: Reference number (string)
- Output: BookingConfirmation or null
- Responsibility: Retrieve from storage, return DTO
- Throws: BookingException if storage fails

### 12.5 IValidationService

**Responsibility:** Enforce all business validation rules.

**Purpose:**
- Centralize validation logic
- Return structured error information
- Support endpoint validation decisions

**Methods:**

**ValidateSearchCriteria**
- Input: SearchCriteria
- Output: ValidationResult (valid: bool, errors: List)
- Responsibility: Check dates, location, category
- Never throws; returns errors in result

**ValidateBookingRequest**
- Input: BookingRequest
- Output: ValidationResult (valid: bool, errors: List)
- Responsibility: Check document type matches location
- Never throws; returns errors in result

**ValidateDocumentForLocation**
- Input: DocumentType, Location
- Output: bool (valid or not)
- Responsibility: Enforce domestic/international rules

### 12.6 IBookingStore

**Responsibility:** Persist and retrieve booking records.

**Purpose:**
- Abstraction for storage backend (in-memory Phase 1, database Phase 2)
- Enable dependency injection

**Methods:**

**SaveBooking**
- Input: Booking entity
- Output: void
- Responsibility: Store booking
- Throws: StorageException if save fails

**GetBookingByReference**
- Input: Reference number
- Output: Booking entity or null
- Responsibility: Retrieve by reference
- Throws: StorageException if retrieval fails

**GetAllBookings**
- Input: none
- Output: List of Booking entities
- Responsibility: Return all stored bookings
- Throws: StorageException

---

## 13. Proposed Components

### 13.1 API Layer Components

#### SearchEndpoint
**Responsibility:** Handle GET /cars/search HTTP requests.

**Concerns:**
- Parameter binding from query string
- Route handling
- Delegate to SearchService
- Return HTTP 200 with aggregated results
- Return HTTP 400 on validation errors

#### BookingEndpoint
**Responsibility:** Handle POST /cars/book HTTP requests.

**Concerns:**
- Request body deserialization
- Route handling
- Delegate to ValidationService
- Delegate to BookingService
- Return HTTP 201 on success with Location header
- Return HTTP 422 on validation errors
- Return HTTP 400 on bad input

#### BookingLookupEndpoint
**Responsibility:** Handle GET /cars/booking/{reference} HTTP requests.

**Concerns:**
- Path parameter extraction
- Route handling
- Delegate to BookingService
- Return HTTP 200 if found
- Return HTTP 404 if not found

### 13.2 Application Service Layer Components

#### SearchService
**Responsibility:** Orchestrate multi-provider search and result aggregation.

**Concerns:**
- Receive SearchCriteria
- Call IProviderClient.SearchAvailableVehicles for each provider
- Call IPricingService to calculate totals
- Filter results by category if provided
- Normalize results to SearchResponse DTO
- Sort by price ascending
- Return results

#### BookingService
**Responsibility:** Create and retrieve booking records.

**Concerns:**
- Validate booking request (delegate to ValidationService)
- Generate booking reference number (format: BK-YYYY-XXXXXX)
- Create Booking entity
- Store to IBookingStore
- Transform to BookingResponse DTO
- Return response with reference

#### ValidationService
**Responsibility:** Enforce all validation rules.

**Concerns:**
- Validate search criteria (dates, location, category)
- Validate booking requests (all fields present)
- Enforce document/location rules (domestic vs. international)
- Return ValidationResult or throw ValidationException

### 13.3 Domain Service Layer Components

#### PricingService
**Responsibility:** Calculate rental prices according to provider rules.

**Concerns:**
- Determine provider (PremiumDrive vs. BudgetWheels)
- Apply provider-specific pricing formula
- Calculate per-day or per-night prices
- Generate price breakdown
- Return Money total and breakdown details

**Premium Drive Logic:**
- Daily rate × number of days
- All days identical price

**BudgetWheels Logic:**
- For each night: determine day-of-week
- Apply 1.2× surcharge for Fri/Sat/Sun
- Sum all nights
- Return day-by-day breakdown

### 13.4 Infrastructure Layer Components

#### PremiumDriveClient
**Responsibility:** Communicate with PremiumDrive provider API.

**Implements:** IProviderClient

**Concerns:**
- Construct API request
- Execute HTTP GET
- Parse JSON response
- Map to ProviderVehicleQuote objects
- Handle API errors gracefully
- Return quotes

#### BudgetWheelsClient
**Responsibility:** Communicate with BudgetWheels provider API.

**Implements:** IProviderClient

**Concerns:**
- Construct API request
- Execute HTTP GET
- Parse JSON response with availability filtering
- Map to ProviderVehicleQuote objects
- Handle API errors gracefully
- Return quotes (excluding unavailable vehicles)

#### InMemoryBookingStore
**Responsibility:** Persist bookings in memory (Phase 1).

**Implements:** IBookingStore

**Concerns:**
- Maintain in-memory collection of Booking objects
- Add new booking on SaveBooking
- Search by reference on GetBookingByReference
- Return all bookings on GetAllBookings
- Return data as-is (no database queries)

---

## 14. Design Decisions

### 14.1 Extensibility Through Abstraction

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

### 14.2 Pricing as Pluggable Service

**Decision:** IPricingService handles all pricing calculations with provider type parameter.

**Rationale:**
1. **Single Responsibility** - Pricing logic isolated from orchestration
2. **Provider Flexibility** - Different formulas handled in one place
3. **Testability** - Pricing rules tested independently
4. **Maintainability** - Changes to formulas contained to one service
5. **Reusability** - Same service used by search and booking flows

### 14.3 Validation Centralization

**Decision:** All validation flows through IValidationService.

**Rationale:**
1. **Consistency** - Same rules applied across all endpoints
2. **Non-Throwing** - Returns structured ValidationResult instead of exceptions
3. **Separation** - Endpoints don't contain business rules
4. **Evolution** - Rules can change without endpoint code changes
5. **Testing** - Rules tested independently with clear input/output

### 14.4 DTO Separation

**Decision:** Request and response DTOs distinct from domain entities.

**Rationale:**
1. **API Contract** - DTOs define HTTP contract independent of domain
2. **Transformation** - Mapping layer between HTTP and domain logic
3. **Future Flexibility** - Database schema can differ from API response
4. **Versioning** - API contracts can evolve separately from domain
5. **Security** - Sensitive domain properties never exposed in HTTP response

### 14.5 In-Memory Storage with Repository Interface

**Decision:** IBookingStore abstracts storage layer; Phase 1 uses in-memory; Phase 2 uses database.

**Rationale:**
1. **Database Agnostic** - No SQL-specific code in services
2. **Migration Path** - Swap InMemoryBookingStore with DatabaseBookingStore
3. **Testing** - Mock IBookingStore for unit tests
4. **SOLID** - Dependency Inversion Principle
5. **Scalability** - Application layer unchanged when storage changes

### 14.6 Minimal API Thin Endpoints

**Decision:** Endpoints delegate immediately to services; no business logic in endpoints.

**Rationale:**
1. **Testability** - Business logic tested through services, not HTTP mocks
2. **Reusability** - Services can be called from other endpoints, background jobs
3. **Clarity** - Endpoints clearly show which service owns which logic
4. **Maintainability** - Changes to business logic don't touch endpoints

---

## 15. Error Handling Strategy

### 15.1 HTTP 400 Bad Request

**When:** Request parameters are invalid, malformed, or missing.

**Causes:**
- Missing required query parameters (pickup, from, to)
- Invalid date format (not ISO 8601)
- Date range invalid (to before from)
- Unrecognized vehicle category
- Missing required JSON fields in POST body
- Invalid JSON syntax
- Invalid GUID format

**Response Structure:**
```json
{
  "errorCode": "INVALID_PARAMETERS",
  "message": "One or more parameters are invalid",
  "details": {
    "fieldName": "error message"
  },
  "timestamp": "2024-08-01T10:30:00Z",
  "requestId": "correlation-id"
}
```

**Examples:**
- Missing `pickup` parameter → MISSING_PICKUP_LOCATION
- Date `from` equals `to` → INVALID_DATE_RANGE
- Category "Motorcycle" → INVALID_VEHICLE_CATEGORY

### 15.2 HTTP 404 Not Found

**When:** Requested resource does not exist.

**Causes:**
- Booking reference number not found in storage
- Vehicle ID does not exist in provider catalog
- Search returns zero results (not an error)

**Response Structure:**
```json
{
  "errorCode": "RESOURCE_NOT_FOUND",
  "message": "The requested resource does not exist",
  "details": {
    "resource": "Booking",
    "identifier": "BK-2024-000123"
  },
  "timestamp": "2024-08-01T10:30:00Z",
  "requestId": "correlation-id"
}
```

**Examples:**
- Booking reference "BK-2024-999999" → BOOKING_NOT_FOUND
- Vehicle ID "00000000-0000-0000-0000-000000000000" → VEHICLE_NOT_FOUND

### 15.3 HTTP 422 Unprocessable Entity

**When:** Request is well-formed but violates business rules.

**Causes:**
- Document type does not match location rules
- NationalId used for international pickup
- Passport not provided for international pickup
- Vehicle no longer available

**Response Structure:**
```json
{
  "errorCode": "VALIDATION_FAILED",
  "message": "Request violates business rules",
  "details": {
    "reason": "INVALID_DOCUMENT_FOR_LOCATION",
    "location": "Dubai",
    "documentType": "NationalId",
    "requirement": "International locations require Passport"
  },
  "timestamp": "2024-08-01T10:30:00Z",
  "requestId": "correlation-id"
}
```

**Error Codes:**
| Code | Reason | Resolution |
|------|--------|-----------|
| INVALID_DOCUMENT_FOR_LOCATION | Document doesn't match location | Use correct document type |
| PASSPORT_REQUIRED | International pickup requires passport | Provide passport |
| DOMESTIC_LOCATION_INVALID_DOCUMENT | Domestic location with passport when NationalId available | Use NationalId or travel with passport |
| VEHICLE_UNAVAILABLE | Selected vehicle no longer available | Choose different vehicle |

### 15.4 HTTP 500 Internal Server Error

**When:** Unexpected application error occurs.

**Causes:**
- Provider API is down
- Unhandled exception in service layer
- Database connection failure (Phase 2+)
- Malformed internal state

**Response Structure:**
```json
{
  "errorCode": "INTERNAL_SERVER_ERROR",
  "message": "An unexpected error occurred. Please try again later.",
  "details": null,
  "timestamp": "2024-08-01T10:30:00Z",
  "requestId": "correlation-id"
}
```

**Handling:**
- Log full exception stack trace with requestId
- Do NOT expose implementation details in response
- Return generic message to client
- Include requestId for customer support reference
- Implement exponential backoff for provider retries

### 15.5 Error Response Header

All error responses should include:
```
X-Request-Id: [correlation-id]
```

This allows tracing errors through logs using the request ID.

---

## 16. Repository Structure

```
car-rental/
│
├── README.md                              ← Project overview
├── spec.md                                ← This document
├── .gitignore                             ← Git ignore rules
├── LICENSE                                ← License (if applicable)
│
├── src/
│   │
│   ├── CarRental.Api/                     ← Minimal API entry point
│   │   ├── CarRental.Api.csproj
│   │   ├── Program.cs                     ← DI registration, middleware
│   │   │
│   │   ├── Endpoints/
│   │   │   ├── SearchEndpoint.cs          ← GET /cars/search
│   │   │   ├── BookingEndpoint.cs         ← POST /cars/book
│   │   │   └── BookingLookupEndpoint.cs   ← GET /cars/booking/{ref}
│   │   │
│   │   ├── Middleware/
│   │   │   └── ErrorHandlingMiddleware.cs ← Exception → HTTP error response
│   │   │
│   │   └── appsettings.json               ← Configuration
│   │
│   ├── CarRental.Application/             ← Business logic & orchestration
│   │   ├── CarRental.Application.csproj
│   │   │
│   │   ├── Services/
│   │   │   ├── SearchService.cs           ← Orchestrate search
│   │   │   ├── BookingService.cs          ← Create & lookup bookings
│   │   │   ├── ValidationService.cs       ← Business rule validation
│   │   │   └── PricingService.cs          ← Calculate prices
│   │   │
│   │   ├── Contracts/
│   │   │   ├── SearchRequest.cs           ← DTO
│   │   │   ├── SearchResponse.cs          ← DTO
│   │   │   ├── AggregatedQuote.cs         ← DTO
│   │   │   ├── BookingRequest.cs          ← DTO
│   │   │   ├── BookingResponse.cs         ← DTO
│   │   │   ├── ValidationResult.cs        ← DTO
│   │   │   └── ErrorResponse.cs           ← DTO
│   │   │
│   │   └── Interfaces/
│   │       ├── ISearchService.cs
│   │       ├── IBookingService.cs
│   │       ├── IValidationService.cs
│   │       └── IPricingService.cs
│   │
│   ├── CarRental.Domain/                  ← Core business entities
│   │   ├── CarRental.Domain.csproj
│   │   │
│   │   ├── Entities/
│   │   │   ├── Vehicle.cs
│   │   │   ├── Booking.cs
│   │   │   ├── SearchCriteria.cs
│   │   │   └── Quote.cs
│   │   │
│   │   ├── ValueObjects/
│   │   │   ├── Location.cs                ← City + Country
│   │   │   ├── DateRange.cs               ← From + To with validation
│   │   │   ├── Money.cs                   ← Amount + Currency
│   │   │   └── DocumentInfo.cs            ← Type + Number
│   │   │
│   │   ├── Enums/
│   │   │   ├── VehicleCategory.cs         ← Economy, Compact, SUV, Minivan
│   │   │   ├── DocumentType.cs            ← NationalId, Passport
│   │   │   ├── InsuranceType.cs           ← Comprehensive, Basic
│   │   │   └── ProviderName.cs            ← PremiumDrive, BudgetWheels
│   │   │
│   │   └── Exceptions/
│   │       ├── DomainException.cs         ← Base exception
│   │       ├── ValidationException.cs
│   │       ├── PricingException.cs
│   │       └── BookingException.cs
│   │
│   └── CarRental.Infrastructure/          ← External integrations
│       ├── CarRental.Infrastructure.csproj
│       │
│       ├── Providers/
│       │   ├── IProviderClient.cs         ← Interface
│       │   ├── PremiumDriveClient.cs      ← Concrete implementation
│       │   ├── BudgetWheelsClient.cs      ← Concrete implementation
│       │   │
│       │   └── Models/
│       │       ├── ProviderVehicleQuote.cs
│       │       └── ProviderSearchResponse.cs
│       │
│       └── Storage/
│           ├── IBookingStore.cs           ← Interface
│           ├── InMemoryBookingStore.cs    ← Phase 1 implementation
│           └── Models/
│               └── BookingEntity.cs
│
└── tests/
    │
    ├── CarRental.Api.Tests/
    │   ├── CarRental.Api.Tests.csproj
    │   ├── Endpoints/
    │   │   ├── SearchEndpointTests.cs
    │   │   ├── BookingEndpointTests.cs
    │   │   └── BookingLookupEndpointTests.cs
    │   └── Integration/
    │       └── ApiIntegrationTests.cs
    │
    ├── CarRental.Application.Tests/
    │   ├── CarRental.Application.Tests.csproj
    │   ├── Services/
    │   │   ├── SearchServiceTests.cs
    │   │   ├── BookingServiceTests.cs
    │   │   ├── ValidationServiceTests.cs
    │   │   └── PricingServiceTests.cs
    │   └── Contracts/
    │       └── ContractTests.cs
    │
    ├── CarRental.Domain.Tests/
    │   ├── CarRental.Domain.Tests.csproj
    │   ├── Entities/
    │   │   ├── VehicleTests.cs
    │   │   ├── BookingTests.cs
    │   │   └── SearchCriteriaTests.cs
    │   └── ValueObjects/
    │       ├── LocationTests.cs
    │       ├── DateRangeTests.cs
    │       ├── MoneyTests.cs
    │       └── DocumentInfoTests.cs
    │
    └── CarRental.Infrastructure.Tests/
        ├── CarRental.Infrastructure.Tests.csproj
        ├── Providers/
        │   ├── PremiumDriveClientTests.cs
        │   └── BudgetWheelsClientTests.cs
        └── Storage/
            └── InMemoryBookingStoreTests.cs
```

---

## 17. Assumptions

1. **Single Instance** - Application runs as single instance; in-memory storage not suitable for distributed deployment
2. **Synchronous Providers** - Provider APIs respond within reasonable timeout (2-5 seconds)
3. **Deterministic Responses** - Provider APIs return consistent results for identical requests
4. **No Authentication** - All endpoints are public; no JWT/OAuth required
5. **Time Zone Consistency** - All dates/times handled in consistent time zone (UTC assumed)
6. **Provider Uptime** - Providers are available; no fallback logic for failed provider queries
7. **Small Dataset** - Number of vehicles and bookings suitable for in-memory storage (hundreds, not millions)
8. **No Concurrent Mutations** - No multi-threaded booking conflicts in Phase 1
9. **Vehicle IDs Globally Unique** - Each vehicle has unique ID across all providers
10. **Reference Number Format** - Booking references follow BK-YYYY-XXXXXX format (never duplicated)

---

## 18. Risks

### 18.1 High Priority

| Risk | Impact | Mitigation |
|------|--------|-----------|
| **Provider API Downtime** | Search fails entirely if any provider is down | Implement graceful degradation; return results from available providers only |
| **Pricing Calculation Errors** | Incorrect pricing leading to customer dissatisfaction | Comprehensive unit tests for each provider formula with edge cases |
| **Date Range Edge Cases** | Incorrect night counting for BudgetWheels surcharge | Test cross-month, cross-year, DST transitions |
| **Document Validation Logic** | Incorrect booking rejection due to validation bug | Clear specification of rules; unit tests for all combinations |

### 18.2 Medium Priority

| Risk | Impact | Mitigation |
|------|--------|-----------|
| **In-Memory Data Loss** | Bookings lost if application restarts | Add persistent storage (database) before Phase 2 production |
| **No Provider Fallback** | Single unavailable provider impacts all searches | Implement optional fallback to cached data or default response |
| **No Rate Limiting** | Potential for abuse or overload | Add rate limiting middleware in Phase 2 |
| **Vehicle Availability Stale** | Quote prices/availability change between search and booking | Return search timestamp; validate availability at booking time |

### 18.3 Low Priority

| Risk | Impact | Mitigation |
|------|--------|-----------|
| **No User Preferences** | Cannot customize search experience | Add user account system in Phase 3+ |
| **No Insurance Selection** | Users cannot upgrade coverage | Add insurance tier selection in future |
| **Limited Payment** | No payment processing yet | Integrate payment gateway in Phase 2+ |

---

## 19. Future Enhancements

### 19.1 Phase 2: Persistence & Scalability

**Database Integration**
- Migrate from in-memory to SQL Server or PostgreSQL
- Persist bookings, search history, vehicle catalog
- Add audit logging for compliance

**Caching & Performance**
- Redis cache for vehicle inventory
- Cache provider pricing for repeated searches
- Implement cache invalidation strategy

**Asynchronous Processing**
- Background jobs for booking confirmations
- Email/SMS notifications
- Analytics data collection

### 19.2 Phase 3: Authentication & Multi-User

**User Management**
- User registration and login
- OAuth2/OpenID Connect integration
- Role-based access control (customer, admin, provider)

**Personalization**
- User booking history
- Saved searches and alerts
- Loyalty program integration
- Preferred vehicle categories

### 19.3 Phase 4: Payment & Transactions

**Payment Processing**
- Credit card, debit card, UPI integration
- Payment gateway (Stripe, Square, PayPal)
- Invoice and receipt generation
- Refund processing for cancellations

**Financial Reconciliation**
- Accounting system integration
- Provider settlement reports
- Commission calculations
- Revenue analytics

### 19.4 Phase 5: Advanced Search & Filtering

**Vehicle Features**
- Transmission type (Manual, Automatic)
- Fuel type (Petrol, Diesel, Electric)
- Mileage limits
- Driver age restrictions
- Child seat requirements

**Booking Features**
- Pick-up and drop-off at different locations
- Insurance upgrade options
- Roadside assistance packages
- Driver enhancement programs

**Marketplace Features**
- Multi-provider aggregation (third, fourth providers)
- Dynamic pricing based on demand
- Promotional codes and discounts
- Seasonal surge pricing

### 19.5 Phase 6: Mobile & Analytics

**Mobile Applications**
- Native iOS and Android apps
- In-app payments
- Push notifications for bookings
- Location-based search

**Analytics & Insights**
- Search trend analysis
- Popular routes and vehicles
- Provider performance metrics
- Customer journey analytics
- Churn and retention metrics

---

## Appendix A: Example Workflows

### A.1 Complete Search Workflow

```
User searches: Mumbai, Aug 15-18, SUV category

1. SearchEndpoint receives GET /cars/search?pickup=Mumbai&from=2024-08-15&to=2024-08-18&category=SUV

2. ValidationService validates:
   - pickup="Mumbai" ✓ (valid location)
   - from="2024-08-15" ✓ (valid date format)
   - to="2024-08-18" ✓ (to > from)
   - category="SUV" ✓ (valid category)
   → ValidationResult.IsValid = true

3. SearchService creates SearchCriteria

4. SearchService calls PremiumDriveClient.SearchAvailableVehicles(criteria)
   → Returns: [SUV1, SUV2, SUV3] with availability=true

5. SearchService calls BudgetWheelsClient.SearchAvailableVehicles(criteria)
   → Returns: [SUV4, SUV5] (SUV6 filtered out due to availability=false)

6. For each quote, SearchService calls PricingService.CalculateTotalPrice()
   - PremiumDrive: 3000 × 3 days = 9000
   - BudgetWheels: 2500 × 1.2 + 2500 × 1.2 + 2500 × 1.2 = 9000

7. SearchService aggregates 5 quotes into SearchResponse

8. SearchService sorts by TotalPrice ascending:
   [BW-SUV5: 8500, BW-SUV4: 8600, PD-SUV1: 9000, PD-SUV2: 9000, PD-SUV3: 9000]

9. SearchEndpoint returns HTTP 200 with results

10. Frontend displays results sorted by price
```

### A.2 Complete Booking Workflow

```
User books vehicle from search results

1. BookingEndpoint receives POST /cars/book
   {
     "driverName": "Rajesh Kumar",
     "documentType": "NationalId",
     "documentNumber": "1234567890123",
     "vehicleId": "uuid-of-suv5",
     "pickupLocation": "Mumbai"
   }

2. ValidationService.ValidateBookingRequest():
   - driverName present ✓
   - documentType="NationalId" ✓
   - documentNumber present ✓
   - vehicleId valid GUID ✓
   - pickupLocation="Mumbai" ✓
   → ValidationResult.IsValid = true

3. ValidationService.ValidateDocumentForLocation("NationalId", "Mumbai"):
   - Mumbai is domestic location ✓
   - NationalId accepted for domestic ✓
   → Result: valid

4. BookingService.CreateBooking():
   - Generate reference: "BK-2024-000001"
   - Create Booking entity
   - Store to InMemoryBookingStore.SaveBooking()
   - Return BookingResponse with reference

5. BookingEndpoint returns HTTP 201 Created
   Location: /cars/booking/BK-2024-000001

6. User retrieves booking using reference number (workflow below)
```

### A.3 Complete Booking Lookup Workflow

```
User checks booking status

1. BookingLookupEndpoint receives GET /cars/booking/BK-2024-000001

2. ValidationService.ValidateReference("BK-2024-000001"):
   - Format matches BK-YYYY-XXXXXX ✓
   → Validation passes

3. BookingService.GetBookingByReference("BK-2024-000001"):
   - InMemoryBookingStore finds booking
   - Returns Booking entity
   - Transforms to BookingResponse DTO

4. BookingLookupEndpoint returns HTTP 200
   {
     "referenceNumber": "BK-2024-000001",
     "driverName": "Rajesh Kumar",
     ... (full booking details)
   }

5. User sees booking confirmation details displayed on screen
```

### A.4 Document Validation Failure Workflow

```
User attempts to book international pickup with NationalId

1. BookingEndpoint receives POST /cars/book
   {
     "driverName": "Rajesh Kumar",
     "documentType": "NationalId",
     "documentNumber": "1234567890123",
     "vehicleId": "uuid-of-vehicle",
     "pickupLocation": "Dubai"
   }

2. ValidationService.ValidateBookingRequest():
   - All fields present ✓
   → ValidationResult.IsValid = true

3. ValidationService.ValidateDocumentForLocation("NationalId", "Dubai"):
   - Dubai is international location
   - NationalId NOT accepted for international ✓
   → Result: invalid
   → Error: PASSPORT_REQUIRED

4. BookingService rejects booking
   → ValidationException thrown with error code

5. ErrorHandlingMiddleware catches exception

6. BookingEndpoint returns HTTP 422 Unprocessable Entity
   {
     "errorCode": "PASSPORT_REQUIRED",
     "message": "International pickup locations require a passport",
     "details": {...},
     "timestamp": "2024-08-01T10:30:00Z",
     "requestId": "correlation-id"
   }

7. Frontend displays error message to user
```

---

**Document Status:** Phase 1A - Specification Complete  
**Ready for:** Implementation Phase (Phase 1B)  
**Next Milestone:** spec.md approved for commit
