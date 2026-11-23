# Backup and restore

Status: planned for PostgreSQL rollout. This repository has no configured database, backup schedule, recovery target, or restore automation. Identify the production provider, backup operator, retention period, recovery point objective (RPO), recovery time objective (RTO), and access policy before launch.

## Backup policy

- Use access-controlled backups and point-in-time recovery if available from the selected provider. Document schedule, retention, encryption, access, and monitoring for failed backups.
- Include both business and Identity data in the recovery plan and preserve a consistent point in time if they span stores. Record schema migration state and deployed image digests.
- Account separately for external attachments, Data Protection keys, and required secrets. Store secret access details in the approved secret store.
- Verify backups by restoring into an isolated, access-controlled database on a documented cadence.

## Restore exercise

1. Select a restore point, confirm the expected data-loss window, and record the source and target.
2. Restore into an isolated environment with live email and other external side effects disabled. Verify schema/migration state and use a compatible application version.
3. Check login, workspace ownership, representative row counts, tenant boundaries, and attachment access where applicable.
4. Review pending outbox work and idempotency records before starting workers. A database restore can replay previously delivered effects, especially external email.
5. Record achieved RPO/RTO, validation results and any repair work. Obtain explicit approval before a production restore or traffic cutover.

For an application rollback with a compatible database, follow [deployment](deployment.md). Treat production database replacement and point-in-time recovery as separate operations.
