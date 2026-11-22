namespace TeamFlow.Domain.Entities;

public class Comment
{
  public Guid Id { get; set; } = Guid.CreateVersion7();
  public Guid WorkspaceId { get; set; }
  public Guid IssueId { get; set; }
  public Guid AuthorUserId { get; set; }
  public required string Body { get; set; }
  public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
  public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
  public DateTimeOffset? DeletedAt { get; set; }
}
