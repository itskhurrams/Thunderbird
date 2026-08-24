# Thunderbird

Clean Architecture API on .NET 10, using ADO.NET for data access, with captcha-gated, two-factor
JWT authentication.

## Features

- Captcha-gated login and registration
- **Mandatory two-factor authentication**: after a correct password, a 6-digit code is sent to both
  the user's email and WhatsApp (same code to both, either delivery proves possession). The code
  expires in 5 minutes, is single-use, and locks out after 5 wrong attempts. A JWT is only issued
  once the code is verified (returned in the response body and set as an `HttpOnly` cookie).
- Passwords are hashed (PBKDF2-SHA256, salted). Legacy plaintext accounts from before hashing was
  introduced are accepted once on login and transparently upgraded to a hash - see `sql/`.
- Captchas are single-use and expire after 5 minutes (enforced in-memory, independent of the DB schema)
- Rate limiting on the auth endpoints
- Global exception handling (`ProblemDetails` responses)
- Health check at `/health` (includes SQL Server connectivity)
- Structured logging via Serilog (console + rolling file sink)
- Config-driven CORS policy
- Docker support (multi-stage build)

## Auth flow

1. `GET /api/captcha` → `{ id, captcha }` (PNG bytes)
2. `POST /api/user` (login) or `POST /api/user/register` with the captcha id/code plus
   credentials → `{ challengeId, message }`. A code is sent to the user's email and WhatsApp.
3. `POST /api/user/2fa/verify` with `{ challengeId, code }` → `{ token, user }`, and the JWT is
   also set as an `HttpOnly` cookie.

## Projects

- `Thunderbird.API` — ASP.NET Core Web API, hosting/composition root
- `Thunderbird.Application` — services (captcha, user, territory, token, 2FA, password hashing)
- `Thunderbird.Domain` — entities, interfaces, options models
- `Thunderbird.Infrastructure.Persistance` — ADO.NET repositories (stored-procedure based)
- `Thunderbird.Infrastructure.Caching` — in-memory cache provider
- `Thunderbird.Infrastructure.Common` — shared conversion helpers
- `Thunderbird.Infrastructure.Logging` — Serilog wiring
- `Thunderbird.Infrastructure.Notifications` — email (SMTP) and WhatsApp (Meta Cloud API) senders
- `Thunderbird.Infrastructure.IOC` — DI container registration
- `tests/Thunderbird.Application.Tests` — xUnit tests

## Running locally

Set the following via `dotnet user-secrets` (already used by the API project) or environment variables — `appsettings.json` intentionally ships without real values:

- `Data:ConnectionString` — SQL Server connection string
- `TokenAuthentication:SecretKey` — JWT signing key
- `Email:SmtpHost`, `Email:FromAddress` (and `Email:Username`/`Email:Password` if your relay needs auth)
- `WhatsApp:AccessToken`, `WhatsApp:PhoneNumberId` — Meta WhatsApp Cloud API credentials. You also
  need an **approved message template** (see `WhatsApp:TemplateName`, default `otp_verification`) -
  Meta rejects business-initiated free-text messages, so this can't work without one.

```
dotnet user-secrets set "Data:ConnectionString" "..." --project src/Thunderbird.API
dotnet user-secrets set "TokenAuthentication:SecretKey" "..." --project src/Thunderbird.API
dotnet user-secrets set "Email:SmtpHost" "..." --project src/Thunderbird.API
dotnet user-secrets set "Email:FromAddress" "..." --project src/Thunderbird.API
dotnet user-secrets set "WhatsApp:AccessToken" "..." --project src/Thunderbird.API
dotnet user-secrets set "WhatsApp:PhoneNumberId" "..." --project src/Thunderbird.API
dotnet run --project src/Thunderbird.API
```

In production, supply these as environment variables (e.g. `Data__ConnectionString`,
`WhatsApp__AccessToken`). The app validates all of these at startup and refuses to boot if any
are missing, since 2FA is mandatory for every login.

The database schema (tables + stored procedures) lives in `sql/`, reverse-engineered from the
entities/repositories since no migration scripts previously existed in this repo - review before
running against a real database:

- `2026-08-24_full_schema.sql` — base tables (`Users`, `Captcha`, `Divisions`) and procedures
- `2026-08-25_two_factor_auth.sql` — adds `email`/`phone_number` columns to `Users` (run after the above)

Both are idempotent (tables/columns created only if missing, procedures use `CREATE OR ALTER`) and
were validated end-to-end against a throwaway LocalDB instance, but they were written blind to your
actual schema, so double check table/column names match if you already have this database
provisioned elsewhere. Any pre-existing account without an email or phone number on file will need
one added before it can log in again, since 2FA is mandatory.

## Tests

```
dotnet test tests/Thunderbird.Application.Tests
```
