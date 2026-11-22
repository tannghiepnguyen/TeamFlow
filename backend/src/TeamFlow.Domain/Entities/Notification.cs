using System.Text.Json;
using TeamFlow.Domain.Enums;

namespace TeamFlow.Domain.Entities;

public class Notification
{
  public Guid Id { get; set; } = Guid.CreateVersion7();
  public Guid WorkspaceId { get; set; }
  public Guid RecipientUserId { get; set; }
  public Guid SourceEventId { get; set; }
  public NotificationType Type { get; set; }
  public required JsonElement Payload { get; set; }
  public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
  public DateTimeOffset? ReadAt { get; set; }
}
