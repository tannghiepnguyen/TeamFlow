using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamFlow.Domain.Entities;
using TeamFlow.Infrastructure.Identity;

namespace TeamFlow.Infrastructure.Persistence.Configurations;

public sealed class ActivityLogConfiguration : IEntityTypeConfiguration<ActivityLog>
{
  public void Configure(EntityTypeBuilder<ActivityLog> builder)
  {
    builder.ToTable(nameof(ActivityLog));
    builder.HasKey(activity => activity.Id);

    builder.Property(activity => activity.Action).IsRequired().HasMaxLength(60);
    builder.Property(activity => activity.Changes).HasColumnType("jsonb");

    builder.HasIndex(activity => new { activity.WorkspaceId, activity.IssueId, activity.OccurredAt });

    builder.HasOne<Issue>()
      .WithMany()
      .HasForeignKey(activity => new { activity.WorkspaceId, activity.IssueId })
      .HasPrincipalKey(issue => new { issue.WorkspaceId, issue.Id })
      .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne<ApplicationUser>()
      .WithMany()
      .HasForeignKey(activity => activity.ActorUserId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
