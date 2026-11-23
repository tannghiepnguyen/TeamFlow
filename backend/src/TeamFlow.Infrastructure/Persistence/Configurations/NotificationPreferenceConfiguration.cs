using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamFlow.Domain.Entities;
using TeamFlow.Infrastructure.Identity;

namespace TeamFlow.Infrastructure.Persistence.Configurations;

public sealed class NotificationPreferenceConfiguration : IEntityTypeConfiguration<NotificationPreference>
{
  public void Configure(EntityTypeBuilder<NotificationPreference> builder)
  {
    builder.HasKey(preference => new { preference.UserId, preference.EventType });

    builder.Property(preference => preference.EventType).IsRequired().HasMaxLength(60);

    builder.HasOne<ApplicationUser>()
      .WithMany()
      .HasForeignKey(preference => preference.UserId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
