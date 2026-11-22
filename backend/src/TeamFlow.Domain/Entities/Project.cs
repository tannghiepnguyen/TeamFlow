namespace TeamFlow.Domain.Entities;

public class Project
{
  public Guid Id { get; set; } = Guid.CreateVersion7();
  public Guid WorkspaceId { get; set; }
  public Guid CreatedByUserId { get; set; }
  public required string Name { get; set; }
  public required string ProjectKey { get; set; }
  public string? Description { get; set; }
  public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
  public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
  public DateTimeOffset? ArchivedAt { get; set; }
}
