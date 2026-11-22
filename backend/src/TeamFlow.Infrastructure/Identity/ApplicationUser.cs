using Microsoft.AspNetCore.Identity;

namespace TeamFlow.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
  public string DisplayName { get; set; } = string.Empty;
  public string? AvatarUrl { get; set; }
  public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
  public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
  public bool IsDeactivated { get; set; }
}
