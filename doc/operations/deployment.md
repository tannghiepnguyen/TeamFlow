# Deployment and rollback

## Current checkout

The frontend builds and runs as a Nitro server with `vp run build` and `vp run start` from `frontend/`. The existing [Render blueprint](../../frontend/render.yaml) covers the frontend. A full application release still needs API hosting, production browser/API routing, PostgreSQL, authentication configuration, and Worker hosting when background jobs ship. Vite's development proxy applies only locally.

The structure guide proposes a single ASP.NET web image serving a static SPA plus a separate Worker image. The current frontend uses SSR through Nitro. Resolve the production hosting arrangement and same-site ingress before adopting the guide's Docker or static asset instructions. [Identity and hosting decision](../architecture/decisions/0002-identity-and-app-contexts.md)

## Planned release gates

1. Run backend restore/build/tests and frontend checks/build. Add disposable PostgreSQL integration tests and browser smoke tests as the workflows land.
2. Build deployable artifacts from the exact tested commit and record immutable image digests or equivalent artifact IDs. For a separate deploy workflow, verify the CI outcome, trusted repository and branch, and tested commit.
3. Deploy to staging. Inject database and email credentials from the host secret store. Configure HTTPS, trusted forwarded headers, cookie and origin policy, and persistent Data Protection keys.
4. Apply reviewed database migrations once as a controlled stage. Check API liveness and dependency readiness, then run login and workspace smoke tests. Verify Worker processing and realtime routing where released.
5. Promote the tested artifacts. Monitor error rates, latency, readiness, database connections and Worker queue lag. Keep request correlation IDs and exclude credentials, cookies and sensitive payloads.

The intended CI separates backend and frontend checks, uses a Docker-capable runner for PostgreSQL integration tests, and retains failure artifacts without secrets. Browser checks require seeded test data, isolated services and readiness checks. No complete CI/deploy workflow is configured in this checkout.

## Rollback

Record the previous known-good artifacts and migration state. Roll back the application artifacts when the database schema is compatible. Use expand-and-contract changes to preserve that option. Validate readiness and smoke tests after rollback. If database recovery is necessary, follow the separately authorized [backup and restore runbook](backup-and-restore.md).
