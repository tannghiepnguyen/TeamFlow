using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamFlow.Domain.Entities;
using TeamFlow.Infrastructure.Identity;

namespace TeamFlow.Infrastructure.Persistence.Configurations;

public sealed class WorkspaceInvitationConfiguration : IEntityTypeConfiguration<WorkspaceInvitation>
{
  public void Configure(EntityTypeBuilder<WorkspaceInvitation> builder)
  {
    builder.ToTable(nameof(WorkspaceInvitation));
    builder.HasKey(invitation => invitation.Id);

    builder.Property(invitation => invitation.EmailNormalized).IsRequired().HasMaxLength(256);
    builder.Property(invitation => invitation.Role).HasConversion<string>().HasMaxLength(20);
    builder.Property(invitation => invitation.TokenHash).IsRequired().HasMaxLength(128);

    builder.HasIndex(invitation => invitation.TokenHash).IsUnique();

    builder.HasOne<Workspace>()
      .WithMany()
      .HasForeignKey(invitation => invitation.WorkspaceId)
      .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne<ApplicationUser>()
      .WithMany()
      .HasForeignKey(invitation => invitation.InvitedByUserId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
