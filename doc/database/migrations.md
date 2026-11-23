# Database migrations

The checkout has no EF Core context, migration history, PostgreSQL connection configuration, or pinned EF CLI. Current setup runs without a database.

## Planned workflow

- Add EF Core/Npgsql and the Identity model deliberately. Decide the context/schema layout and migration history ownership before generating the initial migrations. [Identity and context decision](../architecture/decisions/0002-identity-and-app-contexts.md)
- Keep context mappings, migrations and snapshots under `TeamFlow.Infrastructure`. With separate contexts, the structure guide proposes `Identity/Migrations` and `Persistence/Migrations`, separate history tables and explicit configuration for each context.
- Pin compatible SDK, EF packages, Npgsql provider and local `dotnet-ef` tooling when introduced. Confirm context names and API startup wiring before publishing executable commands.
- Create a new migration for each schema change. Review the SQL, model snapshot, constraint validation, index creation, backfill behavior, and compatibility with existing data and application versions.
- Verify migrations against disposable PostgreSQL from an empty database and the previous supported schema. Test migration order and transactional behavior where multiple contexts are involved.
- Run reviewed SQL or migration bundles once as a controlled release step, with an explicit target database and authorization. Preserve applied migrations and avoid concurrent migration execution from API or Worker startup.

If Identity and business data use different contexts, keep issue, activity and outbox writes within one business transaction. Plan any cross-context operations explicitly. [Database schema](schema.md)

## Recovery

Use backward-compatible schema changes so a prior application image can run during rollback. For an incompatible change, follow the release recovery plan and restore test evidence. A database restore can discard later writes and needs separate authorization. [Backup and restore](../operations/backup-and-restore.md)
