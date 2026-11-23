# Troubleshooting

## Current checkout

| Symptom | Check |
| --- | --- |
| `vp` unavailable or frontend fails to resolve packages | Confirm Vite+ is on PATH, including `vp.exe` on Windows, then run `vp install` from `frontend/`. |
| PowerShell policy blocks npm | Run `npm.cmd run dev` from the root. |
| SDK cannot target the project | Run `dotnet --list-sdks` and verify that the SDK supports `net11.0`. |
| One host stops just after launch | Read the first host error. Run hosts separately per [local setup](../development/setup.md) to isolate port or startup failures. |
| Proxy errors or unexpected 404 | Check `https://localhost:7045/weatherforecast` directly and `http://localhost:3000/api/weatherforecast` through Vite. Verify `API_PROXY_TARGET` and the prefix rewrite. |
| Local browser warns about HTTPS | Check the API launch profile and trust the ASP.NET development certificate when necessary. |
| Scalar/OpenAPI missing | Confirm the API uses the Development environment and check the routes in the [setup guide](../development/setup.md). |

## Once persistence and background processing ship

| Symptom | Check |
| --- | --- |
| Database connection fails | Compare native host and container connection settings, database readiness, and secret injection. Keep credential values out of diagnostic output. |
| API reachable but readiness fails | Inspect dependency health and migration state. Review release logs before applying or retrying a migration. |
| Cookie sign-in or CSRF failures | Check public origin, cookie attributes, antiforgery token flow, reverse proxy HTTPS headers and Data Protection key access. |
| Issue edit returns 409 | Reload the issue and review the latest version before resubmitting changes. A stale write must leave the committed data intact. |
| Notifications lag | Compare outbox age, available times, lease expiry, attempt counts and Worker readiness. Check persisted notifications before investigating live hints. |
| Unexpected tenant visibility | Check current membership, resource workspace predicates, related entity scope, cache isolation and SignalR authorization. Capture correlation IDs without sensitive payloads. |

A Compose stop preserves the named database volume if that setup is introduced. Volume removal requires explicit authorization. For release incidents see [deployment](deployment.md) and [backup and restore](backup-and-restore.md).
