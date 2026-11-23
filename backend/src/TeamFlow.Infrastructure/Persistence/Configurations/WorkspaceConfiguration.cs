using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamFlow.Domain.Entities;
using TeamFlow.Infrastructure.Identity;

namespace TeamFlow.Infrastructure.Persistence.Configurations;

public sealed class WorkspaceConfiguration : IEntityTypeConfiguration<Workspace>
{
  public void Configure(EntityTypeBuilder<Workspace> builder)
  {
    builder.HasKey(workspace => workspace.Id);

    builder.Property(workspace => workspace.Name).IsRequired().HasMaxLength(150);
    builder.Property(workspace => workspace.Slug).IsRequired().HasMaxLength(80);
    builder.Property(workspace => workspace.Description).HasMaxLength(500);

    builder.HasIndex(workspace => workspace.Slug).IsUnique();

    builder.HasOne<ApplicationUser>()
      .WithMany()
      .HasForeignKey(workspace => workspace.CreatedByUserId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
