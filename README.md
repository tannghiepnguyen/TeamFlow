# TeamFlow

TeamFlow is a full-stack project and issue management application being built to help teams organize work across separate workspaces. Its intended scope includes projects, issues, sprints, comments, notifications, and reporting, with workspace membership and role-based permissions.

The frontend and backend can run together locally. The backend includes the entity model, PostgreSQL schema migration, and optional development seed data. Business workflows and authentication are still to be implemented.

## Current status

| Area | Current state |
| --- | --- |
| API | ASP.NET Core controllers with an antiforgery token endpoint, development OpenAPI, and Scalar |
| Backend layers | Domain entities and enums, plus Infrastructure models and EF Core configurations for the 16 TeamFlow models and custom Identity user fields |
| Background processing | Worker project scaffolded |
| Frontend | TanStack Start application with routing, query integration, Tailwind styling, and reusable UI primitives |
| Local workflow | One command starts the frontend and API together |
| Tests | PostgreSQL schema tests and opt-in PostgreSQL seeding tests, plus scaffold tests |
| Identity and persistence | A unified EF Core context, initial PostgreSQL migration, and opt-in demo records. Identity services and workspace authorization are planned |

## Technology

| Layer | Stack |
| --- | --- |
| Backend | C#, .NET 11 (`net11.0`), ASP.NET Core, Worker Service |
| API documentation | ASP.NET Core OpenAPI and Scalar |
| Frontend | React 19 and strict TypeScript |
| Routing and rendering | TanStack Start, TanStack Router, Nitro |
| Server data | TanStack Query |
| UI | Tailwind CSS 4, Radix UI, reusable components |
| Frontend tooling | Vite+ (`vp`) and pnpm |
| Backend tests | xUnit |
| Planned data and identity | PostgreSQL, EF Core/Npgsql, ASP.NET Core Identity |

## Repository structure

```text
TeamFlow/
|-- AGENTS.md
|-- README.md
|-- package.json                    # Combined development command
|-- scripts/dev.mjs                 # Starts and stops both applications
|-- backend/
|   |-- AGENTS.md
|   |-- TeamFlow.slnx
|   |-- src/
|   |   |-- TeamFlow.Domain/
|   |   |-- TeamFlow.Application/
|   |   |-- TeamFlow.Infrastructure/
|   |   |-- TeamFlow.API/
|   |   `-- TeamFlow.Worker/
|   `-- tests/
|       |-- TeamFlow.UnitTests/
|       |-- TeamFlow.IntegrationTests/
|       `-- TeamFlow.ArchitectureTests/
`-- frontend/
    |-- AGENTS.md
    |-- package.json
    |-- vite.config.ts
    |-- render.yaml                 # Frontend Render blueprint
    `-- src/
        |-- routes/                 # File-based routes and root layout
        |-- components/ui/          # Shared UI primitives
        |-- integrations/           # TanStack Query integration
        |-- lib/                    # Shared utilities
        |-- router.tsx
        `-- styles.css
```

## Architecture

The backend is organized around Clean Architecture boundaries:

- **Domain:** entities, value objects, and business invariants.
- **Application:** use cases, validation, authorization, DTOs, and infrastructure interfaces.
- **Infrastructure:** planned persistence, Identity, and external service implementations.
- **API:** HTTP endpoints, request/response mapping, and dependency injection composition.
- **Worker:** background processing when required by implemented features.

The production projects follow the [direct references in the architecture overview](doc/architecture/overview.md#layer-boundaries). Business rules and workspace authorization belong on the backend. The frontend owns presentation, user interaction, and server-data caching.

Business entities hold user and workspace identifiers. EF Core mappings define keys, tenant-scoped foreign keys, indexes, and the issue concurrency token. ASP.NET Core Identity supplies its own standard entity mappings and Infrastructure configures the additional user fields. See [Database migrations](doc/database/migrations.md) for context registration and verification.

The frontend uses TanStack Start with server rendering and a Nitro production server. Routes live in `frontend/src/routes/`; the tooling generates the route tree.

## Getting started

### Prerequisites

- Git.
- Node.js compatible with the installed frontend tooling, with npm available for the root launcher.
- A .NET SDK capable of building `net11.0` projects. Check installed SDKs with `dotnet --list-sdks`.
- Vite+ installed with `vp` on PATH. On Windows, the combined launcher invokes `vp.exe`.

The frontend declares pnpm `12.21.0` in `frontend/package.json` and includes a pnpm lockfile. Use Vite+ for frontend dependency management and commands. The app shell and antiforgery endpoint can run without PostgreSQL. See [Database migrations](doc/database/migrations.md) to create and seed a local database.

### Initial setup

From the repository root, restore backend dependencies:

```sh
dotnet restore backend/TeamFlow.slnx
```

Install frontend dependencies:

```sh
cd frontend
vp install
cd ..
```

Trust the ASP.NET HTTPS development certificate if it is not already configured:

```sh
dotnet dev-certs https --trust
```

### Run both applications

From the repository root:

```sh
npm run dev
```

In Windows PowerShell, use `npm.cmd run dev` if the execution policy blocks `npm.ps1`.

| Service | Local URL |
| --- | --- |
| Frontend | <http://localhost:3000> |
| Backend HTTPS | <https://localhost:7045> |
| Backend HTTP | <http://localhost:5107> |
| OpenAPI document (development) | <https://localhost:7045/openapi/v1.json> |
| Scalar API reference (development) | <https://localhost:7045/scalar/v1> |

The launcher runs the API with `dotnet watch` using its HTTPS profile and the frontend with `vp run dev`. Press Ctrl+C to stop both applications. If either process exits, the launcher stops the other. It uses Node.js built-ins and needs no root dependency installation. It does not start the Worker.

### Run applications separately

API, from the repository root:

```sh
dotnet watch run --project backend/src/TeamFlow.API --launch-profile https
```

Frontend, from a separate terminal:

```sh
cd frontend
vp run dev
```

## API proxy and configuration

In development, Nitro forwards `/api/*` requests to the backend, where ASP.NET Core treats `/api` as the request's base path:

```text
Browser:  /api/antiforgery/token
Backend:  https://localhost:7045/api/antiforgery/token
```

To use another backend target, create `frontend/.env.local` with:

```dotenv
API_PROXY_TARGET=https://localhost:7045
```

Restart the frontend after changing this value. The proxy is configured in `frontend/vite.config.ts`. Certificate verification is disabled for this development proxy to support local ASP.NET certificates.

The current API does not establish a versioned `/api/v1` convention. If endpoint prefixes change, update backend routes, the proxy, and frontend callers together.

Keep secrets out of source control and browser bundles. The API reads its PostgreSQL connection from `ConnectionStrings:TeamFlow`, with `ConnectionStrings:DefaultConnection` as a fallback.

## Trace API requests

Run `npm run dev` and open the browser’s Network tab. Select an API request, copy its `X-Request-Id` response header, and search the terminal output for that ID. For a quick check, open `http://localhost:3000/api/antiforgery/token` with the Network tab open.

API request logs include the HTTP method, route template, status code, and elapsed time. Successful requests and redirects appear at Information, client errors at Warning, and server errors at Error. The console prints the request ID on the request summary and any corresponding unhandled exception log. The request summary uses the route template and omits query strings, headers, cookies, and bodies. The exception log records the exception type and stack frames.

Set `Logging__LogLevel__TeamFlow` to `Debug` in the process environment to see TeamFlow’s debug messages. When the Worker runs separately, the same setting shows its heartbeat.

## Build and validation

Backend, from the repository root:

```sh
dotnet restore backend/TeamFlow.slnx
dotnet build backend/TeamFlow.slnx --no-restore
```

Run the built test assemblies listed in [Verification and acceptance](doc/development/testing.md).

Frontend, from `frontend/`:

```sh
vp check
vp run build
```

`vp check` runs the configured formatting, lint, and TypeScript checks. Use `vp test` for relevant frontend tests once a suite is established. There are currently no frontend npm lint, typecheck, or test scripts.

Use `vp run <script>` for package scripts: for example, `vp run dev` preserves the configured port 3000. Vite+ built-in commands and package scripts can have different behavior.

## Production and deployment

The frontend builds to a Nitro server application. From `frontend/`, build and start it with:

```sh
vp run build
vp run start
```

`frontend/render.yaml` provides a frontend Render blueprint. It does not deploy the .NET backend or a database. Production hosting must explicitly route browser API requests to the backend; Vite's development proxy is not included in the production server configuration.

A complete deployment still needs backend hosting, API routing, secrets, and the planned database/authentication configuration. The frontend is not currently configured to be served as static files by ASP.NET Core.

## Planned capabilities

- Account registration, login, and recovery through ASP.NET Core Identity.
- Workspaces, memberships, and workspace-scoped roles and permissions.
- Projects, issues, sprints, and comments.
- PostgreSQL persistence with EF Core migrations and concurrency protection.
- Notifications, reporting, and background processing as required.
- Meaningful automated tests, including cross-workspace access isolation.

These are intended capabilities, not implemented features or delivery commitments.

## Contributing

Keep changes focused and follow existing conventions. Update API contracts and frontend callers together, add meaningful tests for changed behavior, and report checks that could not be run.

Coding-agent guidance is maintained in [AGENTS.md](AGENTS.md), [backend/AGENTS.md](backend/AGENTS.md), and [frontend/AGENTS.md](frontend/AGENTS.md). Frontend-specific notes are available in [frontend/README.md](frontend/README.md).
