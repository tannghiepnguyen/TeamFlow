# Backend instructions

These instructions supplement `../AGENTS.md` for `backend/`.

## Current structure
- `TeamFlow.slnx` includes projects under `src/` and `tests/`, currently targeting `net11.0`.
- `src/TeamFlow.API` hosts controllers, OpenAPI, and Scalar in development. Preserve the uppercase `API` in paths.
- Domain, Application, and Infrastructure contain placeholder classes; Worker is a scaffold. The unit, integration, and architecture test projects also contain placeholders.
- Identity, EF Core, Npgsql, database contexts, migrations, and application dependency wiring are not yet implemented. Do not assume project references enforce the intended boundaries.

## Intended dependency boundaries
- Domain: entities, value objects, invariants, and domain events; no dependencies on Application, Infrastructure, HTTP, EF Core, or Identity.
- Application: business use cases, DTOs, validation, authorization, and infrastructure interfaces; depends on Domain. Organize new use cases by feature.
- Infrastructure: persistence, Identity integration, and external service implementations; depends on Application and Domain.
- API: HTTP mapping and dependency injection composition; keep controllers thin. Reference Infrastructure for composition, not to move persistence into controllers.
- Worker: background host when a concrete background-processing requirement exists. Keep business logic in the appropriate shared layer.
- Add project references deliberately when implementing layers. Avoid introducing generic repositories, mediator packages, or redundant service layers without a concrete need.

## API behavior
- Inspect existing routes before adding endpoints. The current sample uses `/weatherforecast`; the frontend development proxy forwards `/api/weatherforecast` to it.
- If adopting versioned routes, update the proxy, callers, and documentation together to avoid stripping or doubling prefixes.
- Use request/response DTOs, validate body/route/query values, and preserve domain invariants independently of request validation.
- Use Problem Details for new API errors and consistent status codes: 400 validation, 401 unauthenticated, 403 forbidden, 404 missing or intentionally concealed, 409 conflicts.
- Do not expose exception details, persistence entities, or Identity models to clients.
- Propagate cancellation tokens for asynchronous I/O. Paginate potentially unbounded collections and use stable sorting.
- Keep OpenAPI and frontend types/callers aligned with endpoint changes.

## Authentication and tenancy when implemented
- Use ASP.NET Core Identity for credentials and account lifecycle; do not implement password hashing or recovery yourself.
- Keep Identity-specific types in Infrastructure; reference users by Guid in domain entities.
- Model workspace roles on membership rather than treating global Identity roles as workspace permissions.
- Authenticate and authorize each operation, and scope tenant-owned queries and writes by the authorized workspace. Check that related project, issue, assignee, and membership records belong to that workspace.
- An authorization attribute or EF query filter alone is insufficient for resource-level checks.
- Configure secure cookies, CSRF protection, CORS/origin behavior, and production Data Protection key persistence when introducing browser cookie authentication.

## Persistence when implemented
- Use EF Core/Npgsql with PostgreSQL for the planned persistence layer; keep mappings and migrations in Infrastructure.
- Define constraints, indexes, delete behavior, and concurrency rules explicitly. Use UTC timestamps and PostgreSQL `timestamptz` where appropriate.
- Add migrations for schema changes and inspect generated SQL. Do not modify migrations already applied to shared environments.
- Use transactions for changes that must succeed together. Keep external network side effects outside transactions.
- Protect concurrent edits from lost updates with a version token or conditional write and expose conflicts to callers.
- Introduce a transactional outbox only when reliable event delivery is required; consumers must tolerate retries and duplicates.
- Never run destructive database commands or production migrations without explicit authorization.

## Verification
Run from the repository root:
```sh
dotnet restore backend/TeamFlow.slnx
dotnet build backend/TeamFlow.slnx --no-restore
dotnet test backend/TeamFlow.slnx --no-build
```
To run only the API:
```sh
dotnet run --project backend/src/TeamFlow.API --launch-profile https
```
- Put invariant tests in UnitTests, API/persistence behavior in IntegrationTests, and dependency-boundary tests in ArchitectureTests.
- Add meaningful assertions when replacing placeholder tests. Test cross-workspace access denial for tenant-owned behavior and conflict/retry handling when affected.
- Do not assume a passing placeholder suite verifies an implementation.
- Verify actual context names and startup configuration before documenting or executing EF migration commands. No database migration command is configured yet.
- Report build/test results and any unverified behavior; avoid unrelated formatting changes.
