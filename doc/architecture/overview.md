# System architecture

TeamFlow is designed as a modular monolith for project and issue management, with an API and separately runnable Worker in one repository. A workspace is the tenant boundary. PostgreSQL is the planned source of truth.

## Current checkout

| Area | Current state |
| --- | --- |
| API | ASP.NET Core controllers, a sample weather endpoint, and development OpenAPI/Scalar |
| Domain | Entity and enum definitions for the planned workflows |
| Application | Scaffold for use cases |
| Infrastructure | Initial EF Core/Npgsql mappings, Identity user profile type, issue sequence record, outbox record, and processing ledger |
| Worker | Background service scaffold |
| Frontend | React 19, strict TypeScript, TanStack Start/Router/Query, Tailwind 4 and Nitro |
| Tests | Generated PostgreSQL schema checks for the initial mappings, plus scaffold tests |

```text
Browser -> TanStack Start/Vite (localhost:3000)
           /api/* -> development proxy -> ASP.NET Core API (localhost:7045)
                      strips /api          GET /weatherforecast
```

[Root README](../../README.md) documents the active launcher and configuration. An application DbContext, database setup, authentication, tenant policies, outbox processing, and business endpoints remain planned.

## Layer boundaries

| Project | Responsibility | Direct project references |
| --- | --- | --- |
| `TeamFlow.Domain` | Entities, invariants, value objects and domain events | None |
| `TeamFlow.Application` | Feature commands/queries, validation, authorization, DTOs and ports | Domain |
| `TeamFlow.Infrastructure` | EF mappings/migrations, Identity, data access and external adapters | Application |
| `TeamFlow.API` | HTTP and SignalR routing, request identity, error mapping and composition | Application, Infrastructure |
| `TeamFlow.Worker` | Scoped background execution, cancellation and system-actor context | Application, Infrastructure |

Infrastructure uses Domain types through Application’s transitive project reference. API’s direct reference to Infrastructure supports service registration at the composition root. The Worker and API compose services independently and share use cases through Application.

Organize use cases by feature. Separate commands from queries where it improves ownership and validation. Keep controllers focused on transport concerns. Preserve domain independence from ASP.NET Core, Identity and EF Core.

## Planned runtime

```text
Browser -> same-site frontend/API ingress -> API -> Application -> PostgreSQL
                                                 |
                                              SignalR hints (planned)
Worker -> claim outbox in PostgreSQL -> persist notifications
                                      -> optional email or authorized live delivery
```

The production ingress and API routing arrangement remain open. See [deployment](../operations/deployment.md) and the [identity and hosting decision](decisions/0002-identity-and-app-contexts.md).

## Frontend boundaries

Keep routes in `frontend/src/routes/` and shared UI primitives in `components/ui/`. Add feature modules as workflows land, with route-level composition and feature-local API calls, components, schemas and tests as needed. Shared modules should be independent of feature internals.

Use TanStack Query for server state. Partition query keys by workspace and filters, isolate SSR caches per request/user, and clear affected caches on logout or identity changes. Keep transient state local when possible. Put business authorization and business mutations in the backend. Preserve accessible controls, loading/empty/error states, and user-local display of UTC timestamps.
