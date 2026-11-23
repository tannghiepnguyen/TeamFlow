# TeamFlow engineering documentation

Start with [product scope](requirements/scope.md) for release priorities, [architecture](architecture/overview.md) for system boundaries, or [local setup](development/setup.md) to run the checkout.

| Area | Documents |
| --- | --- |
| Product | [Scope and quality targets](requirements/scope.md), [workspace roles](requirements/workspace-roles.md), [user stories and functional requirements](requirements/user-stories.md) |
| Architecture | [Overview](architecture/overview.md), [tenancy and authorization](architecture/tenancy-and-authorization.md), [outbox and notifications](architecture/outbox-and-notifications.md), [decision records](architecture/decisions/README.md) |
| Data and API | [Schema and field dictionary](database/schema.md), [migrations](database/migrations.md), [API conventions and proposed routes](api/conventions.md) |
| Development | [Setup](development/setup.md), [testing and acceptance](development/testing.md) |
| Operations | [Deployment](operations/deployment.md), [backup and restore](operations/backup-and-restore.md), [troubleshooting](operations/troubleshooting.md) |

The documentation uses three supplied references.

- **TeamFlow_Requirements_and_Data_Model.docx** (v1.0, 15 November 2026) supplies requirement IDs, priorities, business rules, proposed routes, milestones, acceptance targets, and ADR topics.
- **TeamFlow_Project_Structure.docx** supplies the documentation outline and proposed development and operating model.
- **TeamFlow_Entities.csv** supplies the field inventory, C# types, proposed PostgreSQL types, and enum values. The field dictionary retains framework-managed entries.

[Root README](../README.md) and the checkout describe current behavior. The reference designs assume .NET 10, a static Vite SPA, npm, and a different proxy and production layout. This checkout targets `net11.0` and uses Vite+, pnpm and TanStack Start/Nitro. Follow [local setup](development/setup.md) for runnable commands. Proposed database mappings, API routes, and release gates need implementation and verification before use. The product specification calls for one-command Compose startup, while the structure guide proposes a database-only default with native hosts. [ADR-001](architecture/decisions/0001-monorepo-without-aspire.md) records the current development baseline and the open Compose delivery choice.

Update requirement IDs, contracts, schema constraints, and acceptance tests together when behavior changes. Generate an OpenAPI artifact from the running API once that workflow exists.
