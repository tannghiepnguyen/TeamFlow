using System.Linq.Expressions;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TeamFlow.Domain.Entities;
using TeamFlow.Domain.Enums;
using TeamFlow.Infrastructure.Identity;
using TeamFlow.Infrastructure.Outbox;

namespace TeamFlow.Infrastructure.Persistence.Configurations;

public static class DemoDataConfiguration
{
  private static readonly Guid UserId = Guid.Parse("c198f1ae-67bb-435e-8e12-f7b3133dc200");
  private static readonly Guid RoleId = Guid.Parse("c198f1ae-67bb-435e-8e12-f7b3133dc201");
  private static readonly Guid WorkspaceId = Guid.Parse("c198f1ae-67bb-435e-8e12-f7b3133dc202");
  private static readonly Guid ProjectId = Guid.Parse("c198f1ae-67bb-435e-8e12-f7b3133dc203");
  private static readonly Guid SprintId = Guid.Parse("c198f1ae-67bb-435e-8e12-f7b3133dc204");
  private static readonly Guid IssueId = Guid.Parse("c198f1ae-67bb-435e-8e12-f7b3133dc205");
  private static readonly Guid LabelId = Guid.Parse("c198f1ae-67bb-435e-8e12-f7b3133dc206");
  private static readonly Guid EventId = Guid.Parse("c198f1ae-67bb-435e-8e12-f7b3133dc207");
  private static readonly DateTimeOffset SampleDate = new(2026, 1, 12, 9, 0, 0, TimeSpan.Zero);
  private const string DemoEmail = "demo@teamflow.example";
  private const string DemoProvider = "TeamFlow.Demo";
  private const string EventType = nameof(NotificationType.IssueAssigned);

  public static void Configure(DbContextOptionsBuilder options)
  {
    options.UseSeeding((context, _) => SeedAsync(context, useAsync: false, CancellationToken.None)
      .GetAwaiter().GetResult());
    options.UseAsyncSeeding((context, _, cancellationToken) =>
      SeedAsync(context, useAsync: true, cancellationToken));
  }

  private static async Task SeedAsync(DbContext context, bool useAsync, CancellationToken cancellationToken)
  {
    Task<bool> AnyAsync<T>(IQueryable<T> query) =>
      useAsync ? query.AnyAsync(cancellationToken) : Task.FromResult(query.Any());

    async Task<bool> AddIfEmptyAsync<T>(T row, bool parentsExist = true,
      Expression<Func<T, bool>>? matchesDemoRow = null) where T : class
    {
      if (!parentsExist)
        return false;

      // Use the whole table for the emptiness check, even if query filters are added later.
      var rows = context.Set<T>().IgnoreQueryFilters();
      if (await AnyAsync(rows))
        return matchesDemoRow is not null && await AnyAsync(rows.Where(matchesDemoRow));

      context.Add(row);
      return true;
    }

    var userExists = await AddIfEmptyAsync(new ApplicationUser
    {
      Id = UserId,
      UserName = DemoEmail,
      NormalizedUserName = DemoEmail.ToUpperInvariant(),
      Email = DemoEmail,
      NormalizedEmail = DemoEmail.ToUpperInvariant(),
      DisplayName = "TeamFlow Demo",
      IsDeactivated = true,
      LockoutEnabled = true,
      LockoutEnd = DateTimeOffset.MaxValue,
      CreatedAt = SampleDate,
      UpdatedAt = SampleDate
    }, matchesDemoRow: user => user.Id == UserId && user.NormalizedEmail == DemoEmail.ToUpperInvariant());

    var roleExists = await AddIfEmptyAsync(new IdentityRole<Guid>
    {
      Id = RoleId, Name = "Demo", NormalizedName = "DEMO"
    }, matchesDemoRow: role => role.Id == RoleId && role.NormalizedName == "DEMO");

    await AddIfEmptyAsync(new IdentityUserRole<Guid> { UserId = UserId, RoleId = RoleId },
      userExists && roleExists);
    await AddIfEmptyAsync(new IdentityUserClaim<Guid>
    {
      UserId = UserId, ClaimType = "teamflow:demo", ClaimValue = "true"
    }, userExists);
    await AddIfEmptyAsync(new IdentityRoleClaim<Guid>
    {
      RoleId = RoleId, ClaimType = "teamflow:demo", ClaimValue = "true"
    }, roleExists);
    await AddIfEmptyAsync(new IdentityUserLogin<Guid>
    {
      UserId = UserId, LoginProvider = DemoProvider, ProviderKey = "demo", ProviderDisplayName = "Demo"
    }, userExists);
    await AddIfEmptyAsync(new IdentityUserToken<Guid>
    {
      UserId = UserId, LoginProvider = DemoProvider, Name = "Sample"
    }, userExists);

    var workspaceExists = await AddIfEmptyAsync(new Workspace
    {
      Id = WorkspaceId, CreatedByUserId = UserId, Name = "TeamFlow Demo", Slug = "teamflow-demo",
      Description = "Sample workspace for exploring TeamFlow.", CreatedAt = SampleDate, UpdatedAt = SampleDate
    }, userExists, workspace => workspace.Id == WorkspaceId && workspace.Slug == "teamflow-demo");

    var membershipExists = await AddIfEmptyAsync(new WorkspaceMember
    {
      WorkspaceId = WorkspaceId, UserId = UserId, Role = WorkspaceRole.Admin, JoinedAt = SampleDate
    }, workspaceExists, member => member.WorkspaceId == WorkspaceId && member.UserId == UserId && member.IsActive);

    await AddIfEmptyAsync(new WorkspaceInvitation
    {
      WorkspaceId = WorkspaceId, InvitedByUserId = UserId, EmailNormalized = "INVITEE@TEAMFLOW.EXAMPLE",
      Role = WorkspaceRole.Member, TokenHash = "teamflow-demo-revoked", CreatedAt = SampleDate,
      ExpiresAt = SampleDate.AddDays(7), RevokedAt = SampleDate
    }, membershipExists);

    var projectExists = await AddIfEmptyAsync(new Project
    {
      Id = ProjectId, WorkspaceId = WorkspaceId, CreatedByUserId = UserId, Name = "Website Launch",
      ProjectKey = "WEB", Description = "Coordinate the first public release.",
      CreatedAt = SampleDate, UpdatedAt = SampleDate
    }, membershipExists, project => project.Id == ProjectId && project.WorkspaceId == WorkspaceId);

    var sprintExists = await AddIfEmptyAsync(new Sprint
    {
      Id = SprintId, WorkspaceId = WorkspaceId, ProjectId = ProjectId, Name = "Launch Preparation",
      Goal = "Review the launch checklist.", StartAt = SampleDate, EndAt = SampleDate.AddDays(14),
      CreatedAt = SampleDate, Status = SprintStatus.Planned
    }, projectExists, sprint => sprint.Id == SprintId && sprint.WorkspaceId == WorkspaceId && sprint.ProjectId == ProjectId);

    var labelExists = await AddIfEmptyAsync(new Label
    {
      Id = LabelId, WorkspaceId = WorkspaceId, Name = "Launch", Color = "#2563EB", CreatedAt = SampleDate
    }, workspaceExists, label => label.Id == LabelId && label.WorkspaceId == WorkspaceId);

    var sequences = context.Set<ProjectIssueSequence>().IgnoreQueryFilters();
    // Keep an existing sequence intact and avoid issuing a number it is about to allocate.
    var issueNumberAvailable = projectExists &&
      !await AnyAsync(sequences.Where(sequence => sequence.ProjectId == ProjectId && sequence.NextNumber == 1));
    var issueExists = await AddIfEmptyAsync(new Issue
    {
      Id = IssueId, WorkspaceId = WorkspaceId, ProjectId = ProjectId, ReporterUserId = UserId,
      AssigneeUserId = UserId, SprintId = sprintExists ? SprintId : null, IssueNumber = 1,
      Title = "Review the launch checklist", Description = "Confirm owners and due dates before launch.",
      IssueType = IssueType.Task, Status = IssueStatus.Todo, Priority = IssuePriority.Medium,
      Position = 1, Version = 1, CreatedAt = SampleDate, UpdatedAt = SampleDate, DueAt = SampleDate.AddDays(5)
    }, issueNumberAvailable, issue => issue.Id == IssueId && issue.WorkspaceId == WorkspaceId && issue.ProjectId == ProjectId);

    if (projectExists && !await AnyAsync(sequences))
    {
      var issueNumbers = context.Set<Issue>().IgnoreQueryFilters()
        .Where(issue => issue.WorkspaceId == WorkspaceId && issue.ProjectId == ProjectId)
        .Select(issue => (int?)issue.IssueNumber);
      var highestNumber = useAsync ? await issueNumbers.MaxAsync(cancellationToken) : issueNumbers.Max();
      var nextNumber = Math.Max(highestNumber ?? 0, issueExists ? 1 : 0) + 1;
      context.Add(new ProjectIssueSequence { ProjectId = ProjectId, NextNumber = nextNumber });
    }

    await AddIfEmptyAsync(new IssueLabel { WorkspaceId = WorkspaceId, IssueId = IssueId, LabelId = LabelId },
      issueExists && labelExists);
    await AddIfEmptyAsync(new Comment
    {
      WorkspaceId = WorkspaceId, IssueId = IssueId, AuthorUserId = UserId,
      Body = "The checklist is ready for review.", CreatedAt = SampleDate, UpdatedAt = SampleDate
    }, issueExists);
    await AddIfEmptyAsync(new IssueAttachment
    {
      WorkspaceId = WorkspaceId, IssueId = IssueId, UploadedByUserId = UserId,
      OriginalFileName = "launch-checklist.txt", ObjectKey = "demo/launch-checklist.txt",
      ContentType = "text/plain", SizeBytes = 0, CreatedAt = SampleDate, DeletedAt = SampleDate
    }, issueExists);
    await AddIfEmptyAsync(new ActivityLog
    {
      WorkspaceId = WorkspaceId, IssueId = IssueId, ActorUserId = UserId, Action = EventType,
      Changes = JsonSerializer.SerializeToElement(new { assigneeUserId = UserId }), OccurredAt = SampleDate
    }, issueExists);

    var eventPayload = JsonSerializer.SerializeToElement(new { issueId = IssueId, assigneeUserId = UserId });
    var eventExists = await AddIfEmptyAsync(new OutboxMessage
    {
      Id = EventId, WorkspaceId = WorkspaceId, AggregateId = IssueId, AggregateType = nameof(Issue),
      EventType = EventType, Payload = eventPayload, OccurredAt = SampleDate, AvailableAt = SampleDate,
      ProcessedAt = SampleDate
    }, issueExists, message => message.Id == EventId && message.WorkspaceId == WorkspaceId &&
      message.AggregateId == IssueId && message.EventType == EventType && message.ProcessedAt != null);

    await AddIfEmptyAsync(new ProcessedEvent
    {
      ConsumerName = "DemoNotifications", EventId = EventId, ProcessedAt = SampleDate
    }, eventExists);
    await AddIfEmptyAsync(new Notification
    {
      WorkspaceId = WorkspaceId, RecipientUserId = UserId, SourceEventId = EventId,
      Type = NotificationType.IssueAssigned, Payload = eventPayload, CreatedAt = SampleDate
    }, eventExists);
    await AddIfEmptyAsync(new NotificationPreference
    {
      UserId = UserId, EventType = EventType, InAppEnabled = true
    }, userExists);

    if (useAsync)
      await context.SaveChangesAsync(cancellationToken);
    else
      context.SaveChanges();
  }
}
