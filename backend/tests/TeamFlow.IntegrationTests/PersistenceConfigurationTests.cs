using Microsoft.EntityFrameworkCore;
using TeamFlow.Domain.Entities;
using TeamFlow.Infrastructure.Persistence;

namespace TeamFlow.IntegrationTests;

public class PersistenceConfigurationTests
{
  [Fact]
  public void PostgreSqlSchemaDefinesKeyAndPropertyConstraints()
  {
    using var context = CreateContext();

    var sql = context.Database.GenerateCreateScript();

    Assert.Contains("\"Slug\" character varying(80) NOT NULL", sql);
    Assert.Contains("\"ProjectKey\" character varying(12) NOT NULL", sql);
    Assert.Contains("\"Title\" character varying(250) NOT NULL", sql);
    Assert.Contains("\"Position\" numeric(20,10) NOT NULL", sql);
    Assert.Contains("PRIMARY KEY (\"WorkspaceId\", \"UserId\")", sql);
    Assert.Contains("CREATE UNIQUE INDEX \"IX_Workspace_Slug\" ON \"Workspace\" (\"Slug\")", sql);
    Assert.Contains("CREATE UNIQUE INDEX \"IX_Project_WorkspaceId_ProjectKey\" ON \"Project\" (\"WorkspaceId\", \"ProjectKey\")", sql);
    Assert.Contains("CREATE UNIQUE INDEX \"IX_Issue_ProjectId_IssueNumber\" ON \"Issue\" (\"ProjectId\", \"IssueNumber\")", sql);
  }

  [Fact]
  public void IssueProjectForeignKeyIncludesWorkspace()
  {
    using var context = CreateContext();

    var sql = context.Database.GenerateCreateScript();

    Assert.Contains(
      "FOREIGN KEY (\"WorkspaceId\", \"ProjectId\") REFERENCES \"Project\" (\"WorkspaceId\", \"Id\") ON DELETE RESTRICT",
      sql);
    Assert.True(context.Model.FindEntityType(typeof(Issue))!.FindProperty(nameof(Issue.Version))!.IsConcurrencyToken);
  }

  [Theory]
  [InlineData("ActivityLog", "Issue", "\"WorkspaceId\", \"IssueId\"", "\"WorkspaceId\", \"Id\"")]
  [InlineData("Comment", "Issue", "\"WorkspaceId\", \"IssueId\"", "\"WorkspaceId\", \"Id\"")]
  [InlineData("IssueAttachment", "Issue", "\"WorkspaceId\", \"IssueId\"", "\"WorkspaceId\", \"Id\"")]
  [InlineData("IssueLabel", "Issue", "\"WorkspaceId\", \"IssueId\"", "\"WorkspaceId\", \"Id\"")]
  [InlineData("IssueLabel", "Label", "\"WorkspaceId\", \"LabelId\"", "\"WorkspaceId\", \"Id\"")]
  [InlineData("Sprint", "Project", "\"WorkspaceId\", \"ProjectId\"", "\"WorkspaceId\", \"Id\"")]
  [InlineData("Issue", "Sprint", "\"WorkspaceId\", \"ProjectId\", \"SprintId\"", "\"WorkspaceId\", \"ProjectId\", \"Id\"")]
  public void ChildRecordsUseScopedForeignKeys(
    string childTable, string parentTable, string childColumns, string parentColumns)
  {
    using var context = CreateContext();

    var sql = context.Database.GenerateCreateScript();

    Assert.Contains(sql.Split('\n'), line =>
      line.Contains($"CONSTRAINT \"FK_{childTable}_{parentTable}_", StringComparison.Ordinal)
      && line.Contains(
        $"FOREIGN KEY ({childColumns}) REFERENCES \"{parentTable}\" ({parentColumns}) ON DELETE RESTRICT",
        StringComparison.Ordinal));
  }

  [Fact]
  public void PostgreSqlSchemaDefinesChecksAndDeduplication()
  {
    using var context = CreateContext();

    var sql = context.Database.GenerateCreateScript();

    Assert.Contains("CHECK (char_length(\"Body\") <= 10000)", sql);
    Assert.Contains("GENERATED ALWAYS AS (lower(\"Name\")) STORED", sql);
    Assert.Contains("CREATE UNIQUE INDEX \"IX_Label_WorkspaceId_NormalizedName\" ON \"Label\" (\"WorkspaceId\", \"NormalizedName\")", sql);
    Assert.Contains("CHECK (\"StartAt\" < \"EndAt\")", sql);
    Assert.Contains("CREATE UNIQUE INDEX \"IX_Sprint_WorkspaceId_ProjectId\" ON \"Sprint\" (\"WorkspaceId\", \"ProjectId\") WHERE \"Status\" = 'Active'", sql);
    Assert.Contains("CREATE UNIQUE INDEX \"IX_Notification_RecipientUserId_SourceEventId_Type\" ON \"Notification\" (\"RecipientUserId\", \"SourceEventId\", \"Type\")", sql);
    Assert.Contains("ON \"OutboxMessage\" (\"AvailableAt\", \"Id\") WHERE \"ProcessedAt\" IS NULL", sql);
    Assert.Contains("PRIMARY KEY (\"ConsumerName\", \"EventId\")", sql);
    Assert.Contains("\"Changes\" jsonb NOT NULL", sql);
    Assert.Contains("\"Payload\" jsonb NOT NULL", sql);
  }

  [Fact]
  public void PostgreSqlModelIncludesAllCatalogEntitiesAndIdentityConstraints()
  {
    using var context = CreateContext();

    var sql = context.Database.GenerateCreateScript();

    Assert.Equal(23, context.Model.GetEntityTypes().Count());
    Assert.Contains("CREATE TABLE \"AspNetUsers\"", sql);
    Assert.Contains("CREATE TABLE \"AspNetRoles\"", sql);
    Assert.Contains("CREATE TABLE \"AspNetUserRoles\"", sql);
    Assert.Contains("CREATE TABLE \"AspNetUserClaims\"", sql);
    Assert.Contains("CREATE TABLE \"AspNetRoleClaims\"", sql);
    Assert.Contains("CREATE TABLE \"AspNetUserLogins\"", sql);
    Assert.Contains("CREATE TABLE \"AspNetUserTokens\"", sql);
    Assert.Contains("\"DisplayName\" character varying(120) NOT NULL", sql);
    Assert.Contains("CREATE UNIQUE INDEX \"EmailIndex\" ON \"AspNetUsers\" (\"NormalizedEmail\")", sql);
    Assert.Contains("FOREIGN KEY (\"RecipientUserId\") REFERENCES \"AspNetUsers\" (\"Id\") ON DELETE RESTRICT", sql);
  }

  private static AppDbContext CreateContext() =>
    new(new DbContextOptionsBuilder<AppDbContext>().UseNpgsql().Options);
}
