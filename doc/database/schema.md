# PostgreSQL data model and field dictionary

The CSV supplies 165 fields across 23 models. The C# types and enum values reflect the source catalog. PostgreSQL types and constraints describe the proposed mapping. [Migrations](migrations.md)

## Relationships and ownership

```text
Global Identity user -> WorkspaceMember <- Workspace
Workspace -> invitations, projects, labels, outbox and notifications
Project -> ProjectIssueSequence, Issue, Sprint
Issue -> comments, attachments, activity and IssueLabel
Label -> IssueLabel
Issue -> optional Sprint in the same workspace and project
Outbox event -> ProcessedEvent ledger and recipient notifications (logical)
Global Identity user -> NotificationPreference
```

Use workspace IDs in tenant queries and composite foreign keys for tenant-owned relationships. Workspace is the tenant root. The source broadly requires `workspace_id` on tenant-owned rows. The CSV scopes the issue sequence through the project and records the consumer/event pair on the processing ledger. Confirm these indirect keys when implementing the mapping.

Use application validation and explicit database constraints. Define indexes for foreign keys, active memberships, board and list queries, pending outbox work and unread notifications. Preserve project-local issue numbering and stable enum names. Persist UTC instants as `timestamptz` and display user-local time. Restrict hard deletes of core records. [Archive and retention decision](../architecture/decisions/0006-archive-retention-and-recovery.md)

## Field inventory

A C# type ending in `?` indicates modeled nullability. Confirm SQL nullability, defaults and indexes when creating the mappings. Framework-owned Identity tables include `AspNetRoles`, `AspNetUserRoles`, `AspNetUserClaims`, `AspNetRoleClaims`, `AspNetUserLogins` and `AspNetUserTokens`.

### `Workspace`

Location: `TeamFlow.Domain/Entities`.

Primary key `Id`. Normalize `Slug` to lowercase and make it globally unique. Reference the creator user and create their Admin membership atomically. Archive via `ArchivedAt`.

| Property | C# type | PostgreSQL type | Enum values |
| --- | --- | --- | --- |
| `Id` | `Guid` | `uuid` | — |
| `CreatedByUserId` | `Guid` | `uuid` | — |
| `Name` | `string` | `varchar(150)` | — |
| `Slug` | `string` | `varchar(80)` | — |
| `Description` | `string?` | `varchar(500)` | — |
| `CreatedAt` | `DateTimeOffset` | `timestamptz` | — |
| `UpdatedAt` | `DateTimeOffset` | `timestamptz` | — |
| `ArchivedAt` | `DateTimeOffset?` | `timestamptz` | — |

### `WorkspaceMember`

Location: `TeamFlow.Domain/Entities`.

Composite primary key `(WorkspaceId, UserId)`. Workspace and user are foreign keys. `InvitedByUserId` optionally references the inviter. Check active membership and preserve at least one active Admin. `IsActive` defaults true.

| Property | C# type | PostgreSQL type | Enum values |
| --- | --- | --- | --- |
| `WorkspaceId` | `Guid` | `uuid` | — |
| `UserId` | `Guid` | `uuid` | — |
| `Role` | `WorkspaceRole` | `varchar(20)` | Admin, Member, Viewer |
| `JoinedAt` | `DateTimeOffset` | `timestamptz` | — |
| `InvitedByUserId` | `Guid?` | `uuid` | — |
| `IsActive` | `bool` | `boolean` | — |

### `WorkspaceInvitation`

Location: `TeamFlow.Domain/Entities`.

Primary key `Id`. Workspace and inviter are foreign keys. Persist only the token hash. Validate the intended email, allowed role, expiry, and unused/unrevoked state. Accept and create membership atomically.

| Property | C# type | PostgreSQL type | Enum values |
| --- | --- | --- | --- |
| `Id` | `Guid` | `uuid` | — |
| `WorkspaceId` | `Guid` | `uuid` | — |
| `InvitedByUserId` | `Guid` | `uuid` | — |
| `EmailNormalized` | `string` | `varchar(256)` | — |
| `Role` | `WorkspaceRole` | `varchar(20)` | Admin, Member, Viewer |
| `TokenHash` | `string` | `varchar(128)` | — |
| `ExpiresAt` | `DateTimeOffset` | `timestamptz` | — |
| `CreatedAt` | `DateTimeOffset` | `timestamptz` | — |
| `AcceptedAt` | `DateTimeOffset?` | `timestamptz` | — |
| `RevokedAt` | `DateTimeOffset?` | `timestamptz` | — |

### `Project`

Location: `TeamFlow.Domain/Entities`.

Primary key `Id`, tenant alternate key `(WorkspaceId, Id)`, and unique `(WorkspaceId, ProjectKey)`. Normalize the key to uppercase. Reference the workspace and creator. Archive via `ArchivedAt`.

| Property | C# type | PostgreSQL type | Enum values |
| --- | --- | --- | --- |
| `Id` | `Guid` | `uuid` | — |
| `WorkspaceId` | `Guid` | `uuid` | — |
| `CreatedByUserId` | `Guid` | `uuid` | — |
| `Name` | `string` | `varchar(150)` | — |
| `ProjectKey` | `string` | `varchar(12)` | — |
| `Description` | `string?` | `text` | — |
| `CreatedAt` | `DateTimeOffset` | `timestamptz` | — |
| `UpdatedAt` | `DateTimeOffset` | `timestamptz` | — |
| `ArchivedAt` | `DateTimeOffset?` | `timestamptz` | — |

### `Issue`

Location: `TeamFlow.Domain/Entities`.

Primary key `Id`, tenant alternate key `(WorkspaceId, Id)` and unique `(ProjectId, IssueNumber)`. Use a composite project FK. Reporter and optional assignee reference users. An assignee must be active in the same workspace. The optional sprint must match the workspace and project. Sanitize rendered Markdown. Define the transition matrix for FR-I07 and condition P1 updates on the expected version. Increment the version on success.

| Property | C# type | PostgreSQL type | Enum values |
| --- | --- | --- | --- |
| `Id` | `Guid` | `uuid` | — |
| `WorkspaceId` | `Guid` | `uuid` | — |
| `ProjectId` | `Guid` | `uuid` | — |
| `ReporterUserId` | `Guid` | `uuid` | — |
| `AssigneeUserId` | `Guid?` | `uuid` | — |
| `SprintId` | `Guid?` | `uuid` | — |
| `IssueNumber` | `int` | `integer` | — |
| `Title` | `string` | `varchar(250)` | — |
| `Description` | `string?` | `text` | — |
| `IssueType` | `IssueType` | `varchar(20)` | Task, Bug, Story |
| `Status` | `IssueStatus` | `varchar(30)` | Todo, InProgress, InReview, Done, Cancelled |
| `Priority` | `IssuePriority` | `varchar(20)` | Low, Medium, High, Urgent |
| `Position` | `decimal` | `numeric(20,10)` | — |
| `Version` | `long` | `bigint` | — |
| `CreatedAt` | `DateTimeOffset` | `timestamptz` | — |
| `UpdatedAt` | `DateTimeOffset` | `timestamptz` | — |
| `DueAt` | `DateTimeOffset?` | `timestamptz` | — |
| `CompletedAt` | `DateTimeOffset?` | `timestamptz` | — |
| `ArchivedAt` | `DateTimeOffset?` | `timestamptz` | — |

### `Comment`

Location: `TeamFlow.Domain/Entities`.

Primary key `Id` with tenant-scoped issue FK and author user FK. Body is required and capped at 10,000 characters. Apply author/Admin permissions and retain audit metadata when soft-deleting via `DeletedAt`.

| Property | C# type | PostgreSQL type | Enum values |
| --- | --- | --- | --- |
| `Id` | `Guid` | `uuid` | — |
| `WorkspaceId` | `Guid` | `uuid` | — |
| `IssueId` | `Guid` | `uuid` | — |
| `AuthorUserId` | `Guid` | `uuid` | — |
| `Body` | `string` | `text` | — |
| `CreatedAt` | `DateTimeOffset` | `timestamptz` | — |
| `UpdatedAt` | `DateTimeOffset` | `timestamptz` | — |
| `DeletedAt` | `DateTimeOffset?` | `timestamptz` | — |

### `Label`

Location: `TeamFlow.Domain/Entities`.

Primary key `Id` and tenant alternate key `(WorkspaceId, Id)`. Enforce case-insensitive uniqueness with a unique index on `(WorkspaceId, NormalizedName)` using a database-generated `lower(Name)` field. Reference the workspace and validate `Color` as `#RRGGBB`.

| Property | C# type | PostgreSQL type | Enum values |
| --- | --- | --- | --- |
| `Id` | `Guid` | `uuid` | — |
| `WorkspaceId` | `Guid` | `uuid` | — |
| `Name` | `string` | `varchar(60)` | — |
| `Color` | `string` | `varchar(7)` | — |
| `CreatedAt` | `DateTimeOffset` | `timestamptz` | — |

### `IssueLabel`

Location: `TeamFlow.Domain/Entities`.

Composite primary key `(IssueId, LabelId)`. Use workspace-scoped foreign keys to Issue and Label to block cross-tenant tagging.

| Property | C# type | PostgreSQL type | Enum values |
| --- | --- | --- | --- |
| `WorkspaceId` | `Guid` | `uuid` | — |
| `IssueId` | `Guid` | `uuid` | — |
| `LabelId` | `Guid` | `uuid` | — |

### `Sprint`

Location: `TeamFlow.Domain/Entities`.

P2. Primary key `Id` and tenant-scoped project FK. Enforce `StartAt < EndAt` and at most one active sprint per project, with concurrency-safe enforcement.

| Property | C# type | PostgreSQL type | Enum values |
| --- | --- | --- | --- |
| `Id` | `Guid` | `uuid` | — |
| `WorkspaceId` | `Guid` | `uuid` | — |
| `ProjectId` | `Guid` | `uuid` | — |
| `Name` | `string` | `varchar(120)` | — |
| `Goal` | `string?` | `text` | — |
| `StartAt` | `DateTimeOffset` | `timestamptz` | — |
| `EndAt` | `DateTimeOffset` | `timestamptz` | — |
| `CreatedAt` | `DateTimeOffset` | `timestamptz` | — |
| `Status` | `SprintStatus` | `varchar(20)` | Planned, Active, Completed |

### `IssueAttachment`

Location: `TeamFlow.Domain/Entities`.

P2. Primary key `Id`, tenant-scoped issue FK and uploader user FK. Validate content type and enforce quotas. Store objects privately under random keys, treat the original filename as display text, and authorize short-lived download URLs. Soft-delete through `DeletedAt`. Define malware scanning if adopted.

| Property | C# type | PostgreSQL type | Enum values |
| --- | --- | --- | --- |
| `Id` | `Guid` | `uuid` | — |
| `WorkspaceId` | `Guid` | `uuid` | — |
| `IssueId` | `Guid` | `uuid` | — |
| `UploadedByUserId` | `Guid` | `uuid` | — |
| `OriginalFileName` | `string` | `varchar(255)` | — |
| `ObjectKey` | `string` | `text` | — |
| `ContentType` | `string` | `varchar(120)` | — |
| `SizeBytes` | `long` | `bigint` | — |
| `CreatedAt` | `DateTimeOffset` | `timestamptz` | — |
| `DeletedAt` | `DateTimeOffset?` | `timestamptz` | — |

### `ActivityLog`

Location: `TeamFlow.Domain/Entities`.

Append-only issue history. Primary key `Id`, tenant-scoped issue FK and optional actor user FK. Store redacted before/after data. Example actions include `Created`, `Updated`, `Assigned`, `StatusChanged` and `CommentAdded`. FR-W07 membership audit needs a model decision because this record requires an issue.

| Property | C# type | PostgreSQL type | Enum values |
| --- | --- | --- | --- |
| `Id` | `Guid` | `uuid` | — |
| `WorkspaceId` | `Guid` | `uuid` | — |
| `IssueId` | `Guid` | `uuid` | — |
| `ActorUserId` | `Guid?` | `uuid` | — |
| `Action` | `string` | `varchar(60)` | — |
| `Changes` | `JsonElement` | `jsonb` | — |
| `OccurredAt` | `DateTimeOffset` | `timestamptz` | — |

### `Notification`

Location: `TeamFlow.Domain/Entities`.

Primary key `Id` and recipient user FK. Recommended unique key `(RecipientUserId, SourceEventId, Type)`. Keep payload minimal and verify active workspace membership plus recipient ownership for reads and mark-read operations.

| Property | C# type | PostgreSQL type | Enum values |
| --- | --- | --- | --- |
| `Id` | `Guid` | `uuid` | — |
| `WorkspaceId` | `Guid` | `uuid` | — |
| `RecipientUserId` | `Guid` | `uuid` | — |
| `SourceEventId` | `Guid` | `uuid` | — |
| `Type` | `NotificationType` | `varchar(60)` | IssueAssigned, Mention, CommentAdded |
| `Payload` | `JsonElement` | `jsonb` | — |
| `CreatedAt` | `DateTimeOffset` | `timestamptz` | — |
| `ReadAt` | `DateTimeOffset?` | `timestamptz` | — |

### `NotificationPreference`

Location: `TeamFlow.Domain/Entities`.

P2. Global preferences keyed by `(UserId, EventType)`. Define defaults and keep mandatory account-security mail outside opt-out.

| Property | C# type | PostgreSQL type | Enum values |
| --- | --- | --- | --- |
| `UserId` | `Guid` | `uuid` | — |
| `EventType` | `string` | `varchar(60)` | — |
| `EmailEnabled` | `bool` | `boolean` | — |
| `InAppEnabled` | `bool` | `boolean` | — |

### `ProjectIssueSequence`

Location: `TeamFlow.Infrastructure/Persistence`.

Primary key `ProjectId` also references Project. `NextNumber` starts at one. Allocate within the issue-create transaction using a locked row or atomic counter.

| Property | C# type | PostgreSQL type | Enum values |
| --- | --- | --- | --- |
| `ProjectId` | `Guid` | `uuid` | — |
| `NextNumber` | `int` | `integer` | — |

### `OutboxMessage`

Location: `TeamFlow.Infrastructure/Outbox`.

Primary key `Id` is the event ID. Persist workspace context and versioned payload in the business transaction. Attempts default to zero. Track availability, processing and worker leases. Sanitize errors.

| Property | C# type | PostgreSQL type | Enum values |
| --- | --- | --- | --- |
| `Id` | `Guid` | `uuid` | — |
| `WorkspaceId` | `Guid` | `uuid` | — |
| `AggregateId` | `Guid` | `uuid` | — |
| `EventType` | `string` | `varchar(150)` | — |
| `AggregateType` | `string` | `varchar(80)` | — |
| `Payload` | `JsonElement` | `jsonb` | — |
| `OccurredAt` | `DateTimeOffset` | `timestamptz` | — |
| `AvailableAt` | `DateTimeOffset` | `timestamptz` | — |
| `ProcessedAt` | `DateTimeOffset?` | `timestamptz` | — |
| `LockedUntil` | `DateTimeOffset?` | `timestamptz` | — |
| `Attempts` | `int` | `integer` | — |
| `LastError` | `string?` | `text` | — |
| `LockedBy` | `string?` | `varchar(100)` | — |

### `ProcessedEvent`

Location: `TeamFlow.Infrastructure/Outbox`.

Composite primary key `(ConsumerName, EventId)` with a logical reference to the event. Persist alongside the database side effect in the same transaction.

| Property | C# type | PostgreSQL type | Enum values |
| --- | --- | --- | --- |
| `ConsumerName` | `string` | `varchar(100)` | — |
| `EventId` | `Guid` | `uuid` | — |
| `ProcessedAt` | `DateTimeOffset` | `timestamptz` | — |

### `ApplicationUser`

Location: `TeamFlow.Infrastructure/Identity`.

Global account in `AspNetUsers`. Extends `IdentityUser<Guid>`. Identity owns the inherited mappings. Enforce unique normalized email, confirmation before normal access, and deactivation. Keep credential hashes and security stamps out of APIs and logs.

| Property | C# type | PostgreSQL type | Enum values |
| --- | --- | --- | --- |
| `Id` | `Guid` | `uuid` | — |
| `UserName` | `string?` | `varchar(256)` | — |
| `NormalizedUserName` | `string?` | `varchar(256)` | — |
| `Email` | `string?` | `varchar(256)` | — |
| `NormalizedEmail` | `string?` | `varchar(256)` | — |
| `EmailConfirmed` | `bool` | `boolean` | — |
| `PhoneNumberConfirmed` | `bool` | `boolean` | — |
| `TwoFactorEnabled` | `bool` | `boolean` | — |
| `LockoutEnabled` | `bool` | `boolean` | — |
| `IsDeactivated` | `bool` | `boolean` | — |
| `PasswordHash` | `string?` | `text` | — |
| `SecurityStamp` | `string?` | `text` | — |
| `ConcurrencyStamp` | `string?` | `text` | — |
| `PhoneNumber` | `string?` | `text` | — |
| `AvatarUrl` | `string?` | `text` | — |
| `LockoutEnd` | `DateTimeOffset?` | `timestamptz` | — |
| `AccessFailedCount` | `int` | `integer` | — |
| `DisplayName` | `string` | `varchar(120)` | — |
| `CreatedAt` | `DateTimeOffset` | `timestamptz` | — |
| `UpdatedAt` | `DateTimeOffset` | `timestamptz` | — |

### `IdentityRole<Guid>`

Location: `TeamFlow.Infrastructure/Identity`.

Optional global roles in `AspNetRoles`. Workspace roles are defined on memberships.

| Property | C# type | PostgreSQL type | Enum values |
| --- | --- | --- | --- |
| `Id` | `Guid` | `uuid` | — |
| `Name` | `string?` | `varchar(256)` | — |
| `NormalizedName` | `string?` | `varchar(256)` | — |
| `ConcurrencyStamp` | `string?` | `text` | — |

### `IdentityUserRole<Guid>`

Location: `TeamFlow.Infrastructure/Identity`.

Identity-managed model. Use the framework's schema migrations.

| Property | C# type | PostgreSQL type | Enum values |
| --- | --- | --- | --- |
| `UserId` | `Guid` | Identity mapping | — |
| `RoleId` | `Guid` | Identity mapping | — |

### `IdentityUserClaim<Guid>`

Location: `TeamFlow.Infrastructure/Identity`.

Identity-managed model. Use the framework's schema migrations.

| Property | C# type | PostgreSQL type | Enum values |
| --- | --- | --- | --- |
| `Id` | `int` | Identity mapping | — |
| `UserId` | `Guid` | Identity mapping | — |
| `ClaimType` | `string?` | Identity mapping | — |
| `ClaimValue` | `string?` | Identity mapping | — |

### `IdentityRoleClaim<Guid>`

Location: `TeamFlow.Infrastructure/Identity`.

Identity-managed model. Use the framework's schema migrations.

| Property | C# type | PostgreSQL type | Enum values |
| --- | --- | --- | --- |
| `Id` | `int` | Identity mapping | — |
| `RoleId` | `Guid` | Identity mapping | — |
| `ClaimType` | `string?` | Identity mapping | — |
| `ClaimValue` | `string?` | Identity mapping | — |

### `IdentityUserLogin<Guid>`

Location: `TeamFlow.Infrastructure/Identity`.

Identity-managed model. Use the framework's schema migrations.

| Property | C# type | PostgreSQL type | Enum values |
| --- | --- | --- | --- |
| `LoginProvider` | `string` | Identity mapping | — |
| `ProviderKey` | `string` | Identity mapping | — |
| `UserId` | `Guid` | Identity mapping | — |

### `IdentityUserToken<Guid>`

Location: `TeamFlow.Infrastructure/Identity`.

Identity-managed model. Use the framework's schema migrations.

| Property | C# type | PostgreSQL type | Enum values |
| --- | --- | --- | --- |
| `UserId` | `Guid` | Identity mapping | — |
| `LoginProvider` | `string` | Identity mapping | — |
| `Name` | `string` | Identity mapping | — |
| `Value` | `string?` | Identity mapping | — |
