using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamFlow.Domain.Entities;
using TeamFlow.Domain.Enums;

namespace TeamFlow.Infrastructure.Persistence.Configurations;

public sealed class SprintConfiguration : IEntityTypeConfiguration<Sprint>
{
  public void Configure(EntityTypeBuilder<Sprint> builder)
  {
    builder.HasKey(sprint => sprint.Id);
    builder.HasAlternateKey(sprint => new { sprint.WorkspaceId, sprint.ProjectId, sprint.Id });

    builder.Property(sprint => sprint.Name).IsRequired().HasMaxLength(120);
    builder.Property(sprint => sprint.Status).HasConversion<string>().HasMaxLength(20);

    builder.HasIndex(sprint => new { sprint.WorkspaceId, sprint.ProjectId })
      .IsUnique()
      .HasFilter($"\"Status\" = '{SprintStatus.Active}'");
    builder.ToTable(table => table.HasCheckConstraint("CK_Sprint_DateRange", "\"StartAt\" < \"EndAt\""));

    builder.HasOne<Project>()
      .WithMany()
      .HasForeignKey(sprint => new { sprint.WorkspaceId, sprint.ProjectId })
      .HasPrincipalKey(project => new { project.WorkspaceId, project.Id })
      .OnDelete(DeleteBehavior.Restrict);
  }
}
