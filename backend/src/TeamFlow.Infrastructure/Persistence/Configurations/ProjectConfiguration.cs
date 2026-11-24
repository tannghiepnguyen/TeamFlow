using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamFlow.Domain.Entities;
using TeamFlow.Infrastructure.Identity;

namespace TeamFlow.Infrastructure.Persistence.Configurations;

public sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
  public void Configure(EntityTypeBuilder<Project> builder)
  {
    builder.ToTable(nameof(Project));
    builder.HasKey(project => project.Id);
    builder.HasAlternateKey(project => new { project.WorkspaceId, project.Id });

    builder.Property(project => project.Name).IsRequired().HasMaxLength(150);
    builder.Property(project => project.ProjectKey).IsRequired().HasMaxLength(12);

    builder.HasIndex(project => new { project.WorkspaceId, project.ProjectKey }).IsUnique();

    builder.HasOne<Workspace>()
      .WithMany()
      .HasForeignKey(project => project.WorkspaceId)
      .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne<ApplicationUser>()
      .WithMany()
      .HasForeignKey(project => project.CreatedByUserId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
