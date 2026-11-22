namespace TeamFlow.Domain.Entities;

public class IssueAttachment
{
  public Guid Id { get; set; } = Guid.CreateVersion7();
  public Guid WorkspaceId { get; set; }
  public Guid IssueId { get; set; }
  public Guid UploadedByUserId { get; set; }
  public required string OriginalFileName { get; set; }
  public required string ObjectKey { get; set; }
  public required string ContentType { get; set; }
  public long SizeBytes { get; set; }
  public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
  public DateTimeOffset? DeletedAt { get; set; }
}
