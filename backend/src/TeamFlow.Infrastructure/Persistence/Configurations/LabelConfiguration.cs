using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamFlow.Domain.Entities;

namespace TeamFlow.Infrastructure.Persistence.Configurations;

public sealed class LabelConfiguration : IEntityTypeConfiguration<Label>
{
  public void Configure(EntityTypeBuilder<Label> builder)
  {
    builder.ToTable(nameof(Label));
    builder.HasKey(label => label.Id);
    builder.HasAlternateKey(label => new { label.WorkspaceId, label.Id });

    builder.Property(label => label.Name).IsRequired().HasMaxLength(60);
    builder.Property(label => label.Color).IsRequired().HasMaxLength(7);
    builder.Property<string>("NormalizedName")
      .HasComputedColumnSql("""lower("Name")""", stored: true)
      .IsRequired();

    builder.HasIndex(nameof(Label.WorkspaceId), "NormalizedName").IsUnique();
    builder.ToTable(table => table.HasCheckConstraint("CK_Label_Color", "\"Color\" ~ '^#[0-9A-Fa-f]{6}$'"));

    builder.HasOne<Workspace>()
      .WithMany()
      .HasForeignKey(label => label.WorkspaceId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
