# Identity hosting and database contexts

Status: Proposed (source ADR-001 and ADR-002)  
Recorded: 2026-11-23

## Context

The application needs global accounts, workspace-local authorization, browser session protection and an atomic issue/activity/outbox write. The structure guide proposes separate Identity and business contexts and a single-origin ASP.NET static frontend host. The checkout runs a TanStack Start/Nitro frontend.

## Proposed decisions

Use ASP.NET Core Identity and same-site cookie sessions as the product baseline. Decide how production ingress, SSR, cookie attributes, antiforgery token delivery, allowed origins and persistent Data Protection keys fit together.

Consider a unified context or separate Identity and business contexts. Separate contexts allow independent schema ownership and migration histories. A unified context simplifies writes whose transaction spans both models. In either arrangement, keep issue, activity and outbox records in a shared business transaction.

## Consequences

The chosen ingress affects cookie scope, SSR request forwarding and proxy routing. The context choice affects migration ownership, user references and transaction coordination. Before accepting this record, establish the frontend/API topology, antiforgery flow, context names, schema boundaries and how any cross-context work remains consistent. [Migrations](../../database/migrations.md)
