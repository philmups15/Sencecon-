# Sencecon

A Clean Architecture boilerplate for an ASP.NET Core Web API, built on .NET 10.

## Architecture

```
src/
  Sencecon.Domain          Entities, enums, domain exceptions. No dependencies.
  Sencecon.Application     CQRS use cases (MediatR), validation (FluentValidation),
                            interfaces implemented by outer layers.
  Sencecon.Infrastructure  EF Core (PostgreSQL), JWT issuing, password hashing.
  Sencecon.API             Controllers, composition root (Program.cs), middleware.
tests/
  Sencecon.Application.UnitTests   Validator/handler tests, no external dependencies.
```

Dependencies point inward: `API -> Infrastructure -> Application -> Domain`, and
`API -> Application`. The Application layer only depends on abstractions
(`IApplicationDbContext`, `IJwtTokenGenerator`, `IPasswordHasher`), which
Infrastructure implements — so business logic never depends on EF Core, Postgres,
or ASP.NET Core directly.

Includes a sample `TodoItem` feature (Create/Update/Delete/Get, one per user) and
an `Auth` feature (Register/Login issuing JWTs) to demonstrate the pattern end to end.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- PostgreSQL (local instance or Docker)

`dotnet restore`, `dotnet build`, and `dotnet test` have all been verified against this
setup — the solution builds clean with zero warnings and the sample unit tests pass.

## Getting started

1. Start PostgreSQL, e.g.:

```bash
docker run --name sencecon-db -e POSTGRES_PASSWORD=postgres -p 5432:5432 -d postgres:16
```

2. Update the connection string and JWT secret in
   `src/Sencecon.API/appsettings.Development.json` if needed (defaults assume the
   container above). Never commit a real JWT secret — use `dotnet user-secrets` or
   environment variables in real environments.

3. Restore and create the database (an `InitialCreate` migration is already checked in):

```bash
dotnet restore
dotnet tool install --global dotnet-ef   # first time only
dotnet ef database update --project src/Sencecon.Infrastructure --startup-project src/Sencecon.API
```

4. Run the API:

```bash
dotnet run --project src/Sencecon.API
```

Swagger UI opens at `https://localhost:5081/swagger`. The API also auto-applies
pending migrations on startup in the Development environment.

## Adding a migration

```bash
dotnet ef migrations add <Name> --project src/Sencecon.Infrastructure --startup-project src/Sencecon.API
```

## Running tests

```bash
dotnet test
```

## Adding a new feature

Follow the existing `TodoItems` slice as a template:

1. Add/extend an entity in `Sencecon.Domain`.
2. Add an EF Core configuration in `Sencecon.Infrastructure/Persistence/Configurations`
   (and a migration).
3. Add a command/query + handler + validator under
   `Sencecon.Application/<Feature>/...`.
4. Expose it via a controller action in `Sencecon.API/Controllers`.

## Auth

`POST /api/auth/register` and `POST /api/auth/login` return a JWT. Send it as
`Authorization: Bearer <token>` on subsequent requests to `[Authorize]` endpoints
(e.g. `/api/todoitems`). `ICurrentUserService` reads the user id from the token's
`sub` claim.
