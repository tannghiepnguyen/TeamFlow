using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamFlow.Domain.Entities;
using TeamFlow.Infrastructure.Identity;

namespace TeamFlow.Infrastructure.Persistence.Configurations;

public sealed class WorkspaceMemberConfiguration : IEntityTypeConfiguration<WorkspaceMember>
{
  public void Configure(EntityTypeBuilder<WorkspaceMember> builder)
  {
    builder.HasKey(member => new { member.WorkspaceId, member.UserId });

    builder.Property(member => member.Role).HasConversion<string>().HasMaxLength(20);
    builder.Property(member => member.IsActive).HasDefaultValue(true);

    builder.HasOne<Workspace>()
      .WithMany()
      .HasForeignKey(member => member.WorkspaceId)
      .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne<ApplicationUser>()
      .WithMany()
      .HasForeignKey(member => member.UserId)
      .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne<ApplicationUser>()
      .WithMany()
      .HasForeignKey(member => member.InvitedByUserId)
      .OnDelete(DeleteBehavior.Restrict);

    builder.HasIndex(member => new { member.UserId, member.IsActive });
  }
}
