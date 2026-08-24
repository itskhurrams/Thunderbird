# Thunderbird

Clean Architecture API on .NET 10, using ADO.NET for data access, with captcha-gated JWT authentication.

## Features

- Captcha-gated login issuing a JWT (returned in the response body and set as an `HttpOnly` cookie)
- Captchas are single-use and expire after 5 minutes (enforced in-memory, independent of the DB schema)
- Rate limiting on the auth endpoints
- Global exception handling (`ProblemDetails` responses)
- Health check at `/health` (includes SQL Server connectivity)
- Structured logging via Serilog (console + rolling file sink)
- Config-driven CORS policy
- Docker support (multi-stage build)

## Projects

- `Thunderbird.API` — ASP.NET Core Web API, hosting/composition root
- `Thunderbird.Application` — services (captcha, user, territory, token)
- `Thunderbird.Domain` — entities, interfaces, options models
- `Thunderbird.Infrastructure.Persistance` — ADO.NET repositories (stored-procedure based)
- `Thunderbird.Infrastructure.Caching` — in-memory cache provider
- `Thunderbird.Infrastructure.Common` — shared conversion helpers
- `Thunderbird.Infrastructure.Logging` — Serilog wiring
- `Thunderbird.Infrastructure.IOC` — DI container registration
- `tests/Thunderbird.Application.Tests` — xUnit tests

## Running locally

Set the following via `dotnet user-secrets` (already used by the API project) or environment variables — `appsettings.json` intentionally ships without real values:

- `Data:ConnectionString` — SQL Server connection string
- `TokenAuthentication:SecretKey` — JWT signing key

```
dotnet user-secrets set "Data:ConnectionString" "..." --project src/Thunderbird.API
dotnet user-secrets set "TokenAuthentication:SecretKey" "..." --project src/Thunderbird.API
dotnet run --project src/Thunderbird.API
```

In production, supply these as environment variables (`Data__ConnectionString`, `TokenAuthentication__SecretKey`).

## Tests

```
dotnet test tests/Thunderbird.Application.Tests
```

## Not yet implemented

- Password hashing (the stored procedure currently does the comparison)
- User registration (needs a new stored procedure)
