# Car Rental Availability System

A cloud-ready, multi-provider aggregation platform that unifies rental availability and pricing across competing rental service providers. Built with .NET 8 Minimal APIs and React to demonstrate enterprise-grade architectural patterns, extensibility, and separation of concerns.

---

## Quick Start 🚀

### Prerequisites

- **.NET 8 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Node.js 18+** - [Download](https://nodejs.org/)
- **Git** - [Download](https://git-scm.com/)

### Clone Repository

```bash
git clone https://github.com/Mundekar/car-rental.git
cd car-rental
```

### ⚡ One-Click Launcher (Alternate Option)

For fastest setup, run the PowerShell launcher script which starts both backend and frontend automatically:

```powershell
# From project root directory
.\run-app.ps1
```

The script will:
- ✅ Verify prerequisites (.NET 8, Node.js)
- ✅ Validate project structure
- ✅ Launch backend in new window (`http://localhost:5000`)
- ✅ Launch frontend in new window (`http://localhost:3000`)
- ✅ Wait for services to be ready

Once ready, open `http://localhost:3000` in your browser.

> **Note:** If you get PowerShell execution policy errors, run this first:
> ```powershell
> Set-ExecutionPolicy -ExecutionPolicy Bypass -Scope Process
> ```

---

### Manual Setup (Step-by-Step)

If you prefer to run commands manually:

### Backend Setup

```bash
# Navigate to solution directory
cd car-rental

# Restore packages
dotnet restore

# Build solution
dotnet build

# Run tests (90+ tests, 100% pass rate)
dotnet test

# Start API (Development)
dotnet run --project src/CarRental.Api
```

**API runs on:** `http://localhost:5000`  
**Swagger UI:** `http://localhost:5000/swagger`

### Frontend Setup

```bash
# Navigate to frontend directory
cd car-rental-ui

# Install dependencies
npm install

# Optional: override API base URL (defaults to http://localhost:5000)
# PowerShell:
$env:VITE_API_BASE_URL="http://localhost:5000"
# Bash:
export VITE_API_BASE_URL="http://localhost:5000"

# Start development server
npm run dev

# Run frontend unit tests (Vitest + React Testing Library)
npm test

# Production build
npm run build

# Preview production build
npm run preview
```

**Frontend runs on:** `http://localhost:3000`

### Verify Installation

1. **Backend:** Open `http://localhost:5000/swagger` - should show Swagger UI with 3 endpoints
2. **Frontend:** Open `http://localhost:3000` - should show Car Rental home page
3. **Backend Tests:** Run `dotnet test` - should show backend tests passing
4. **Frontend Tests:** Run `npm test` from `car-rental-ui` - should show Vitest tests passing

---

## Project Overview

The **Car Rental Availability System** is a cloud-ready, multi-provider aggregation platform designed to unify rental availability and pricing across competing rental service providers. The system presents a single, normalized search interface to end users while abstracting the complexities of provider-specific pricing models, availability logic, and booking workflows.

This project demonstrates enterprise-grade architectural patterns, extensibility principles, and separation of concerns in modern .NET applications.


## Features

| Feature | Description |
|---------|-------------|
| **Multi-Provider Search** | Query available vehicles across two rental providers in a single API call |
| **Result Aggregation** | Normalize and combine results from multiple providers |
| **Price-Based Sorting** | Automatically sort results by total price (ascending) |
| **Category Filtering** | Filter by vehicle category: Economy, Compact, SUV, Minivan |
| **Weekend Pricing** | Apply provider-specific pricing models (flat vs. dynamic with 20% weekend surcharges) |
| **Booking** | Reserve vehicles with driver information and document validation |
| **Document Validation** | Validate travel documents based on location (domestic vs. international) |
| **Booking Lookup** | Retrieve booking confirmations by reference number |
| **API Documentation** | Swagger/OpenAPI integration for endpoint exploration |
| **Comprehensive Tests** | 90+ unit tests covering all business logic (100% pass rate) |
| **Responsive UI** | React frontend with mobile and desktop support |

**Supported Vehicle Categories:** Economy, Compact, SUV, Minivan  
**Supported Locations (Domestic):** Mumbai, Bengaluru  
**Supported Locations (International):** Dubai, Singapore, London

---

## Technology Stack

### Backend

| Component | Technology |
|-----------|------------|
| Framework | .NET 8 |
| API | Minimal APIs |
| Language | C# |
| Dependency Injection | Built-in DI |
| Testing | xUnit + Moq |

### Frontend

| Component | Technology | 
|-----------|-----------|
| **Framework** | React 18.2.0 | 
| **Language** | TypeScript  5.2 |
| **Build Tool** | Vite 5.0 |  
| **HTTP Client** | Axios 1.18.1| 
| **Unit Testing** | Vitest + React Testing Library |


### Data Storage

| Component | Technology | Purpose |
|-----------|-----------|----------|
| **Persistence** | ConcurrentDictionary (In-Memory) | Thread-safe single-instance storage |
| **Database** | None | Simplified infrastructure for assessment scope |

### Development Tools

| Tool | Purpose |
|------|----------|
| **Visual Studio Code** | Primary editor |
| **Git** | Version control |
| **npm** | Dependency management (frontend) |
| **dotnet CLI** | Build and run (backend) |

---

## Architecture

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

1. Implement new `ICarRentalProvider` interface
2. Create provider-specific pricing strategy (implements `IPricingStrategy`)
3. Register in dependency injection container
4. System automatically includes results in aggregated responses

**Example:** Adding a third provider (e.g., "EconomyRents") requires:
- One new provider class (~100 LOC)
- One new pricing strategy class (~50 LOC)
- Two lines of DI registration
- **Zero changes** to CarRentalService, endpoints, or existing tests

---

## Repository Structure

### Backend (Single Monolithic Project)

   ```
   car-rental/
   ├── src/
   │   └── CarRental.Api/
   │       ├── Endpoints/
   │       ├── Services/
   │       ├── Providers/
   │       ├── Models/
   │       ├── DTOs/
   │       ├── Interfaces/
   │       ├── Middleware/
   │       └── Program.cs
   │
   ├── tests/
   │   └── CarRental.Tests/
   │
   ├── car-rental-ui/
   │   ├── src/
   │   └── package.json
   │
   ├── README.md
   ├── spec.md
   ├── prompts.md
   └── reflection.md
   ```


## Design Decisions

### 1. Strategy Pattern for Pricing

**Why:** Isolates pricing logic from service orchestration, allowing provider-specific pricing models to be changed independently without affecting core business logic or requiring conditional branching in services.

### 2. Dependency Injection Container

**Why:** Built-in .NET DI container provides inversion of control without external dependencies, enabling testability through mock injection and supporting loose coupling between layers.

### 3. Provider Abstraction (ICarRentalProvider)

**Why:** Enables zero-modification extensibility—new providers can be added by implementing the interface and registering in DI, without touching endpoint or service code. Demonstrates Open/Closed Principle.

### 4. In-Memory Storage (ConcurrentDictionary)

**Why:** Provides thread-safe storage suitable for assessment scope while avoiding database complexity. Deterministic behavior supports reliable testing. Can be replaced with database layer in production without API changes.

---

## Assumptions

- In-memory storage is used for the scope of this assessment.
- External rental providers are simulated and assumed to be available.
- Authentication and authorization are out of scope.
- The application is intended for a single-instance deployment.

---

