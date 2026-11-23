# Tenancy and authorization

The application must authenticate and authorize each workspace-owned operation. A global Identity user can hold different roles in different workspaces. The server resolves current active membership from the authenticated user ID and requested workspace, applies the operation permission, and scopes reads and writes to that workspace. An ID or role supplied by a client is input to validate. [Workspace roles](../requirements/workspace-roles.md) defines the permission baseline.

## Request and data boundaries

1. Resolve the caller from the authenticated session. Account endpoints and workspace creation or invitation acceptance have their own authorization conditions.
2. Verify active membership and the action-specific permission for existing-workspace operations. For member and invitation changes, validate the actor and target under the same workspace.
3. Read or mutate the resource with its workspace predicate and validate linked records in the same scope. Assignments require current active membership. A sprint must also belong to the issue's project.
4. Add tenant-scoped foreign keys for persisted relationships and explicit application checks for rules such as active membership and the last Admin.
5. Return the established error response. A missing or concealed resource may return 404 under the policy defined in [API conventions](../api/conventions.md).

Use equivalent checks for reports, exports, downloads, notifications and SignalR. Authorize group subscriptions on the server and define access revocation for existing connections. Notification listings and read writes also require recipient ownership. Worker operations use explicit tenant context and a system actor with purpose-specific privileges.

## Session and browser controls

Use ASP.NET Core Identity for account credentials, tokens and supported lockout/2FA. Require email confirmation before regular access and honor product-level deactivation. For cookie sessions, configure HTTPS, HttpOnly, Secure, intentional SameSite, origin/CORS handling, and antiforgery validation on state-changing requests. Persist Data Protection keys across production restarts or replicas. Keep tokens, cookies and credentials out of logs and browser storage.

## Verification

Exercise each tenant-owned operation with two workspaces and known resource IDs. Cover unauthorized reads and writes, inactive membership, viewer mutations, cross-workspace associations, role changes, SignalR subscriptions and recipient-only notifications. Verify concurrent role changes cannot remove all active Admins. [Testing and acceptance](../development/testing.md)
