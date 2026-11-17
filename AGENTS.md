# TeamFlow repository instructions

## Scope and project purpose
TeamFlow is a project and issue management application intended to support multiple workspaces. Prioritize understandable code, correct business rules, tenant isolation, and reliable persistence.

Read `backend/AGENTS.md` for backend changes and `frontend/AGENTS.md` for frontend changes. Keep shared rules here and layer-specific guidance in those files.

## Current repository
- `backend/TeamFlow.slnx`: ASP.NET Core solution targeting `net11.0`, with Domain, Application, Infrastructure, API, Worker, and three test projects.
- `frontend/`: React 19, strict TypeScript, TanStack Start/Router/Query, Tailwind CSS 4, and Vite+; Nitro provides the production server.
- `scripts/dev.mjs`: starts the API and frontend together and stops both if either exits.
- Backend business layers and tests are currently scaffolds. Identity, EF Core, PostgreSQL, tenant authorization, and an outbox are intended architecture, not existing capabilities. Inspect the code before assuming they are configured.
- Do not introduce Aspire, caching services, additional hosts, or deployment infrastructure as incidental changes.

## Development commands
From the repository root:
```sh
npm run dev
dotnet restore backend/TeamFlow.slnx
dotnet build backend/TeamFlow.slnx --no-restore
dotnet test backend/TeamFlow.slnx --no-build
```
On Windows, use `npm.cmd` if PowerShell blocks `npm.ps1`. The launcher needs Node.js, the SDK required by the project files, Vite+ on PATH, and frontend dependencies installed with `vp install` from `frontend/`.

Frontend runs at `http://localhost:3000`; API HTTPS runs at `https://localhost:7045`. Trust the ASP.NET development certificate if needed with `dotnet dev-certs https --trust`.

From `frontend/`, run `vp run build` and `vp check`. Run applicable tests with `vp test` once tests are present. There are currently no npm lint, typecheck, or test scripts; do not invent them. Consult the frontend instructions for Vite+ command semantics.

## Shared conventions
- Follow existing patterns; keep changes focused and avoid unrelated reformatting or dependency upgrades.
- Use English for identifiers and technical documentation unless localization is requested.
- Backend owns business rules, authorization, and persistence constraints. Frontend validation improves usability and does not establish authorization.
- Update backend contracts and frontend callers together. Use generated API types only if generation is configured; never hand-edit generated outputs.
- The development proxy currently removes `/api` before forwarding. Coordinate proxy and endpoint changes; do not assume `/api/v1` is already configured.
- Use UTC for persisted timestamps and user-local time for display. Prefer Guid identifiers for new business entities unless an established model requires otherwise.
- Explain the purpose and trade-offs of new dependencies. Do not add abstractions solely for hypothetical future needs.

## Security and data
- Every new tenant-owned operation must verify workspace membership and scope reads and writes to the authorized workspace. Never trust client-provided IDs or roles as proof of access.
- Use framework-supported authentication. If Identity/cookie authentication is introduced, implement deliberate CSRF, cookie, and origin protections alongside it.
- Never commit secrets, private keys, production connection strings, or real credentials. Keep examples sanitized and do not log tokens, cookies, or sensitive payloads.
- Do not edit generated files or build outputs such as `bin/`, `obj/`, `node_modules/`, `.output/`, or generated route trees directly.
- Once EF Core is introduced, use migrations for schema changes; never rewrite migrations applied in shared environments. Require explicit authorization for destructive data operations or production migrations.

## Completion
- Run checks appropriate to the changed behavior and report failures or checks not run, including the reason.
- Add meaningful tests for changed business rules, authorization, and other important behavior. Documentation-only edits do not require application tests.
- For tenant functionality, verify another workspace cannot read or mutate the resource.
- Update affected contracts, configuration examples, migrations, and documentation.
- Summarize changes, validation results, and material limitations. Do not describe scaffolds or placeholder tests as implemented features or behavioral coverage.
