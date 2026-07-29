Project Stack

- .NET 8
- React
- TypeScript
- xUnit

Architecture

- Use Minimal APIs
- Use Dependency Injection
- Follow SOLID
- Keep endpoints thin
- Business logic belongs in services
- Prefer composition over inheritance

Testing

- xUnit + Moq
- Add tests for all new services
- Prefer deterministic test data

Coding Style

- Use async/await
- XML docs on public APIs
- Avoid magic strings
- Reuse existing abstractions before creating new ones

Frontend

- API calls through apiService.ts
- Use environment variables
- Prefer reusable hooks