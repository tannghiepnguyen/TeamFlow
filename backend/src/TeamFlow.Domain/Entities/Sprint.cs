using TeamFlow.Domain.Enums;

namespace TeamFlow.Domain.Entities;

public class Sprint
{
  public Guid Id { get; set; } = Guid.CreateVersion7();
  public Guid WorkspaceId { get; set; }
  public Guid ProjectId { get; set; }
  public required string Name { get; set; }
  public string? Goal { get; set; }
  public DateTimeOffset StartAt { get; set; }
  public DateTimeOffset EndAt { get; set; }
  public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
  public SprintStatus Status { get; set; }
}
