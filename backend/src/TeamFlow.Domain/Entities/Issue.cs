using TeamFlow.Domain.Enums;

namespace TeamFlow.Domain.Entities;

public class Issue
{
  public Guid Id { get; set; } = Guid.CreateVersion7();
  public Guid WorkspaceId { get; set; }
  public Guid ProjectId { get; set; }
  public Guid ReporterUserId { get; set; }
  public Guid? AssigneeUserId { get; set; }
  public Guid? SprintId { get; set; }
  public int IssueNumber { get; set; }
  public required string Title { get; set; }
  public string? Description { get; set; }
  public IssueType IssueType { get; set; }
  public IssueStatus Status { get; set; }
  public IssuePriority Priority { get; set; }
  public decimal Position { get; set; }
  public long Version { get; set; }
  public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
  public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
  public DateTimeOffset? DueAt { get; set; }
  public DateTimeOffset? CompletedAt { get; set; }
  public DateTimeOffset? ArchivedAt { get; set; }
}
