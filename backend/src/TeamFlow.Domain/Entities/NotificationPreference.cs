namespace TeamFlow.Domain.Entities;

public class NotificationPreference
{
  public Guid UserId { get; set; }
  public required string EventType { get; set; }
  public bool EmailEnabled { get; set; }
  public bool InAppEnabled { get; set; }
}
