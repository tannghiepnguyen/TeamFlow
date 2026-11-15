# TeamFlow

## Local development

Prerequisites: Node.js, the .NET SDK required by the backend, and Vite+ (`vp`).

Install frontend dependencies once from the `frontend` directory with `vp install`.
If the ASP.NET HTTPS development certificate is not set up, run
`dotnet dev-certs https --trust` once.

From the project root, start both apps with:

```bash
npm run dev
```

In Windows PowerShell, use `npm.cmd run dev` if the execution policy blocks `npm.ps1`.

- Frontend: http://localhost:3000 (Vite hot reload)
- Backend: https://localhost:7045 (`dotnet watch` with the HTTPS launch profile)
- Frontend `/api/*` requests are proxied to the backend.

Press Ctrl+C to stop both apps. If either app exits, the launcher stops the other.
The root launcher uses Node.js built-ins and needs no additional dependencies.
