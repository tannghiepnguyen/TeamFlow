# Local development

## Prerequisites

Use a .NET SDK capable of targeting `net11.0`, Node.js with npm, and Vite+ (`vp`) on PATH. On Windows the root launcher invokes `vp.exe`. The frontend package declares pnpm and has a pnpm lockfile. The current scaffold runs without PostgreSQL or Docker.

## Run the checkout

From the root:

```powershell
dotnet restore backend/TeamFlow.slnx
Set-Location frontend
vp install
Set-Location ..
npm run dev
```

Use `npm.cmd run dev` if PowerShell blocks the npm script. Trust the ASP.NET development certificate with `dotnet dev-certs https --trust` if needed.

| Component | Address |
| --- | --- |
| Frontend | `http://localhost:3000` |
| API HTTPS | `https://localhost:7045` |
| API HTTP | `http://localhost:5107` |
| OpenAPI in Development | `https://localhost:7045/openapi/v1.json` |
| Scalar in Development | `https://localhost:7045/scalar/v1` |

Ctrl+C stops the two hosts. If one exits, the launcher stops the other. For separate terminals, run `dotnet watch run --project backend/src/TeamFlow.API --launch-profile https` from the root and `vp run dev` from `frontend/`. The combined launcher starts the API and frontend only. The Worker can be started separately with `dotnet run --project backend/src/TeamFlow.Worker` to exercise its scaffold.

Nitro forwards browser calls to `/api/*` with the prefix intact. ASP.NET Core uses `/api` as its development base path. Set `API_PROXY_TARGET` in `frontend/.env.local` and restart the frontend to override the default `https://localhost:7045`. Keep certificate-validation relaxation confined to the local development proxy.

## Planned database setup

The structure guide proposes PostgreSQL on loopback using a database-only Compose service and a named volume. The requirements also call for full stack Compose startup as a release target. Before adding database instructions, finalize local topology, context names, secret settings and migration commands.

Keep Compose interpolation secrets in an ignored local file where applicable. Configure native .NET host secrets separately through User Secrets or environment variables. Frontend variables exposed to the browser must contain public configuration only. [Database migrations](../database/migrations.md)
