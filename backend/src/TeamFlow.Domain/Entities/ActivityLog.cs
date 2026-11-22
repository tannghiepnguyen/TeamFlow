using System.Text.Json;

namespace TeamFlow.Domain.Entities;

public class ActivityLog
{
  public Guid Id { get; set; } = Guid.CreateVersion7();
  public Guid WorkspaceId { get; set; }
  public Guid IssueId { get; set; }
  public Guid? ActorUserId { get; set; }
  public required string Action { get; set; }
  public required JsonElement Changes { get; set; }
  public DateTimeOffset OccurredAt { get; set; }
}
