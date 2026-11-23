# User stories and functional requirements

These are planned requirements. See [scope](scope.md) for measurable targets and [acceptance scenarios](../development/testing.md) for verification.

## Representative user stories

| Actor | Goal | Requirements |
| --- | --- | --- |
| Account holder | Confirm my email, sign in securely, manage my profile and recover my account | FR-A01–FR-A05 |
| Workspace Admin | Create a workspace, invite teammates, and keep at least one active Admin | FR-W01–FR-W06 |
| Workspace Member | Organize issues on a board, assign work to active teammates, and collaborate through comments and labels | FR-P01, FR-I01–FR-I06 |
| Workspace Viewer | Follow permitted project work and reports using read-only access | FR-I02, FR-R02–FR-R04 |
| Notification recipient | See and acknowledge my updates within the workspaces I can access | FR-N04–FR-N06 |
| System operator | Recover asynchronous processing after a failure without duplicating notifications | FR-N01–FR-N03, FR-N08 |

## Identity and access

| ID | Priority | Requirement |
| --- | --- | --- |
| FR-A01 | P0 | Register using a unique email address and compliant password. Reject invalid or duplicate accounts. |
| FR-A02 | P0 | Confirm email via a single-use, expiring link before regular workspace access. |
| FR-A03 | P0 | Sign in and sign out using ASP.NET Core Identity secure cookie sessions. |
| FR-A04 | P0 | Retrieve and edit current user display name/profile. |
| FR-A05 | P0 | Reset password with a single-use, expiring token and revoke relevant sessions as configured. |
| FR-A06 | P1 | Apply login rate limiting, failed-attempt lockout and security event audit. |
| FR-A07 | P2 | Offer 2FA using supported Identity mechanisms. |

## Workspace and tenant management

| ID | Priority | Requirement |
| --- | --- | --- |
| FR-W01 | P0 | Authenticated user creates a workspace and becomes its first Admin atomically. |
| FR-W02 | P0 | List only workspaces where user has active membership. |
| FR-W03 | P0 | Admin edits workspace settings and manages member roles. |
| FR-W04 | P0 | Admin creates/revokes expiring email invitations. Each invitee may accept an invitation exactly once. |
| FR-W05 | P0 | Every workspace operation checks current active membership and required permission. |
| FR-W06 | P0 | Prevent removing/demoting last active Admin. |
| FR-W07 | P1 | Audit membership and role changes. |
| FR-W08 | P2 | Archive a workspace and block mutations while archived. |

## Projects, issues and collaboration

| ID | Priority | Requirement |
| --- | --- | --- |
| FR-P01 | P0 | Admin/Member create projects according to the permission matrix. Project keys are unique within a workspace. |
| FR-P02 | P0 | List, search and archive projects, respecting tenant scope. |
| FR-I01 | P0 | Create an issue with project-local unique readable number. |
| FR-I02 | P0 | View issues in paginated, sortable, filterable list and Kanban board. |
| FR-I03 | P0 | Edit issue fields, including status, priority, assignee and due date. |
| FR-I04 | P0 | Reject assignment to users outside the active workspace membership. |
| FR-I05 | P0 | Add, edit and soft-delete comments subject to author/Admin rules. |
| FR-I06 | P0 | Create reusable labels and attach/detach labels from issues. |
| FR-I07 | P1 | Validate transitions and enforce optimistic concurrency on issue updates. |
| FR-I08 | P1 | Generate activity entries for significant issue changes. |
| FR-I09 | P1 | Push authorized task updates to connected clients using SignalR. |
| FR-I10 | P2 | Create sprints, associate issues and calculate sprint summaries. |
| FR-I11 | P2 | Upload/download/delete issue attachments with authorized object storage access. |

## Notifications and asynchronous processing

| ID | Priority | Requirement |
| --- | --- | --- |
| FR-N01 | P1 | Create an outbox event within the same transaction as issue assignment/status changes. |
| FR-N02 | P1 | Worker polls/claims pending events and retries failures with capped exponential backoff. |
| FR-N03 | P1 | Consumer processes events idempotently so repeat delivery produces one logical notification. |
| FR-N04 | P1 | Create user-facing in-app notifications for assignment and comments. |
| FR-N05 | P1 | Recipients can list, count unread, and mark their own notifications read. |
| FR-N06 | P1 | Deliver live notification hints over SignalR with server authorization. |
| FR-N07 | P2 | Send optional email notifications using a provider abstraction and user preferences. |
| FR-N08 | P2 | Provide operational handling for permanently failed/dead-letter events. |

## Reporting and user experience

| ID | Priority | Requirement |
| --- | --- | --- |
| FR-R01 | P0 | Provide responsive workspace navigation, project board, issue drawer and error states. |
| FR-R02 | P1 | Display issue counts by status, assignee and priority. |
| FR-R03 | P1 | Provide workspace-scoped date-range and project filters for reports. |
| FR-R04 | P1 | Support server-side pagination and sorting for large issue lists. |
| FR-R05 | P2 | Show sprint progress, completion velocity and cycle-time trends. |
| FR-R06 | P2 | Export CSV reports with tenant-specific authorization. |
