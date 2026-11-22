using System.Text.Json;

namespace TeamFlow.Infrastructure.Outbox;

public class OutboxMessage
{
  public Guid Id { get; set; } = Guid.CreateVersion7();
  public Guid WorkspaceId { get; set; }
  public Guid AggregateId { get; set; }
  public required string EventType { get; set; }
  public required string AggregateType { get; set; }
  public required JsonElement Payload { get; set; }
  public DateTimeOffset OccurredAt { get; set; }
  public DateTimeOffset AvailableAt { get; set; }
  public DateTimeOffset? ProcessedAt { get; set; }
  public DateTimeOffset? LockedUntil { get; set; }
  public int Attempts { get; set; }
  public string? LastError { get; set; }
  public string? LockedBy { get; set; }
}
