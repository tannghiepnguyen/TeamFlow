# Verification and acceptance

The checkout has scaffold test projects and no established frontend test suite. Use the commands below to check the current checkout. Add meaningful assertions as releases implement the [functional requirements](../requirements/user-stories.md) and [quality targets](../requirements/scope.md).

## Current checks

From the repository root:

```powershell
dotnet restore backend/TeamFlow.slnx
dotnet build backend/TeamFlow.slnx --no-restore
dotnet test backend/TeamFlow.slnx --no-build
```

From `frontend/`:

```powershell
vp check
vp run build
```

## Planned coverage

| Test level | Coverage |
| --- | --- |
| Unit | Entity invariants, command/query validation and authorization outcomes |
| Integration | Cookies and CSRF, tenant-scoped API and SQL behavior, database constraints, migrations and outbox transactions using disposable PostgreSQL via Testcontainers |
| Architecture | Allowed project/namespace dependencies and host independence |
| Frontend | Data-dependent UI, keyboard interaction, state transitions and error recovery using the frontend test runner once configured |
| Browser | Playwright journeys for registration, workspace creation, issue edits, permission denial and conflict recovery when configured |
| Performance | Reproduce NFR-P01/P02 traffic and dataset, record p95/error rate and environment, capture representative query plans for NFR-P03 |

## Acceptance scenarios

| Scenario | Applies to | Pass condition |
| --- | --- | --- |
| Create workspace | FR-W01 | Workspace and creator Admin membership commit together and roll back together on failure |
| Cross-tenant access | FR-W05, NFR-S01 | An actor in workspace A cannot read or write known resources in workspace B, including indirect associations |
| Last Admin removal | FR-W06 | Removing or demoting the final active Admin fails, including concurrent changes |
| Issue assignment | FR-I04 | A user who lacks active membership in the issue workspace cannot be assigned |
| Concurrent issue edits | FR-I07, NFR-C01 | A single write wins for the same expected version and the losing write returns 409 |
| Outbox crash after side effect | FR-N01–FR-N04, NFR-R01 | Retrying produces one logical recipient notification |
| Expired or reused invitation | FR-W04 | Expired/revoked tokens fail and an accepted invitation cannot create membership twice |
| Concurrent issue creation | FR-I01 | Project issue numbers stay unique under concurrent creates |
| Unauthorized SignalR update | FR-I09, FR-N06 | A client cannot subscribe to another workspace or receive another recipient's notifications |
| Offline Worker | FR-N02, NFR-R02 | Issue writes still succeed and queued notifications appear after the Worker resumes |
