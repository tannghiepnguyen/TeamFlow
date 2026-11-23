using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamFlow.Domain.Entities;
using TeamFlow.Infrastructure.Identity;

namespace TeamFlow.Infrastructure.Persistence.Configurations;

public sealed class IssueConfiguration : IEntityTypeConfiguration<Issue>
{
  public void Configure(EntityTypeBuilder<Issue> builder)
  {
    builder.HasKey(issue => issue.Id);
    builder.HasAlternateKey(issue => new { issue.WorkspaceId, issue.Id });

    builder.Property(issue => issue.Title).IsRequired().HasMaxLength(250);
    builder.Property(issue => issue.IssueType).HasConversion<string>().HasMaxLength(20);
    builder.Property(issue => issue.Status).HasConversion<string>().HasMaxLength(30);
    builder.Property(issue => issue.Priority).HasConversion<string>().HasMaxLength(20);
    builder.Property(issue => issue.Position).HasPrecision(20, 10);
    builder.Property(issue => issue.Version).IsConcurrencyToken();

    builder.HasIndex(issue => new { issue.ProjectId, issue.IssueNumber }).IsUnique();

    builder.HasOne<Project>()
      .WithMany()
      .HasForeignKey(issue => new { issue.WorkspaceId, issue.ProjectId })
      .HasPrincipalKey(project => new { project.WorkspaceId, project.Id })
      .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne<Sprint>()
      .WithMany()
      .HasForeignKey(issue => new { issue.WorkspaceId, issue.ProjectId, issue.SprintId })
      .HasPrincipalKey(sprint => new { sprint.WorkspaceId, sprint.ProjectId, sprint.Id })
      .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne<ApplicationUser>()
      .WithMany()
      .HasForeignKey(issue => issue.ReporterUserId)
      .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne<ApplicationUser>()
      .WithMany()
      .HasForeignKey(issue => issue.AssigneeUserId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
