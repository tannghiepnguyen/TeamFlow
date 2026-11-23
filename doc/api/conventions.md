# API conventions and proposed routes

The current controller sample is `GET /weatherforecast`. Its browser-facing development path is `/api/weatherforecast` because Vite strips `/api` before forwarding. OpenAPI and Scalar are available in Development. Coordinate the planned versioned route prefix with backend routes, frontend callers, development proxy and production ingress.

## Request and response rules

- Use request and response DTOs. Validate route, query and body inputs. Keep Identity internals and persistence models out of responses.
- Use Problem Details (RFC 9457). Use 400 for invalid input, 401 for missing authentication, 403 for forbidden operations, 404 for missing or intentionally concealed resources, and 409 for conflicts. For stale issue writes, include current version metadata when authorized and appropriate.
- Require verified active membership and permission for tenant routes. Account, workspace creation and invitation flows have their own checks. See [workspace roles](../requirements/workspace-roles.md).
- Paginate unbounded lists with a documented cap and stable sorting. Specify allowed filters, sort keys, tie-breaker, page parameters and response envelope when implemented.
- Use same-site Identity cookies and antiforgery validation on state-changing browser requests. Define the token exchange, cookie attributes and allowed origins in the [hosting decision](../architecture/decisions/0002-identity-and-app-contexts.md).
- For optimistic concurrency, accept the expected version and condition the write on it. Keep frontend types and conflict handling aligned with OpenAPI.

## Proposed surface

`{wid}` is the workspace ID, `{pid}` the project ID, and `{iid}` the issue ID. Role names express minimum access.

| Method | Route | Permission or purpose |
| --- | --- | --- |
| `POST` | `/api/v1/auth/login` | Public Identity login. The framework route may differ. |
| `POST` | `/api/v1/auth/logout` | Authenticated |
| `GET` | `/api/v1/me` | Authenticated self-profile |
| `GET/POST` | `/api/v1/workspaces` | Member list / authenticated create |
| `GET/PATCH` | `/api/v1/workspaces/{wid}` | Viewer read / Admin update |
| `GET/POST` | `/api/v1/workspaces/{wid}/members` | Member read / Admin invite flow |
| `POST` | `/api/v1/workspaces/{wid}/invitations` | Admin |
| `POST` | `/api/v1/invitations/accept` | Authenticated with token |
| `GET/POST` | `/api/v1/workspaces/{wid}/projects` | Viewer read / Member create |
| `GET/POST` | `/api/v1/workspaces/{wid}/projects/{pid}/issues` | Viewer read / Member create |
| `GET/PATCH` | `/api/v1/workspaces/{wid}/issues/{iid}` | Viewer read / Member edit with version |
| `GET/POST` | `/api/v1/workspaces/{wid}/issues/{iid}/comments` | Viewer read / Member comment |
| `GET/PATCH` | `/api/v1/workspaces/{wid}/notifications` | Recipient only |
| `GET` | `/api/v1/workspaces/{wid}/reports/summary` | Viewer |
| `WS` | `/hubs/workspaces` | Authenticated + workspace authorization |

Choose one Identity endpoint mapping. If using framework-mapped endpoints, mount them deliberately and add adapters where custom routes are needed. Registration, confirmation, reset, profile edits, member management, invitation revocation, archival, comment edits/deletion, labels and detailed notification actions still need explicit route and DTO specifications. Resolve the roster read permission and choose a single invitation creation route before publishing the contract. Confirm P2 routes when those features enter scope.

Generate an OpenAPI artifact and frontend types once the workflow is configured. Check contract drift in CI if generated outputs are tracked.
