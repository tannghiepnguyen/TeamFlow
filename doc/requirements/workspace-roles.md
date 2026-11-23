# Workspace roles and permissions

A membership connects a global account to one workspace with an active status and role. The creator becomes the first Admin atomically. The source uses “owner” for the creator and defines the roles `Admin`, `Member` and `Viewer`. Dedicated ownership transfer semantics remain open.

| Operation | Admin | Member | Viewer |
| --- | --- | --- | --- |
| View workspace/project/issue | Yes | Yes | Yes |
| Manage workspace settings | Yes | No | No |
| Invite/remove/change member role | Yes | No | No |
| Create project | Yes | Yes | No |
| Archive project | Yes | No | No |
| Create/edit issue | Yes | Yes | No |
| Comment | Yes | Yes | No |
| Edit own comment | Yes | Yes | No |
| Moderate/delete another comment | Yes | No | No |
| View reports | Yes | Yes | Yes |

## Membership and invitations

- Admins manage invitations and member roles. An invitation cannot grant privileges beyond the inviter's authority. Store only a token hash.
- Invitation acceptance is an authenticated entry flow for the intended email recipient. Verify that the token is unused, unexpired and unrevoked, and that the recipient lacks duplicate active membership. Commit acceptance and membership together.
- Require active membership on workspace operations. The workspace list includes active memberships. Workspace creation and accepting a valid invitation have their own entry checks.
- Guard the last active Admin against removal or demotion across concurrent writes. Audit membership and role changes in P1.
- FR-W08 proposes blocking mutations when a workspace is archived. Historical access, restoration and retention remain open.

[Tenancy and authorization](../architecture/tenancy-and-authorization.md)
