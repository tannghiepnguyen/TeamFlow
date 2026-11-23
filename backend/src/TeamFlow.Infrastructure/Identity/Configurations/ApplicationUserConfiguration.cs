using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TeamFlow.Infrastructure.Identity.Configurations;

public sealed class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
  public void Configure(EntityTypeBuilder<ApplicationUser> builder)
  {
    builder.Property(user => user.DisplayName).IsRequired().HasMaxLength(120);
    builder.Property(user => user.IsDeactivated).HasDefaultValue(false);
    builder.HasIndex(user => user.NormalizedEmail).IsUnique();
  }
}
