# AI Prompts Log — Car Rental Availability System

This file records every significant prompt submitted to GitHub Copilot during development, the AI's autonomous judgement calls, and the rationale accepted or overridden by the developer.

---

## How to Read This Log

Each entry contains:
- **Prompt** — exact or representative prompt submitted
- **AI Judgement Call** — a decision the AI made autonomously that was not explicitly specified
- **Accepted / Overridden** — whether the developer kept the AI's choice
- **Rationale** — why the decision was correct or wrong

---

## Prompt 1 — Project Skeleton & Boilerplate 

**Prompt submitted:**
```
You are a Principal .NET 8 Solution Architect.

This is PHASE 1B of the project.

Generate production-quality project structure with:
CarRental.Api, CarRental.Tests, car-rental-ui

Create all folders, interfaces, services, providers, endpoints, middleware,
validators, extensions, and configuration files.

Every class should contain XML summary comments.
Every method should throw NotImplementedException.
No business logic. No implementation. No fake data.
```

**AI Judgement Call 1 — Minimal API over MVC Controllers**
The AI chose ASP.NET Core Minimal APIs instead of the traditional Controller pattern without being asked to choose.
- *Accepted.* Minimal APIs reduce boilerplate, have lower memory overhead, and align with .NET 8 best practices. Endpoints are explicit and centralised in `Program.cs`.

**AI Judgement Call 2 — Extension methods for DI registration**
The AI organised all service registrations into extension methods (`AddApplicationServices()`, `AddRentalProviders()`, `AddValidators()`) rather than inline registrations in `Program.cs`.
- *Accepted.* This keeps `Program.cs` readable as a composition root and groups related registrations by concern.

**AI Judgement Call 3 — Interface-first design before any implementation**
The AI created all interfaces in `Interfaces/` before writing any concrete class, even though the prompt only asked for a skeleton.
- *Accepted.* Enforces the Dependency Inversion Principle from the start and makes the contract explicit before implementation details are considered.

**AI Judgement Call 4 — Scoped lifetime for all services**
The AI registered every service as `AddScoped` (per-request lifetime) without being given a lifetime strategy.
- *Accepted.* Per-request lifetime is the correct default for services that may hold request-scoped state and aligns with ASP.NET Core conventions.

---

## Prompt 2 — Foundation Layer: Models, DTOs, Enums & Stub Providers 

**Prompt submitted:**
```
This is Phase 2 of the Car Rental Availability project.

Implement ONLY the foundation of the application:
- Domain Models, DTOs, Enums, Provider Contracts, Provider Stub Implementations, DI

PremiumDrive: Always available, flat daily pricing, comprehensive insurance, free 48h cancellation.
BudgetWheels: Mix of available/unavailable, 20% weekend surcharge, basic insurance, non-refundable.

Create deterministic in-memory data. Never generate random data.
```

**AI Judgement Call 1 — Enums over strings for domain classifications**
The AI replaced every string field (`"Economy"`, `"Passport"`, `"Basic"`) with typed enums (`VehicleCategory`, `DocumentType`, `InsuranceType`, `CancellationPolicy`, `ProviderType`) without being asked to.
- *Accepted.* Enums provide compile-time safety, eliminate typos, and make the domain model self-documenting. String comparisons for domain values are a known source of subtle bugs.

**AI Judgement Call 2 — Location as a C# record (value semantics)**
The AI modelled `Location` as a `record` rather than a `class`.
- *Accepted.* Locations are value objects — equality is based on the location name, not object identity. Records provide this behaviour with minimal boilerplate and enforce immutability.

**AI Judgement Call 3 — Night-by-night iteration for BudgetWheels pricing**
The prompt said "20% surcharge on weekends" but did not specify whether to multiply daily rate × days or iterate night by night. The AI chose to iterate through each individual night and check the day of week.
- *Accepted.* The day-by-day approach is the only correct one when a rental spans a mix of weekday and weekend nights. A blanket multiplier would over- or under-charge for partial-weekend rentals.

**AI Judgement Call 4 — 8 deterministic vehicles per provider (2 per category)**
The prompt asked for deterministic data but did not specify how many vehicles. The AI chose 8 vehicles (2 per category) per provider with hardcoded `ProviderVehicleId` strings.
- *Accepted.* Gives sufficient breadth for category-filter testing while keeping the fixture manageable. Fixed IDs make test assertions stable.

---

## Prompt 3 — Search Feature: Pricing Strategies, Service Orchestration & Endpoint 

**Prompt submitted:**
```
This is Phase 3 of the Car Rental Availability project.
Implement ONLY the Search feature.

Implement: CarRentalService, Pricing Strategy Pattern, Vehicle Normalization,
Search Endpoint, Search Validation, Unit Tests.

SEARCH FLOW: Validate → Query providers → Filter unavailable → Calculate pricing
→ Normalize → Sort by price → Return.

GET /cars/search  params: pickup, from, to, category (optional)

PRICING STRATEGY
IPricingStrategy.CalculateTotalPrice(decimal dailyRate, DateOnly from, DateOnly to)
PremiumDrive: dailyRate × numberOfNights
BudgetWheels: Friday/Saturday/Sunday get 20% surcharge, iterate night-by-night.

Return HTTP 400 for missing/invalid fields.
Sort by TotalPrice ascending before returning.
```

**AI Judgement Call 1 — Strategy Pattern for pricing (not if/else in service)**
The AI introduced a full `IPricingStrategy` interface with a `IPricingStrategyRegistry` rather than branching on provider type inside `CarRentalService`.
- *Accepted.* The Strategy Pattern satisfies the Open/Closed Principle: adding a third provider requires only a new strategy class and a DI registration with zero changes to `CarRentalService`.

**AI Judgement Call 2 — Task.WhenAll for parallel provider queries**
The prompt described a sequential flow. The AI queried all providers concurrently using `Task.WhenAll`.
- *Accepted.* Providers are independent; parallelism eliminates the latency of sequential queries and is essential for meeting the 2-second NFR as the provider count grows.

**AI Judgement Call 3 — DateOnly (not DateTime) in pricing strategy signatures**
The AI used `DateOnly` for the `from`/`to` parameters in `CalculateTotalPrice` rather than `DateTime`.
- *Accepted.* Rental periods are calendar-date based, not time-based. `DateOnly` makes the intent explicit in the method signature and removes any risk of time-of-day artefacts in calculations.

**AI Judgement Call 4 — Guid.NewGuid() for VehicleId in normalisation**
The AI initially generated a fresh `Guid.NewGuid()` for each vehicle during normalisation.
- *Overridden (fixed in later session).* A random ID on every search request means the same vehicle gets a different ID on every response, breaking booking lookup and cross-request consistency. The correct approach — which was applied — is a deterministic ID derived via MD5 hash of `"ProviderName:ProviderVehicleId"`.

**AI Judgement Call 5 — Filter unavailable vehicles before normalisation**
The AI filtered `IsAvailable == false` before the normalisation loop rather than filtering after.
- *Accepted.* Filtering first is more efficient (fewer items to process) and avoids normalising data that will never be returned to the client.

---

## Prompt 4 — Booking Feature: Document Validation, Service & Endpoints 

**Prompt submitted:**
```
This is Phase 4. Implement ONLY the Booking feature.

POST /cars/book — DriverName, DocumentType, DocumentNumber, VehicleId,
Provider, PickupLocation, PickupDate, ReturnDate

GET /cars/booking/{reference}

DOCUMENT VALIDATION
Domestic (Mumbai, Bengaluru): NationalId or Passport accepted.
International (Dubai, Singapore, London): Passport only.
If validation fails: HTTP 422.

BOOKING REFERENCE: CR-YYYYMMDD-XXXXXX (deterministic, incrementing).
```

**AI Judgement Call 1 — IDocumentValidationService as a dedicated interface**
The AI extracted document validation into its own service and interface rather than embedding the location/document logic inside `BookingService`.
- *Accepted.* Isolating the validation rule (domestic vs. international) makes it independently testable and allows `SearchRequestValidator` to reuse it later for pickup-location validation (which was needed and implemented).

**AI Judgement Call 2 — ConcurrentDictionary for in-memory booking store**
The prompt said "in-memory storage" without specifying a thread-safety mechanism. The AI chose a static `ConcurrentDictionary` with `Interlocked.Increment` for the sequence counter.
- *Accepted.* `ConcurrentDictionary` provides lock-free reads and atomic writes — correct for a web server handling concurrent booking requests without a database.

**AI Judgement Call 3 — All validation failures mapped to HTTP 422**
The AI initially mapped both missing-field errors and document/location errors to `InvalidOperationException` → HTTP 422 in the endpoint.
- *Overridden (fixed in later session).* The spec distinguishes: missing fields → HTTP 400, invalid document/location combination → HTTP 422. The fix introduced `BookingValidationException` thrown by `ValidateRequest()` which the endpoint catches as 400, while document-validation `InvalidOperationException` continues to map to 422.

**AI Judgement Call 4 — Interlocked counter for booking reference uniqueness**
The AI used `Interlocked.Increment` on a static `long` field rather than `Guid.NewGuid()` or a timestamp-only approach.
- *Accepted.* Atomic increment is thread-safe without locks and produces human-readable, sequentially ordered references that are easier to debug and audit than GUIDs.

---

## Prompt 5 — Frontend: React SPA with Search, Results & Booking 

**Prompt submitted:**
```
Implement the React + TypeScript frontend for the car rental system.

Components: SearchForm, ResultsTable, BookingForm, BookingConfirmation.
Pages: HomePage, ResultsPage, BookingPage, ConfirmationPage.
Hooks: useSearch, useBooking.
Services: apiService.ts — all API communication through axios.

API base URL from environment variable VITE_API_BASE_URL.
Show all document types in BookingForm; show a clear error for invalid selections.
Results sorted server-side; client-side sort only when user explicitly selects.
```

**AI Judgement Call 1 — Hardcoded http://localhost:5000 in apiService.ts**
The AI initially hardcoded the API base URL as a string literal rather than reading from an environment variable.
- *Overridden (fixed in later session).* A hardcoded host breaks portability across environments (dev, staging, CI). The fix: `.env` with `VITE_API_BASE_URL=http://localhost:5000`, `apiService.ts` reads `import.meta.env.VITE_API_BASE_URL`, and `vite.config.ts` uses `loadEnv` so the dev-server proxy derives from the same variable.

**AI Judgement Call 2 — Client-side sort applied on every render by default**
`ResultsTable` initialised `sortBy` to `'asc'` so `useMemo` re-sorted the server response on every render, even though the server already returns results sorted ascending.
- *Overridden (fixed in later session).* The default state is now `''` (no client sort). The sort is applied only when the user explicitly selects an order from the dropdown. The server-supplied order is preserved otherwise, matching the spec requirement.

**AI Judgement Call 3 — Filtering out invalid document types in BookingForm**
The AI rendered only the allowed document types in the dropdown (e.g., only Passport for international locations), silently hiding National ID.
- *Overridden (fixed in later session).* The spec requires all document types to always be shown; invalid selections for the chosen location must show a clear rejection message styled with `theme.colors.error`, never silently filtered.

**AI Judgement Call 4 — Custom hooks for all stateful API interactions**
The AI extracted all search and booking state into `useSearch` and `useBooking` hooks rather than managing state directly in page components.
- *Accepted.* Custom hooks separate data-fetching concern from rendering concern, make the state logic reusable and independently testable, and keep page components thin.

---

## Prompt 6 — Identified few things which are not mathincg with provieded document and configuration

**Prompt submitted (series of targeted corrections):**
```
Fix the following incomplete/incorrect behaviours identified against the spec:

1. BookingForm only shows allowed document types; invalid types must be shown
   with a clear rejection message.
2. SearchRequestValidator does not validate pickup against the known city list.
3. BookingEndpoints maps all InvalidOperationExceptions to 422;
   field-validation failures must be 400.
4. CarRentalService uses Guid.NewGuid() for VehicleId at normalisation time.
5. ResultsPage re-sorts client-side on every render.
6. apiService.ts hardcodes http://localhost:5000.
```

**AI Judgement Call 1 — IsKnownLocation added to IDocumentValidationService**
Rather than duplicating the city list inside `SearchRequestValidator`, the AI extended `IDocumentValidationService` with `IsKnownLocation(string location)` and injected the service into the validator.
- *Accepted.* The city list has a single source of truth (the service). The validator consumes it via the existing abstraction with no duplication.

**AI Judgement Call 2 — BookingValidationException as the discriminator for 400 vs 422**
The AI introduced a new `BookingValidationException` class in `Common/` rather than adding a flag to `InvalidOperationException` or using a different catch-order trick.
- *Accepted.* A dedicated exception type is the cleanest discriminator: the exception's type carries the semantic meaning (field validation failure), and the endpoint catch chain is explicit and readable.

**AI Judgement Call 3 — MD5 for deterministic VehicleId derivation**
The AI used `MD5.HashData` to produce a 16-byte deterministic `Guid` from `"ProviderName:ProviderVehicleId"`, noting that MD5 is used for stable identifier derivation, not for security.
- *Accepted.* .NET 8 does not include built-in UUID v5. MD5 over a namespaced input string produces a deterministic, collision-resistant identifier for this use case. The security non-use is documented in a code comment.

---

