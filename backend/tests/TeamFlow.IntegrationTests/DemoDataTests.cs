using Microsoft.EntityFrameworkCore;
using Npgsql;
using TeamFlow.Domain.Entities;
using TeamFlow.Infrastructure.Identity;
using TeamFlow.Infrastructure.Persistence;
using TeamFlow.Infrastructure.Persistence.Configurations;

namespace TeamFlow.IntegrationTests;

public class DemoDataTests
{
  private static string? ConnectionString => Environment.GetEnvironmentVariable("TEAMFLOW_TEST_POSTGRES");
  public static bool HasPostgreSql => !string.IsNullOrWhiteSpace(ConnectionString);

  [Fact(SkipUnless = nameof(HasPostgreSql), Skip = "Set TEAMFLOW_TEST_POSTGRES to a disposable PostgreSQL server.")]
  public Task SeedsEveryEmptyTableAndLeavesPopulatedTablesAlone() =>
    WithDatabaseAsync(async connectionString =>
    {
      await using (var context = CreateContext(connectionString, seed: true))
      {
        await context.Database.MigrateAsync(TestContext.Current.CancellationToken);

        using var commandBuilder = new NpgsqlCommandBuilder();
        foreach (var entity in context.Model.GetEntityTypes())
        {
          var table = commandBuilder.QuoteIdentifier(entity.GetTableName()!);
          var countQuery = $"SELECT count(*)::integer AS \"Value\" FROM {table}";
          var count = await context.Database.SqlQueryRaw<int>(countQuery)
            .SingleAsync(TestContext.Current.CancellationToken);
          Assert.True(count == 1, $"Expected one row in {table}, found {count}.");
        }

        var user = await context.Users.SingleAsync(TestContext.Current.CancellationToken);
        Assert.True(user.IsDeactivated);
        Assert.Null(user.PasswordHash);
        var issue = await context.Issues.SingleAsync(TestContext.Current.CancellationToken);
        var sequence = await context.ProjectIssueSequences.SingleAsync(TestContext.Current.CancellationToken);
        Assert.Equal(issue.ProjectId, sequence.ProjectId);
        Assert.Equal(issue.IssueNumber + 1, sequence.NextNumber);

        await context.Labels.ExecuteUpdateAsync(setters => setters.SetProperty(label => label.Name, "Edited label"),
          TestContext.Current.CancellationToken);
        await context.Comments.ExecuteDeleteAsync(TestContext.Current.CancellationToken);
        context.Comments.Add(new Comment
        {
          WorkspaceId = issue.WorkspaceId, IssueId = issue.Id, AuthorUserId = user.Id, Body = "Existing comment"
        });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        await context.NotificationPreferences.ExecuteDeleteAsync(TestContext.Current.CancellationToken);
      }

      using var rerun = CreateContext(connectionString, seed: true);
      rerun.Database.Migrate();
      Assert.Equal("Edited label", rerun.Labels.Single().Name);
      Assert.Equal("Existing comment", rerun.Comments.Single().Body);
      Assert.True(rerun.NotificationPreferences.Single().InAppEnabled);
      Assert.Single(rerun.Issues);
      Assert.Single(rerun.OutboxMessages);
    });

  [Fact(SkipUnless = nameof(HasPostgreSql), Skip = "Set TEAMFLOW_TEST_POSTGRES to a disposable PostgreSQL server.")]
  public Task SkipsEmptyDependentTablesWhenOnlyAnUnrelatedUserExists() =>
    WithDatabaseAsync(async connectionString =>
    {
      await using (var context = CreateContext(connectionString, seed: false))
      {
        await context.Database.MigrateAsync(TestContext.Current.CancellationToken);
        Assert.Empty(context.Roles);
        context.Users.Add(new ApplicationUser
        {
          Id = Guid.NewGuid(), UserName = "existing@example.org", DisplayName = "Existing user"
        });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
      }

      await using var rerun = CreateContext(connectionString, seed: true);
      await rerun.Database.MigrateAsync(TestContext.Current.CancellationToken);
      Assert.Equal("existing@example.org", (await rerun.Users.SingleAsync(TestContext.Current.CancellationToken)).UserName);
      Assert.Empty(rerun.Workspaces);
      Assert.Empty(rerun.Projects);
      Assert.Empty(rerun.NotificationPreferences);
      Assert.Single(rerun.Roles);
    });

  private static async Task WithDatabaseAsync(Func<string, Task> test)
  {
    var connectionString = new NpgsqlConnectionStringBuilder(ConnectionString)
    {
      Database = $"teamflow_test_{Guid.NewGuid():N}"
    }.ConnectionString;

    try
    {
      await test(connectionString);
    }
    finally
    {
      await using var context = CreateContext(connectionString, seed: false);
      await context.Database.EnsureDeletedAsync();
    }
  }

  private static AppDbContext CreateContext(string connectionString, bool seed)
  {
    var options = new DbContextOptionsBuilder<AppDbContext>().UseNpgsql(connectionString);
    if (seed)
      DemoDataConfiguration.Configure(options);

    return new AppDbContext(options.Options);
  }
}
