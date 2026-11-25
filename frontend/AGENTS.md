# Frontend instructions

These instructions supplement `../AGENTS.md` for `frontend/`. Preserve the Vite+ toolchain guidance below.

## Current stack and layout
- React 19, strict TypeScript, TanStack Start with file-based TanStack Router, TanStack Query, Tailwind CSS 4, and Nitro.
- `src/routes/`: route files; `__root.tsx` defines the shared document/layout.
- `src/router.tsx`: router setup; `src/integrations/tanstack-query/`: query integration.
- `src/components/ui/`: reusable UI primitives; `src/lib/`: shared utilities; `src/styles.css`: shared styles.
- Use configured `#/*` or `@/*` aliases consistently with surrounding code.
- Add feature modules when needed, but preserve file-based routing under `src/routes/`. Do not reorganize unrelated files or add a second router/state library.
- Do not edit `src/routeTree.gen.ts` by hand; regenerate through the configured tooling (`vp run generate-routes` invokes the build).

## API and server rendering
- Browser API requests use relative `/api/...` URLs. Nitro forwards them with the prefix intact to `API_PROXY_TARGET` or `https://localhost:7045` in development. The API uses `/api` as a development base path.
- Do not assume versioned routes exist.
- Development certificate verification is disabled only in the local proxy. Production requires a deliberate API routing/proxy arrangement; Vite's dev proxy is not production configuration.
- Centralize shared backend request/error handling when adding real features. Keep response types aligned with backend DTOs; do not assume an API generator exists.
- TanStack Start renders on the server: keep browser-only APIs out of server execution and secrets out of client bundles. Do not move backend business rules into frontend server functions.
- Keep server query clients and authenticated caches isolated per request/user. Never share one user's data through module-level mutable server state.
- When authentication is implemented, handle credentials and CSRF according to the backend contract. Never store session secrets in localStorage.
- Handle authentication, permission, validation, and edit-conflict responses explicitly; route guards do not replace backend authorization.

## State and UI
- Use TanStack Query for server data and integrate route loaders with existing query setup where appropriate. Include workspace and relevant filters in query keys.
- Clear affected caches on logout or identity/workspace changes. Use optimistic mutations only with rollback and conflict handling.
- Prefer local React state for transient UI. Do not add Zustand or duplicate server data in a client store without a demonstrated requirement.
- Reuse existing UI primitives, Tailwind tokens, and utility conventions. Avoid `any`, unsafe casts, and duplicated contract types.
- Handle loading, empty, error, and success states for data-driven views. Provide useful field-level validation feedback; Zod and TanStack Form are available if suitable.
- Use semantic controls, labels, visible keyboard focus, accessible dialogs, and responsive layouts. Check affected desktop/mobile widths for visual changes.
- Convert UTC timestamps at the display boundary and keep formatting consistent.

## Commands and completion
Run from `frontend/`:
```sh
vp install
vp run dev
vp run build
vp check
```
- `vp run dev` uses the package script with port 3000; `vp run build` builds the TanStack Start/Nitro application.
- The production start script is `vp run start` after building; do not assume this app is a static-only Vite build.
- Use `vp test` for relevant tests once present. No frontend test suite or Playwright configuration is currently established; do not claim coverage or invoke nonexistent npm scripts.
- Add behavior-oriented tests for important changed interactions when test tooling is established. Avoid large snapshots that merely mirror markup.
- Report build/check/test outcomes, visual verification for UI changes, and any missing setup. Documentation-only changes do not require running the application.

<!--VITE PLUS START-->

# Using Vite+, the Unified Toolchain for the Web

This project is using Vite+, a unified toolchain built on top of Vite, Rolldown, Vitest, tsdown, Oxlint, Oxfmt, and Vite Task. Vite+ wraps runtime management, package management, and frontend tooling in a single global CLI called `vp`. Vite+ is distinct from Vite, and it invokes Vite through `vp dev` and `vp build`. Run `vp help` to print a list of commands and `vp <command> --help` for information about a specific command.

Docs are local at `node_modules/vite-plus/docs` or online at https://viteplus.dev/guide/.

## Built-in Commands vs Scripts

`vp <name>` runs a built-in command. `vp run <name>` runs a `package.json` script or a `vite.config.ts` task. Scripts cannot overwrite built-ins, so `vp dev` and `vp run dev` may do different things. Check `package.json` and `vite.config.ts` first, and run `vp run <name>` when the project defines a script or task with that name.

## Running Scripts

Run TypeScript and JavaScript files with `vpx <file>`, for example `vpx ./scripts/seed.ts --dry-run`. It handles enums, decorators, JSX, and tsconfig `paths`, so do not add `tsx` or `ts-node` to run scripts. `vpx --watch <file>` reruns the file when a file it uses changes, in place of `tsx watch`. `vp node <file>` runs plain Node.js.

## Tool Versions

Run `vp toolchain` to show versions and relationships in the active Vite+
release. Add a tool name to select part of the graph. For example, run
`vp toolchain vite`. Use `--global` to ignore the local `vite-plus` package. Use
`vp why <package>` to show the package-manager dependency graph.

## Review Checklist

- [ ] Run `vp install` after pulling remote changes and before getting started.
- [ ] Run `vp check` and `vp test` to format, lint, type check and test changes.
- [ ] Check if there are `vite.config.ts` tasks or `package.json` scripts necessary for validation, run via `vp run <script>`.
- [ ] If setup, runtime, or package-manager behavior looks wrong, run `vp env doctor` and include its output when asking for help.

<!--VITE PLUS END-->
