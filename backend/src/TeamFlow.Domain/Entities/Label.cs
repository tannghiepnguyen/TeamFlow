namespace TeamFlow.Domain.Entities;

public class Label
{
  public Guid Id { get; set; } = Guid.CreateVersion7();
  public Guid WorkspaceId { get; set; }
  public required string Name { get; set; }
  public required string Color { get; set; }
  public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
