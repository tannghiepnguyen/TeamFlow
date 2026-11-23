# Product scope and quality targets

TeamFlow is a multi-tenant issue tracker for teams managing projects and issues. Each workspace is one tenant. A global account can belong to multiple workspaces with different roles. The checkout provides application scaffolding and entity definitions. These requirements describe planned releases.

## Actors

| Actor | Capabilities |
| --- | --- |
| Visitor | Register, log in, confirm email, reset password |
| Workspace Admin (including the creator) | Manage workspace settings, invitations, membership, projects and issues |
| Workspace member | View workspace, create/manage permitted project work, comment |
| Workspace viewer | Read project data and reports; no mutating operations |
| Background worker | Process outbox events, create notifications, dispatch email and real-time events |
| System operator | Deploy and monitor the system. Tenant business data requires separate authorization. |

## Release priorities

| Priority | Scope |
| --- | --- |
| P0 | Registration, confirmation, sessions and recovery. Workspaces, roles and invitations. Projects, issues and board, comments and labels. |
| P1 | Optimistic concurrency, activity history, durable outbox processing, in-app notifications, authorized live updates and basic reporting. |
| P2 | Sprints, attachments, optional channel preferences and email, advanced reports and exports, failed-event handling and other optional enhancements. |

Billing, a public marketplace, external SSO and a mobile client are outside P0. The source treats caching as optional P2. See the [functional catalog](user-stories.md) for requirement-level priorities.

## Quality targets

Record the machine type, data volume and traffic profile for performance runs. These targets apply to the documented portfolio test environment. NFR-D01 describes a release target. [Local development](../development/setup.md) describes the runnable checkout.

| ID | Category | Acceptance target | Priority |
| --- | --- | --- | --- |
| NFR-S01 | Security | All non-public APIs require authentication. Workspace resources require verified active membership. The test suite exercises tenant access paths with zero known authorization bypasses. | P0 |
| NFR-S02 | Security | Require HTTPS in deployment and set HttpOnly, Secure and appropriate SameSite attributes on cookies. Protect state-changing cookie-authenticated requests against CSRF. | P0 |
| NFR-S03 | Security | Keep credentials, tokens and secret keys out of logs and repositories. Use environment variables or a secret store. Identity handles passwords. | P0 |
| NFR-S04 | Security | Validate inputs, constrain uploads, encode user content and use parameterized EF queries. Audit dependencies. | P0 |
| NFR-R01 | Reliability | Business updates and outbox creation are atomic. Retries are idempotent. Fault-injection tests verify committed events survive. | P1 |
| NFR-R02 | Reliability | Issue creation continues during Worker failures. Retry and monitor pending work. | P1 |
| NFR-R03 | Reliability | Health endpoints distinguish process liveness from dependency readiness. Implement graceful shutdown. | P1 |
| NFR-P01 | Performance | At 100k issues across 50 workspaces, paginated issue list p95 < 500 ms in a documented local load scenario (excluding external network). | P1 |
| NFR-P02 | Performance | For a 20-concurrent-user demo load, core read endpoint error rate < 1% and p95 < 800 ms in a reproducible environment. | P1 |
| NFR-P03 | Performance | Representative dashboard queries have documented EXPLAIN ANALYZE plans and appropriate indexes. | P1 |
| NFR-C01 | Concurrency | For two writes with the same issue version, allow one to succeed and return an observable, tested conflict for the other. | P1 |
| NFR-M01 | Maintainability | Modules expose explicit contracts and access each other's capabilities through those contracts. | P0 |
| NFR-M02 | Maintainability | CI builds and runs formatting/static analysis, unit tests and integration tests before merge. | P1 |
| NFR-T01 | Testability | Cover critical business rules and all tenant access paths. Use disposable PostgreSQL via Testcontainers. | P1 |
| NFR-O01 | Observability | Record structured request, error, job and timing logs with correlation IDs. Trace and measure API and Worker activity. Keep personal data out of telemetry. | P1 |
| NFR-D01 | Deployment | Provide one-command local Docker Compose startup, documented production deployment and repeatable EF migrations. | P0 |
| NFR-D02 | Data integrity | Enforce core invariants using foreign keys and unique constraints. Store UTC timestamps, maintain backups, and document the restore procedure. | P1 |
| NFR-U01 | Usability | Responsive layouts down to 375px viewport, keyboard-operable common actions and clear loading/empty/error states. | P0 |
| NFR-U02 | Accessibility | Aim for WCAG 2.2 AA on main flows. Verify contrast, visible focus and accessible labels through tooling and manual checks. | P2 |
| NFR-X01 | Compatibility | Latest two major versions of Chromium/Firefox and current Safari used for manual smoke checks. | P2 |
| NFR-A01 | API quality | Publish OpenAPI, use stable versioned URL paths and RFC 9457 Problem Details, and cap pagination. | P0 |

## Proposed milestones

The week ranges are estimates measured from project kickoff.

| Milestone | Delivery | Exit criteria |
| --- | --- | --- |
| M1 · Weeks 1–2 | Identity + workspace | Registration, confirmation and login, atomic creator membership, and tenant access tests |
| M2 · Weeks 3–4 | Projects + issues + React MVP | CRUD, board, comments, labels, a deployed preview, and authorization tests |
| M3 · Weeks 5–6 | Concurrency + outbox | 409 conflicts, atomic event writes, and retry and idempotency tests |
| M4 · Weeks 7–8 | Reporting + optimization | Seed data, indexes, query plans, basic telemetry, measurements |
| M5 · Weeks 9–10 | Quality hardening | Playwright workflows, load tests, security negative cases, CI green |
| M6 · Weeks 11–12 | Public launch + portfolio | Demo, architecture docs, README, decisions, deployment and restore notes |

## Glossary

| Term | Meaning |
| --- | --- |
| Tenant | An independent workspace security/data boundary |
| Identity | ASP.NET Core user and authentication framework |
| RBAC | Role-based access control |
| Outbox | Database table holding domain events atomically with business changes |
| Idempotency | Repeated processing has the same logical effect as processing once |
| Optimistic concurrency | Conflict detection using an expected record version |
| P0/P1/P2 | Core / engineering depth / optional enhancement priority |

[Verification and acceptance](../development/testing.md)
