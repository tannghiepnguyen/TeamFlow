using System;

namespace TeamFlow.Domain.Entities;

public class Workspace
{
  public Guid Id { get; set; } = Guid.CreateVersion7();
  public string Name { get; set; } = string.Empty;
  public string Slug { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
  public DateTimeOffset? UpdatedAt { get; set; }
  public DateTimeOffset? ArchivedAt { get; set; }
}
