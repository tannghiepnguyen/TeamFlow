using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamFlow.Domain.Entities;

namespace TeamFlow.Infrastructure.Persistence.Configurations;

public sealed class ProjectIssueSequenceConfiguration : IEntityTypeConfiguration<ProjectIssueSequence>
{
  public void Configure(EntityTypeBuilder<ProjectIssueSequence> builder)
  {
    builder.HasKey(sequence => sequence.ProjectId);

    builder.Property(sequence => sequence.NextNumber).HasDefaultValue(1);
    builder.ToTable(table => table.HasCheckConstraint("CK_ProjectIssueSequence_NextNumber", "\"NextNumber\" > 0"));

    builder.HasOne<Project>()
      .WithOne()
      .HasForeignKey<ProjectIssueSequence>(sequence => sequence.ProjectId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
