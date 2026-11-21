using System;
using TeamFlow.Domain.Enums;

namespace TeamFlow.Domain.Entities;

public class WorkspaceMember
{
  public Guid WorkspaceId { get; set; }
  public Guid UserId { get; set; }
  public WorkspaceRole Role { get; set; }
  public DateTimeOffset JoinedAt { get; set; } = DateTimeOffset.UtcNow;
  public Guid? InvitedByUserId { get; set; }
  public bool IsActive { get; set; } = true;
}
