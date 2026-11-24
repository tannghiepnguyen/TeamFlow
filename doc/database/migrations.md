# Database migrations

`AppDbContext` owns the TeamFlow and ASP.NET Core Identity tables in one PostgreSQL schema. Infrastructure stores the [mappings](../../backend/src/TeamFlow.Infrastructure/Persistence/Configurations) and [migrations](../../backend/src/TeamFlow.Infrastructure/Persistence/Migrations). The API supplies the connection string. API and Worker startup leave migrations to an explicit database update.

The API binds its database settings through the Options pattern. It reads `ConnectionStrings:TeamFlow` first and falls back to `ConnectionStrings:DefaultConnection`. A missing connection string is reported when EF Core resolves the context.

## Local setup

Install the EF Core CLI if it is missing. Run these PowerShell commands from the repository root. Replace the example connection string with the settings for a local PostgreSQL database and keep credentials out of source control.

```powershell
dotnet tool install --global dotnet-ef --version 11.0.0
$env:ConnectionStrings__TeamFlow = 'Host=localhost;Database=teamflow_dev;Username=teamflow;Password=replace-me'
dotnet ef database update --project backend/src/TeamFlow.Infrastructure --startup-project backend/src/TeamFlow.API --context AppDbContext
```

## Demo data

To enable sample rows, set `$env:Persistence__SeedDemoData = 'true'` and add `-- --environment Development` to the database update command. You can run the command again after the migration has already been applied.

The [demo data configuration](../../backend/src/TeamFlow.Infrastructure/Persistence/Configurations/DemoDataConfiguration.cs) adds one row for each empty table that has the required demo parent records. Existing rows stay as they are. If a populated parent table lacks the matching demo record, the related children stay unseeded. The sample account has no sign-in credentials. The invitation is revoked and the outbox event is already processed.

Disable demo seeding before rolling back the initial migration. Applying a generated SQL script creates the schema. The demo data runs through EF Core database updates with the development setting enabled. [EF Core data seeding](https://learn.microsoft.com/en-us/ef/core/modeling/data-seeding)

## Schema changes

- Create a new migration for each schema change. Review the SQL, model snapshot, constraints, indexes and compatibility with existing data.
- Verify migrations against a disposable PostgreSQL database from an empty database and the previous supported schema.
- Apply reviewed SQL or migration bundles in a controlled release step with an explicit target database and authorization. Preserve applied migrations.
- Use backward-compatible schema changes so a prior application image can run during rollback. Follow the [backup and recovery plan](../operations/backup-and-restore.md) for incompatible changes.
