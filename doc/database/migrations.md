# Database migrations

Infrastructure contains [16 TeamFlow mappings](../../backend/src/TeamFlow.Infrastructure/Persistence/Configurations) and the [custom ApplicationUser mapping](../../backend/src/TeamFlow.Infrastructure/Identity/Configurations/ApplicationUserConfiguration.cs). ASP.NET Core Identity supplies the six remaining catalog mappings. A test-only Identity context loads all 23 entities and checks the SQL Npgsql generates. Current setup runs without a database.

## Planned workflow

- Decide the context/schema layout and migration history ownership before generating the initial migrations. [Identity and context decision](../architecture/decisions/0002-identity-and-app-contexts.md)
- A unified context can follow the test context by calling `base.OnModelCreating` on `IdentityDbContext` before applying the Infrastructure configurations. If separate contexts own the schemas, configure the business context's references to the Identity table so its migrations leave table ownership with Identity.
- Before generating the first migration, wire the application context and confirm table and column naming. The mappings currently use EF naming conventions for TeamFlow tables. Update and verify the SQL check constraints, computed columns, and filtered indexes together with any naming convention change.
- Keep normalization, tenant membership verification, and other application workflows in the use cases. Apply the schema constraints after reviewing the migration.
- Keep context mappings, migrations and snapshots under `TeamFlow.Infrastructure`. With separate contexts, the structure guide proposes `Identity/Migrations` and `Persistence/Migrations`, separate history tables and explicit configuration for each context.
- Pin compatible SDK, EF packages, Npgsql provider, and local `dotnet-ef` tooling. Confirm the application context and API startup wiring before publishing executable migration commands.
- Create a new migration for each schema change. Review the SQL, model snapshot, constraint validation, index creation, backfill behavior, and compatibility with existing data and application versions.
- Verify migrations against disposable PostgreSQL from an empty database and the previous supported schema. Test migration order and transactional behavior where multiple contexts are involved.
- Run reviewed SQL or migration bundles once as a controlled release step, with an explicit target database and authorization. Preserve applied migrations and avoid concurrent migration execution from API or Worker startup.

If Identity and business data use different contexts, keep issue, activity and outbox writes within one business transaction. Plan any cross-context operations explicitly. [Database schema](schema.md)

## Recovery

Use backward-compatible schema changes so a prior application image can run during rollback. For an incompatible change, follow the release recovery plan and restore test evidence. A database restore can discard later writes and needs separate authorization. [Backup and restore](../operations/backup-and-restore.md)
