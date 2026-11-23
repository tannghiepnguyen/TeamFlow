# Tenant isolation

Status: Proposed (source ADR-003)  
Recorded: 2026-11-23

## Context

Users can belong to multiple workspaces with different permissions. Shared PostgreSQL storage needs safeguards against missing tenant predicates and cross-workspace references.

## Proposed decision

Require current membership and action authorization in Application, constrain data access by workspace, and use composite tenant-scoped foreign keys. Evaluate PostgreSQL row-level security as an additional safeguard after deciding how API, Worker, migrations and pooled connections would set tenant context.

## Consequences

Application checks cover active role and business rules. Database constraints protect relationship integrity. Integration tests must verify both boundaries with two workspaces and known resource IDs. [Tenancy and authorization](../tenancy-and-authorization.md)
