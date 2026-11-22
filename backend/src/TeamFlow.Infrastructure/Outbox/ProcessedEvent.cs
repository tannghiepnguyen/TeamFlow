namespace TeamFlow.Infrastructure.Outbox;

public class ProcessedEvent
{
  public required string ConsumerName { get; set; }
  public Guid EventId { get; set; }
  public DateTimeOffset ProcessedAt { get; set; }
}
