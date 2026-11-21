using System;
using TeamFlow.Domain.Enums;

namespace TeamFlow.Domain.Entities;

public class WorkspaceInvitation
{
  public Guid Id { get; set; } = Guid.CreateVersion7();
  public Guid WorkspaceId { get; set; }
  public Guid InvitedByUserId { get; set; }
  public string EmailNormalized { get; set; } = string.Empty;
  public WorkspaceRole Role { get; set; }
  public string TokenHash { get; set; } = string.Empty;
  public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
  public DateTimeOffset? ExpiresAt { get; set; }
  public DateTimeOffset? AcceptedAt { get; set; }
  public DateTimeOffset? RevokedAt { get; set; }

}
